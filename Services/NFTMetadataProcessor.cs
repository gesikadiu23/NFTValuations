using Microsoft.Extensions.Caching.Memory;
using NFT;
using NFTValuations.Models.Data;
using NFTValuations.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace NFTValuations
{
    public class NFTMetadataProcessor
    {
        private readonly NFTMetadataExtractor _extractor;
        private readonly IMemoryCache _cache;

        public NFTMetadataProcessor(IMemoryCache cache)
        {
            _extractor = new NFTMetadataExtractor(cache);
            _cache = cache;
        }

        // Processes the metadata for a dictionary of NFTs and inserts the extracted data into a database.
        public async Task ProcessNFTMetadata(Dictionary<string, BigInteger> nftDictionary, DatabaseInserter databaseInserter)
        {
            // ConcurrentBag to store the DatabaseModel objects for parallel processing
            var databaseModels = new ConcurrentBag<DatabaseModel>();

            // Process the metadata for each NFT in parallel using Task.WhenAll
            await Task.WhenAll(nftDictionary.Select(async entry =>
            {
                var contractAddress = entry.Key;
                var tokenIndex = entry.Value;

                try
                {
                    // Check if the metadata for the NFT is already cached
                    if (!_cache.TryGetValue(contractAddress + tokenIndex.ToString(), out NFTMetadata metadata))
                    {
                        // Metadata not found in cache, extract it and store in cache
                        metadata = await _extractor.ExtractNFTMetadata(contractAddress, tokenIndex);

                        if (metadata != null)
                        {
                            // Cache the metadata with a unique cache key
                            _cache.Set(contractAddress + tokenIndex.ToString(), metadata);
                        }
                    }

                    if (metadata != null)
                    {
                        // Create a DatabaseModel object from the extracted metadata
                        var databaseModel = CreateDatabaseModel(metadata, contractAddress, tokenIndex);

                        // Add the DatabaseModel to the concurrent bag
                        databaseModels.Add(databaseModel);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during metadata extraction
                    Console.WriteLine($"Error extracting NFT metadata for contract address {contractAddress}, token index {tokenIndex}: {ex.Message}");
                }
            }));

            // Convert the concurrent bag to a list and insert the DatabaseModels into the database
            await databaseInserter.InsertDatabaseModels(databaseModels.ToList());
        }

        // Creates a DatabaseModel object from the NFTMetadata object.
        private static DatabaseModel CreateDatabaseModel(NFTMetadata metadata, string contractAddress, BigInteger tokenIndex)
        {
            var databaseModel = new DatabaseModel
            {
                // Map properties from NFTMetadata to DatabaseModel
                Name = metadata.Name,
                Description = metadata.Description,
                ExternalUrl = metadata.ExternalUrl,
                Media = metadata.Media,
                Properties = metadata.Properties?.Select(p => new PropertyModel
                {
                    Category = p.Category,
                    Property = p.Property
                }).ToList(),
                ContractTokenId = contractAddress + tokenIndex.ToString() // Concatenation of contract address and token ID
            };

            return databaseModel;
        }
    }
}

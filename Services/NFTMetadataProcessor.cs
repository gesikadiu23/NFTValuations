using Microsoft.Extensions.Caching.Memory;
using NFT.Data.DTOs;
using NFTValuations.Data.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace NFTValuations.Services
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
                    if (!_cache.TryGetValue(contractAddress + tokenIndex.ToString(), out NFTMetadataDTO metadata))
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
                        // Check if the NFT is already present in the database
                        bool isNewNFT = await IsNewNFT(contractAddress, tokenIndex, databaseInserter);

                        if (isNewNFT || HasChangedProperties(contractAddress, tokenIndex, metadata))
                        {
                            // Create a DatabaseModel object from the extracted metadata
                            var databaseModel = CreateDatabaseModel(metadata, contractAddress, tokenIndex);

                            // Add the DatabaseModel to the concurrent bag
                            databaseModels.Add(databaseModel);
                        }
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
        #region private methods

        // Checks if the NFT is new by checking if it exists in the database
        private async Task<bool> IsNewNFT(string contractAddress, BigInteger tokenIndex, DatabaseInserter databaseInserter)
        {
            // Return true if it's a new NFT, false otherwise
            // We can also compare if the object in the database is the same as the one that we are retrieving
            // We are doing that comparison only towards the cached NFT's and we are assuming the cache is always aligned with the database
            var existingNFT = await databaseInserter.GetNFTByContractAndToken(contractAddress, tokenIndex);
            return existingNFT == null;
        }

        // Checks if the properties of the NFT have changed by comparing with the cached metadata
        private bool HasChangedProperties(string contractAddress, BigInteger tokenIndex, NFTMetadataDTO metadata)
        {
            // Retrieve the cached metadata for the NFT
            if (_cache.TryGetValue(contractAddress + tokenIndex.ToString(), out NFTMetadataDTO cachedMetadata))
            {
                // Compare the properties of the cached metadata with the new metadata
                return !cachedMetadata.Properties.SequenceEqual(metadata.Properties);
            }

            // No cached metadata found, consider it as changed
            return true;
        }


        // Creates a DatabaseModel object from the NFTMetadata object.
        private static DatabaseModel CreateDatabaseModel(NFTMetadataDTO metadata, string contractAddress, BigInteger tokenIndex)
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

        #endregion

    }
}

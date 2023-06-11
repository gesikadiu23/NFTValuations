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

        public NFTMetadataProcessor()
        {
            _extractor = new NFTMetadataExtractor();
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
                    // Extract the metadata for the NFT
                    var metadata = await _extractor.ExtractNFTMetadata(contractAddress, tokenIndex);

                    if (metadata != null)
                    {
                        // Create a DatabaseModel object from the extracted metadata
                        var databaseModel = CreateDatabaseModel(metadata);

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
        private static DatabaseModel CreateDatabaseModel(NFTMetadata metadata)
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
                }).ToList()
            };

            return databaseModel;
        }
    }
}

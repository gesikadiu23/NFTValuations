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

        public async Task ProcessNFTMetadata(Dictionary<string, BigInteger> nftDictionary, DatabaseInserter databaseInserter)
        {
            var databaseModels = new ConcurrentBag<DatabaseModel>(); 

            await Task.WhenAll(nftDictionary.Select(async entry =>
            {
                var contractAddress = entry.Key;
                var tokenIndex = entry.Value;

                try
                {
                    var metadata = await _extractor.ExtractNFTMetadata(contractAddress, tokenIndex);

                    if (metadata != null)
                    {
                        var databaseModel = CreateDatabaseModel(metadata);

                        databaseModels.Add(databaseModel);
                    }
                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error extracting NFT metadata for contract address {contractAddress}, token index {tokenIndex}: {ex.Message}");
                }
            }));

            await databaseInserter.InsertDatabaseModels(databaseModels.ToList());
        }

        private static DatabaseModel CreateDatabaseModel(NFTMetadata metadata)
        {
            var databaseModel = new DatabaseModel
            {
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

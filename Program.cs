using Newtonsoft.Json;
using NFTValuations.Services;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace NFTValuations
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var contractAddress = "0xbc4ca0eda7647a8ab7c2061c2e118a18a936f13d";
            var tokenIndex = BigInteger.Parse("1234");

            var extractor = new NFTMetadataExtractor();
            var metadata = await extractor.ExtractNFTMetadata(contractAddress, tokenIndex);

            var result = new
            {
                Name = metadata.Name,
                Description = metadata.Description,
                ExternalUrl = metadata.ExternalUrl,
                Media = metadata.Media,
                Properties = metadata.Properties?.Select(p => new
                {
                    Category = p.Category,
                    Property = p.Property
                }).ToList()
            };

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}

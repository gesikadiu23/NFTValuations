using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NFTValuations.Services;

namespace NFTValuations
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Dictionary that maps contract addresses to token indices for NFTs
            // This is not dynamic as we are assuming there only these 15 NFTs
            // This could also read from an excel file the contract addresses and their Token Ids and process them dynamically
            var nftDictionary = new Dictionary<string, BigInteger>
            {
                 { "0x1a92f7381b9f03921564a437210bb9396471050c",BigInteger.Parse("0") },
                { "0xec9c519d49856fd2f8133a0741b4dbe002ce211b", BigInteger.Parse("30") },
                { "0xeaa4c58427c184413b04db47889b28b5c98ebb7b", BigInteger.Parse("1") },
                { "0x0b22fe0a2995c5389ac093400e52471dca8bb48a", BigInteger.Parse("0") },
                { "0xb4d06d46a8285f4ec79fd294f78a881799d8ced9", BigInteger.Parse("9896") },
                { "0xb668beb1fa440f6cf2da0399f8c28cab993bdd65", BigInteger.Parse("285") },
                { "0xbc4ca0eda7647a8ab7c2061c2e118a18a936f13d", BigInteger.Parse("1234") },
                { "0xdbb163b22e839a26d2a5011841cb4430019020f9", BigInteger.Parse("287") },
                { "0x1cb1a5e65610aeff2551a50f76a87a7d3fb649c6", BigInteger.Parse("30000000") },
                { "0x845dd2a7ee2a92a0518ab2135365ed63fdba0c88", BigInteger.Parse("18") },
                { "0x7bd29408f11d2bfc23c34f18275bbf23bb716bc7", BigInteger.Parse("4563") },
                { "0x059edd72cd353df5106d2b9cc5ab83a52287ac3a", BigInteger.Parse("2000000") },
                { "0x1b829b926a14634d36625e60165c0770c09d02b2", BigInteger.Parse("1000001") },
                { "0xd4e4078ca3495de5b1d4db434bebc5a986197782", BigInteger.Parse("1") },
                { "0x892848074ddea461a15f337250da3ce55580ca85", BigInteger.Parse("0") }
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var metadataProcessor = new NFTMetadataProcessor(memoryCache);
            var databaseInserter = new DatabaseInserter();

            //Processing The NFT dictionary
            await metadataProcessor.ProcessNFTMetadata(nftDictionary, databaseInserter);

        }
    }
}
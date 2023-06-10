using Nethereum.Web3;
using Newtonsoft.Json;
using NFT;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NFTValuations.Services
{
    public class NFTMetadataExtractor
    {
        private string infuraEndpoint = "https://mainnet.infura.io/v3/b3ec6029213a49d4b793ee85d19e1c94";

        public async Task<NFTMetadata> ExtractNFTMetadata(string contractAddress, BigInteger tokenIndex)
        {
            var ABI = NFTMetadata.ABI;

            // Create the contract service
            var web3 = new Web3(infuraEndpoint);
            var contract = web3.Eth.GetContract(ABI, contractAddress);

            // Get the tokenURI function
            var tokenUriFunction = contract.GetFunction("tokenURI");

            var uri = await tokenUriFunction.CallAsync<string>(tokenIndex);

            if (uri.StartsWith("ipfs://"))
            {
                uri = "https://gateway.pinata.cloud/ipfs/" + uri.Substring(7);
            }
            else
            {
                throw new NotSupportedException("Unsupported URI scheme");
            }

            using (var httpClient = new HttpClient())
            {
                var metadataJson = await httpClient.GetStringAsync(uri);
                var metadata = JsonConvert.DeserializeObject<NFTMetadata>(metadataJson);
                return metadata;
            }
        }
    }
}

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
            try
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
                else if (uri.StartsWith("https://ipfs.io/ipfs/"))
                {
                    uri = uri.Replace("https://ipfs.io/ipfs/", "https://gateway.pinata.cloud/ipfs/");
                }
                else if (uri.StartsWith("data:application/json;base64,"))
                {
                    var encodedJson = uri.Substring("data:application/json;base64,".Length);
                    var jsonBytes = Convert.FromBase64String(encodedJson);
                    var decodedJson = Encoding.UTF8.GetString(jsonBytes);
                    return JsonConvert.DeserializeObject<NFTMetadata>(decodedJson);
                }
                else if (uri.StartsWith("https://api.coolcatsnft.com/"))
                {
                    uri = "https://api.coolcatsnft.com/cat/" + uri.Substring("https://api.coolcatsnft.com/cat/".Length);
                }
                else if (uri.StartsWith("https://metadata.3landersnft.com/api/migration/"))
                {
                    uri = uri + ".json";
                }
                else if (uri.StartsWith("https://cryptocubes.io/api/v1/ipfs/"))
                {
                    uri = uri.Replace("https://cryptocubes.io/api/v1/ipfs/", "https://gateway.pinata.cloud/ipfs/");
                }
                else if (uri.StartsWith("https://arweave.net/"))
                {
                    uri = uri.Replace("https://arweave.net/", "https://gateway.pinata.cloud/ipfs/");
                }
                else if (uri.StartsWith("https://token.qql.art/"))
                {
                    uri = uri + "/metadata.json";
                }
                else if (uri.StartsWith("https://meebits.larvalabs.com/meebit/"))
                {
                    uri = "https://meebits.larvalabs.com/metadata/" + uri.Substring("https://meebits.larvalabs.com/meebit/".Length);
                }
                else if (uri.StartsWith("https://api.artblocks.io/token/"))
                {
                    uri = "https://api.artblocks.io/generated/token/" + uri.Substring("https://api.artblocks.io/token/".Length) + "/index.json";
                }
                else if (uri.StartsWith("data:application/json;utf8,"))
                {
                    var json = uri.Substring("data:application/json;utf8,".Length);
                    return JsonConvert.DeserializeObject<NFTMetadata>(json);
                }
                else if (uri.StartsWith("data:text/plain;charset=utf-8,"))
                {
                    var text = uri.Substring("data:text/plain;charset=utf-8,".Length);
                    return JsonConvert.DeserializeObject<NFTMetadata>(text);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting NFT metadata: {ex.Message}");
                return null; 
            }
        }
    }
}

using Nethereum.Web3;
using Newtonsoft.Json;
using NFT;
using System;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NFTValuations.Services
{
    public class NFTMetadataExtractor
    {
        private const string infuraEndpoint = "https://mainnet.infura.io/v3/b3ec6029213a49d4b793ee85d19e1c94";
        private static readonly HttpClient httpClient = new HttpClient();

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

                switch (uri)
                {
                    case var _ when uri.StartsWith("ipfs://"):
                        uri = "https://gateway.pinata.cloud/ipfs/" + uri.Substring(7);
                        break;
                    case var _ when uri.StartsWith("https://ipfs.io/ipfs/"):
                        uri = uri.Replace("https://ipfs.io/ipfs/", "https://gateway.pinata.cloud/ipfs/");
                        break;
                    case var _ when uri.StartsWith("data:application/json;base64,"):
                        var encodedJson = uri.Substring("data:application/json;base64,".Length);
                        var jsonBytes = Convert.FromBase64String(encodedJson);
                        var decodedJson = Encoding.UTF8.GetString(jsonBytes);
                        return JsonConvert.DeserializeObject<NFTMetadata>(decodedJson);
                    case var _ when uri.StartsWith("https://api.coolcatsnft.com/"):
                        uri = "https://api.coolcatsnft.com/cat/" + uri.Substring("https://api.coolcatsnft.com/cat/".Length);
                        break;
                    case var _ when uri.StartsWith("https://metadata.3landersnft.com/api/migration/"):
                        uri += ".json";
                        break;
                    case var _ when uri.StartsWith("https://cryptocubes.io/api/v1/ipfs/"):
                        uri = uri.Replace("https://cryptocubes.io/api/v1/ipfs/", "https://gateway.pinata.cloud/ipfs/");
                        break;
                    case var _ when uri.StartsWith("https://arweave.net/"):
                        uri = uri.Replace("https://arweave.net/", "https://gateway.pinata.cloud/ipfs/");
                        break;
                    case var _ when uri.StartsWith("https://token.qql.art/"):
                        uri += "/metadata.json";
                        break;
                    case var _ when uri.StartsWith("https://meebits.larvalabs.com/meebit/"):
                        uri = "https://meebits.larvalabs.com/metadata/" + uri.Substring("https://meebits.larvalabs.com/meebit/".Length);
                        break;
                    case var _ when uri.StartsWith("https://api.artblocks.io/token/"):
                        uri = "https://api.artblocks.io/generated/token/" + uri.Substring("https://api.artblocks.io/token/".Length) + "/index.json";
                        break;
                    case var _ when uri.StartsWith("data:application/json;utf8,"):
                        var json = uri.Substring("data:application/json;utf8,".Length);
                        return JsonConvert.DeserializeObject<NFTMetadata>(json);
                    case var _ when uri.StartsWith("data:text/plain;charset=utf-8,"):
                        var text = uri.Substring("data:text/plain;charset=utf-8,".Length);
                        return JsonConvert.DeserializeObject<NFTMetadata>(text);
                    default:
                        throw new NotSupportedException("Unsupported URI scheme");
                }

                var metadataJson = await httpClient.GetStringAsync(uri);
                var metadata = JsonConvert.DeserializeObject<NFTMetadata>(metadataJson);
                return metadata;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting NFT metadata: {ex.Message}");
                return null;
            }
        }
    }
}
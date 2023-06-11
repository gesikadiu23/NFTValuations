using Microsoft.Extensions.Caching.Memory;
using Nethereum.Web3;
using Newtonsoft.Json;
using NFT.Data.DTOs;
using System;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NFTValuations.Services
{
    public class NFTMetadataExtractor
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public NFTMetadataExtractor(IMemoryCache cache)
        {
            _httpClient = new HttpClient();
            _cache = cache;
        }


        // The endpoint URL for the Infura API
        private const string infuraEndpoint = "https://mainnet.infura.io/v3/b3ec6029213a49d4b793ee85d19e1c94";
        private const string unsupportedUrlsFilePath = @"";

        // Extracts the metadata for the given NFT.
        public async Task<NFTMetadata> ExtractNFTMetadata(string contractAddress, BigInteger tokenIndex)
        {

            string uri = String.Empty;
            try
            {
                if (_cache.TryGetValue(contractAddress + tokenIndex.ToString(), out NFTMetadata metadata))
                {
                    return metadata;
                }

                // Get the ABI (Application Binary Interface) for the NFTMetadata contract
                var ABI = NFTMetadata.ABI;

                // Create an instance of the Web3 class using the Infura endpoint URL
                var web3 = new Web3(infuraEndpoint);

                // Get the contract instance using the ABI and contract address
                var contract = web3.Eth.GetContract(ABI, contractAddress);

                // Get the tokenURI function from the contract
                var tokenUriFunction = contract.GetFunction("tokenURI");

                // Call the tokenURI function to get the URI of the token's metadata
                uri = await tokenUriFunction.CallAsync<string>(tokenIndex);

                // Process different URI schemes and convert them to a standard format

                switch (uri)
                {
                    // If the URI starts with "ipfs://", replace it with the Pinata IPFS gateway URL
                    case var _ when uri.StartsWith("ipfs://"):
                        uri = "https://gateway.pinata.cloud/ipfs/" + uri.Substring(7);
                        break;

                    // If the URI starts with "https://ipfs.io/ipfs/", replace it with the Pinata IPFS gateway URL
                    case var _ when uri.StartsWith("https://ipfs.io/ipfs/"):
                        uri = uri.Replace("https://ipfs.io/ipfs/", "https://gateway.pinata.cloud/ipfs/");
                        break;

                    // If the URI starts with "data:application/json;base64,", decode the JSON data and return the deserialized object
                    case var _ when uri.StartsWith("data:application/json;base64,"):
                        var encodedJson = uri.Substring("data:application/json;base64,".Length);
                        var jsonBytes = Convert.FromBase64String(encodedJson);
                        var decodedJson = Encoding.UTF8.GetString(jsonBytes);
                        return JsonConvert.DeserializeObject<NFTMetadata>(decodedJson);

                    // If the URI starts with "https://api.coolcatsnft.com/", append "cat/" to the URI
                    case var _ when uri.StartsWith("https://api.coolcatsnft.com/"):
                        uri = "https://api.coolcatsnft.com/cat/" + uri.Substring("https://api.coolcatsnft.com/cat/".Length);
                        break;

                    // If the URI starts with "https://metadata.3landersnft.com/api/migration/", append ".json" to the URI
                    case var _ when uri.StartsWith("https://metadata.3landersnft.com/api/migration/"):
                        uri += ".json";
                        break;

                    // If the URI starts with "https://cryptocubes.io/api/v1/ipfs/", replace it with the Pinata IPFS gateway URL
                    case var _ when uri.StartsWith("https://cryptocubes.io/api/v1/ipfs/"):
                        uri = uri.Replace("https://cryptocubes.io/api/v1/ipfs/", "https://gateway.pinata.cloud/ipfs/");
                        break;

                    // If the URI starts with "https://arweave.net/", replace it with the Pinata IPFS gateway URL
                    case var _ when uri.StartsWith("https://arweave.net/"):
                        uri = uri.Replace("https://arweave.net/", "https://gateway.pinata.cloud/ipfs/");
                        break;

                    // If the URI starts with "https://token.qql.art/", append "/metadata.json" to the URI
                    case var _ when uri.StartsWith("https://token.qql.art/"):
                        uri += "/metadata.json";
                        break;

                    // If the URI starts with "https://meebits.larvalabs.com/meebit/", replace it with "https://meebits.larvalabs.com/metadata/"
                    case var _ when uri.StartsWith("https://meebits.larvalabs.com/meebit/"):
                        uri = "https://meebits.larvalabs.com/metadata/" + uri.Substring("https://meebits.larvalabs.com/meebit/".Length);
                        break;

                    // If the URI starts with "https://api.artblocks.io/token/", replace it with "https://api.artblocks.io/generated/token/{id}/index.json"
                    case var _ when uri.StartsWith("https://api.artblocks.io/token/"):
                        uri = "https://api.artblocks.io/generated/token/" + uri.Substring("https://api.artblocks.io/token/".Length) + "/index.json";
                        break;

                    // If the URI starts with "data:application/json;utf8,", remove the prefix and return the deserialized object
                    case var _ when uri.StartsWith("data:application/json;utf8,"):
                        var json = uri.Substring("data:application/json;utf8,".Length);
                        return JsonConvert.DeserializeObject<NFTMetadata>(json);

                    // If the URI starts with "data:text/plain;charset=utf-8,", remove the prefix and return the deserialized object
                    case var _ when uri.StartsWith("data:text/plain;charset=utf-8,"):
                        var text = uri.Substring("data:text/plain;charset=utf-8,".Length);
                        return JsonConvert.DeserializeObject<NFTMetadata>(text);

                    // If none of the above cases match, throw an exception indicating unsupported URI scheme
                    default:
                        throw new NotSupportedException("Unsupported URI scheme");
                }

                // Fetch the metadata JSON from the resolved URI
                var metadataJson = await _httpClient.GetStringAsync(uri);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));

                _cache.Set(contractAddress + tokenIndex.ToString(), metadata);

                // Deserialize the metadata JSON into an NFTMetadata object
                metadata = JsonConvert.DeserializeObject<NFTMetadata>(metadataJson);

                // Return the extracted metadata
                return metadata;
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the extraction process and return null
                // Write the unsupported URL to a text file
                WriteUnsupportedUrlToFile(contractAddress, tokenIndex, uri);
                Console.WriteLine($"Error extracting NFT metadata: {ex.Message}. Contract Address: {contractAddress}, Token Index: {tokenIndex}");
                return null;
            }
        }

        #region private methods

        private void WriteUnsupportedUrlToFile(string contractAddress, BigInteger tokenIndex, string uri)
        {
            // Create a new line with the unsupported URL
            string line = $"Contract Address : {contractAddress}, Token Index: {tokenIndex}, URL: {uri}";

            try
            {
                // Append the line to the text file
                File.AppendAllText(unsupportedUrlsFilePath, line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during file writing
                Console.WriteLine($"Error writing unsupported URL to file: {ex.Message}");
            }
        }

        #endregion
    }
}
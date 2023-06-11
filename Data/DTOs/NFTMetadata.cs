using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace NFT
{
    public class NFTMetadata
    {
        // The name of the NFT
        [JsonProperty("Name")]
        public string Name { get; set; }

        // The description of the NFT
        [JsonProperty("Description")]
        public string Description { get; set; }

        // The external URL associated with the NFT
        [JsonProperty("ExternalUrl")]
        public string ExternalUrl { get; set; }

        // The media (image or video) associated with the NFT
        [JsonProperty("image")]
        public string Media { get; set; }

        // The list of attributes or properties of the NFT
        [JsonProperty("attributes")]
        public List<Attribute>? Properties { get; set; }

        // The ABI (Application Binary Interface) of the NFT contract
        public static string ABI { get; set; } = @"[
        {
            ""constant"": true,
            ""inputs"": [
                {
                    ""name"": ""_tokenId"",
                    ""type"": ""uint256""
                }
            ],
            ""name"": ""tokenURI"",
            ""outputs"": [
                {
                    ""name"": """",
                    ""type"": ""string""
                }
            ],
            ""payable"": false,
            ""stateMutability"": ""view"",
            ""type"": ""function""
        }
    ]";
    }
}

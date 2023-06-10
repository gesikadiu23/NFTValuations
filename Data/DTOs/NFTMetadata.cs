using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using NFT;

namespace NFT
{
    public class NFTMetadata
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("ExternalUrl")]
        public string ExternalUrl { get; set; }

        [JsonProperty("image")]
        public string Media { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute>? Properties { get; set; }

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

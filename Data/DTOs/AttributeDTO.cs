using Newtonsoft.Json;

namespace NFT.Data.DTOs
{
    public class AttributeDTO
    {
        // The category or trait type of the attribute
        [JsonProperty("trait_type")]
        public string Category { get; set; }

        // The value or property associated with the attribute
        [JsonProperty("value")]
        public string Property { get; set; }
    }
}
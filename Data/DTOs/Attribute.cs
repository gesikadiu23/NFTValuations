using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NFT
{
    public class Attribute
    {
        // The category or trait type of the attribute
        [JsonProperty("trait_type")]
        public string Category { get; set; }

        // The value or property associated with the attribute
        [JsonProperty("value")]
        public string Property { get; set; }
    }
}
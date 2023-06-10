using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NFT.Models
{
    public class Attribute
    {
        [JsonProperty("trait_type")]
        public string Category { get; set; }

        [JsonProperty("value")]
        public string Property { get; set; }
    }
}

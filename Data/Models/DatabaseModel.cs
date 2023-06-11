using System;
using System.Collections.Generic;
using System.Text;

namespace NFTValuations.Models.Data
{
    public class DatabaseModel
    {
        public int Id { get; set; }
        // The name of the NFT
        public string Name { get; set; }
        // The description of the NFT
        public string Description { get; set; }
        // The external URL associated with the NFT
        public string ExternalUrl { get; set; }
        // The media (image or video) associated with the NFT
        public string Media { get; set; }
        // The list of attributes or properties of the NFT
        public List<PropertyModel> Properties { get; set; }
    }

}

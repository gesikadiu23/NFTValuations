using System;
using System.Collections.Generic;
using System.Text;

namespace NFTValuations.Models.Data
{
    public class PropertyModel
    {
        public int Id { get; set; }
        // The category or trait type of the attribute
        public string Category { get; set; }
        // The value or property associated with the attribute
        public string Property { get; set; }
    }
}

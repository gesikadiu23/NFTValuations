namespace NFTValuations.Data.Models
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

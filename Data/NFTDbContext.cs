using Microsoft.EntityFrameworkCore;
using NFTValuations.Data.Models;

namespace NFTValuations.Data
{
    public class NFTDbContext : DbContext
    {
        // DbSet representing the collection of DatabaseModel objects in the database
        public DbSet<DatabaseModel> DatabaseModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your database connection here
            optionsBuilder.UseSqlServer(@"Your_Connection_String");
        }
    }
}

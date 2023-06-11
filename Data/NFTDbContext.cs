using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NFTValuations.Models.Data;

namespace NFTValuations.Data
{
    public class NFTDbContext : DbContext
    {
        // DbSet representing the collection of DatabaseModel objects in the database
        public DbSet<DatabaseModel> DatabaseModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your database connection here
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=N;Integrated Security=true;");
        }
    }
}

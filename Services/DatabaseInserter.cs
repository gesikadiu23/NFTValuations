using NFTValuations.Data;
using NFTValuations.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NFTValuations
{
    public class DatabaseInserter
    {
        public async Task InsertDatabaseModels(List<DatabaseModel> databaseModels)
        {
            using (var dbContext = new NFTDbContext())
            {
                dbContext.DatabaseModels.AddRange(databaseModels);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}

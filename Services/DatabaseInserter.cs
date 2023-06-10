using NFTValuations.Data;
using NFTValuations.Models.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFTValuations
{
    public class DatabaseInserter
    {
        public async Task InsertDatabaseModels(List<DatabaseModel> databaseModels)
        {
            const int batchSize = 5; 

            using (var dbContext = new NFTDbContext())
            {
                for (int i = 0; i < databaseModels.Count; i += batchSize)
                {
                    var batch = databaseModels.Skip(i).Take(batchSize);
                    dbContext.DatabaseModels.AddRange(batch);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}

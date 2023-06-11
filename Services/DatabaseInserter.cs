using NFTValuations.Data;
using NFTValuations.Models.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFTValuations
{
    public class DatabaseInserter
    {
        // Inserts a list of DatabaseModel objects into the database.
        public async Task InsertDatabaseModels(List<DatabaseModel> databaseModels)
        {
            const int batchSize = 5; // The number of DatabaseModels to insert in each batch

            using (var dbContext = new NFTDbContext())
            {
                for (int i = 0; i < databaseModels.Count; i += batchSize)
                {
                    // Take a batch of DatabaseModels based on the batch size
                    var batch = databaseModels.Skip(i).Take(batchSize);

                    // Add the batch of DatabaseModels to the DbContext
                    dbContext.DatabaseModels.AddRange(batch);

                    // Save changes asynchronously to persist the batch in the database
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}

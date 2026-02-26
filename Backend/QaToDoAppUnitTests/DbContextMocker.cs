using Microsoft.EntityFrameworkCore;
using QaToDoApp.Data;

namespace QaToDoAppUnitTests
{
    internal static class DbContextMocker
    {
        public static ToDoDbContext GetToDoAppDbContext(string dbName)
        {            
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            
            var dbContext = new ToDoDbContext(options);

            dbContext.Seed();

            return dbContext;
        }
    }
}

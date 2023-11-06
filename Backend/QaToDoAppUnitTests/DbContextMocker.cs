using Microsoft.EntityFrameworkCore;
using QaToDoApp.Models;

namespace QaToDoAppUnitTests
{
    internal static class DbContextMocker
    {
        public static ToDoContext GetToDoAppDbContext(string dbName)
        {            
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            
            var dbContext = new ToDoContext(options);

            dbContext.Seed();

            return dbContext;
        }
    }
}

using Newtonsoft.Json;
using QaToDoApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using QaToDoApp.Data;

namespace QaToDoAppIntegrationTests
{
    public static class DbInit
    {       
        private static void InitializeDbForTests(ToDoDbContext db)
        {
            db.TodoItems.AddRange(GetSeedingToDoItems());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ToDoDbContext db)
        {
            db.TodoItems.RemoveRange(db.TodoItems);
            InitializeDbForTests(db);
        }

        private static IEnumerable<ToDoItem> GetSeedingToDoItems()
        {
            return new List<ToDoItem>()
            {
                new ToDoItem(){ Id = 1, Text = "ToDo item 1" },
                new ToDoItem(){ Id = 2, Text = "ToDo item 2" },
                new ToDoItem(){ Id = 3, Text = "ToDo item 3" }
            };
        }

        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }
}

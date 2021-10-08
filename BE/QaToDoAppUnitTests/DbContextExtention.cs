using QaToDoApp.Models;

namespace QaToDoAppUnitTests
{
    public static class DbContextExtention
    {
        public static void Seed(this ToDoContext dbContext)
        {
            dbContext.TodoItems.Add(new ToDoItem { Id = 1, Text = "ToDo 1" });
            dbContext.TodoItems.Add(new ToDoItem { Id = 2, Text = "ToDo 2" });
            dbContext.TodoItems.Add(new ToDoItem { Id = 3, Text = "ToDo 3" });
            dbContext.SaveChanges();
        }
    }
}

using QaToDoApp.Models;

namespace QaToDoAppUnitTests
{
    public static class DbContextExtension
    {
        public static void Seed(this ToDoContext dbContext)
        {
            dbContext.TodoItems.Add(new ToDoItem { Id = 1, Text = "ToDo item 1" });
            dbContext.TodoItems.Add(new ToDoItem { Id = 2, Text = "ToDo item 2" });
            dbContext.TodoItems.Add(new ToDoItem { Id = 3, Text = "ToDo item 3" });
            dbContext.SaveChanges();
        }
    }
}

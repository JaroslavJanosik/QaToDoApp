using System;
using QaToDoApp.Data;
using QaToDoApp.Models;

namespace QaToDoAppUnitTests
{
    public static class DbContextExtension
    {
        public static void Seed(this ToDoDbContext dbDbContext)
        {
            dbDbContext.TodoItems.Add(new ToDoItem { Text = "ToDo item 1" ,CreatedDate = DateTime.Now });
            dbDbContext.TodoItems.Add(new ToDoItem { Text = "ToDo item 2" ,CreatedDate = DateTime.Now });
            dbDbContext.TodoItems.Add(new ToDoItem { Text = "ToDo item 3" ,CreatedDate = DateTime.Now });
            dbDbContext.SaveChanges();
            dbDbContext.ChangeTracker.Clear();
        }
    }
}

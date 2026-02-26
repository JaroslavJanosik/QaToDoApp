using System;
using Microsoft.EntityFrameworkCore;
using QaToDoApp.Models;

namespace QaToDoApp.Data
{
    public class ToDoDbContext : DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
            : base(options)
        {
        }

        public DbSet<ToDoItem> TodoItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ToDoItem>().HasData(
                new ToDoItem
                {
                    Id = 1,
                    Text = "ToDoItem 1",
                    Completed = false,
                    CreatedDate = DateTimeOffset.Now
                },
              new ToDoItem
              {
                  Id = 2,
                  Text = "ToDoItem 2",
                  Completed = false,
                  CreatedDate = DateTimeOffset.Now
              },
              new ToDoItem
              {
                  Id = 3,
                  Text = "ToDoItem 3",
                  Completed = true,
                  CreatedDate = DateTimeOffset.Now
              },
              new ToDoItem
              {
                  Id = 4,
                  Text = "ToDoItem 4",
                  Completed = false,
                  CreatedDate = DateTimeOffset.Now
              },
              new ToDoItem
              {
                  Id = 5,
                  Text = "ToDoItem 5",
                  Completed = true,
                  CreatedDate = DateTimeOffset.Now
              });
        }
    }
}
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using QaToDoApp.Controllers;
using QaToDoApp.Models;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace QaToDoAppUnitTests
{
    public class ToDoAppUnitTests
    {
        [Fact]      
        public async Task TestGetTodoItems()
        {
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestGetTodoItems));
            ToDoItemsController toDoItemsController = new ToDoItemsController(dbContext);
           
            var getResponse = await toDoItemsController.GetTodoItems();           
            var value = getResponse.Value;

            dbContext.Dispose();
            
            value.Count().Should().Be(3);            
        }

        [Fact]
        public async Task TestGetTodoItem()
        {
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestGetTodoItem));
            ToDoItemsController toDoItemsController = new ToDoItemsController(dbContext);

            var getResponse = await toDoItemsController.GetToDoItem(1);
            var value = getResponse.Value;

            dbContext.Dispose();

            value.Id.Should().Be(1);           
        }

        [Fact]
        public async Task TestPostTodoItem()
        {
            var toDoItemText = "A new POST ToDo Item";
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestPostTodoItem));
            ToDoItemsController toDoItemsController = new ToDoItemsController(dbContext);
            var request = new ToDoForCreateDto
            {
                Text = $"{toDoItemText}"
            };

            var postResponse = await toDoItemsController.PostToDoItem(request);
            var result = postResponse.Result as CreatedAtActionResult;
            var toDoItem = result.Value as ToDoItem;
                        
            dbContext.Dispose();

            toDoItemText.Should().Be(toDoItem.Text);
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public async Task TestDeleteTodoItem()
        {
            var toDoItemText = "A new DELETE ToDo Item";
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestDeleteTodoItem));
            ToDoItemsController toDoItemsController = new ToDoItemsController(dbContext);
            var request = new ToDoForCreateDto
            {
                Text = $"{toDoItemText}"
            };

            var postResponse = await toDoItemsController.PostToDoItem(request);
            var value = postResponse.Value;
            var result = postResponse.Result as CreatedAtActionResult;
            var item = result.Value as ToDoItem;

            var deleteResponse = await toDoItemsController.DeleteToDoItem(item.Id);
            var deleteStatusCode = deleteResponse as StatusCodeResult;
            var getResponse = await toDoItemsController.GetToDoItem(item.Id);
            var getStatusCode = getResponse.Result as StatusCodeResult;

            dbContext.Dispose();
            getStatusCode.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            deleteStatusCode.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}

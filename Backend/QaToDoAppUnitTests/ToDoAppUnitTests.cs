using System.Diagnostics;
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
            var toDoItemsController = new ToDoItemsController(dbContext);
           
            var getResponse = await toDoItemsController.GetTodoItems();           
            var value = getResponse.Value;

            await dbContext.DisposeAsync();

            var toDoItems = value.ToList();
            toDoItems.Count.Should().Be(3);
            toDoItems[0].Id.Should().Be(1);
            toDoItems[0].Text.Should().Be("ToDo item 1");
            toDoItems[0].Completed.Should().Be(false);
            toDoItems[1].Id.Should().Be(2);
            toDoItems[1].Text.Should().Be("ToDo item 2");
            toDoItems[1].Completed.Should().Be(false);
            toDoItems[2].Id.Should().Be(3);
            toDoItems[2].Text.Should().Be("ToDo item 3");
            toDoItems[2].Completed.Should().Be(false);
        }

        [Fact]
        public async Task TestGetTodoItem()
        {
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestGetTodoItem));
            var toDoItemsController = new ToDoItemsController(dbContext);

            var getResponse = await toDoItemsController.GetToDoItem(1);
            var toDoItem = getResponse.Value;

            await dbContext.DisposeAsync();

            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be("ToDo item 1");
            toDoItem.Completed.Should().Be(false);
        }

        [Fact]
        public async Task TestPostTodoItem()
        {
            const string toDoItemText = "A new POST ToDo Item";
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestPostTodoItem));
            var toDoItemsController = new ToDoItemsController(dbContext);
            var request = new ToDoForCreateDto
            {
                Text = $"{toDoItemText}"
            };

            var postResponse = await toDoItemsController.PostToDoItem(request);
            var result = postResponse.Result as CreatedAtActionResult;
            Debug.Assert(result != null, nameof(result) + " != null");
            var toDoItem = result.Value as ToDoItem;
                        
            await dbContext.DisposeAsync();

            Debug.Assert(toDoItem != null, nameof(toDoItem) + " != null");
            toDoItemText.Should().Be(toDoItem.Text);
            toDoItem.Completed.Should().Be(false);
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task TestPutTodoItem()
        {
            const string toDoItemText = "A new PUT ToDo Item";
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestPutTodoItem));
            var toDoItemsController = new ToDoItemsController(dbContext);
            var request = new ToDoForUpdateDto()
            {
                Text = $"{toDoItemText}",
                Completed = true
            };

            var putResponse = await toDoItemsController.PutToDoItem(1, request);
            var result = putResponse as CreatedAtActionResult;
            Debug.Assert(result != null, nameof(result) + " != null");
            var toDoItem = result.Value as ToDoItem;
                        
            await dbContext.DisposeAsync();

            Debug.Assert(toDoItem != null, nameof(toDoItem) + " != null");
            toDoItemText.Should().Be(toDoItem.Text);
            toDoItem.Completed.Should().Be(true);
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public async Task TestDeleteTodoItem()
        {
            const string toDoItemText = "A new DELETE ToDo Item";
            var dbContext = DbContextMocker.GetToDoAppDbContext(nameof(TestDeleteTodoItem));
            var toDoItemsController = new ToDoItemsController(dbContext);
            var request = new ToDoForCreateDto
            {
                Text = $"{toDoItemText}"
            };

            var postResponse = await toDoItemsController.PostToDoItem(request);
            var value = postResponse.Value;
            var result = postResponse.Result as CreatedAtActionResult;
            Debug.Assert(result != null, nameof(result) + " != null");
            var item = result.Value as ToDoItem;

            Debug.Assert(item != null, nameof(item) + " != null");
            var deleteResponse = await toDoItemsController.DeleteToDoItem(item.Id);
            var deleteStatusCode = deleteResponse as StatusCodeResult;
            var getResponse = await toDoItemsController.GetToDoItem(item.Id);
            var getStatusCode = getResponse.Result as StatusCodeResult;

            await dbContext.DisposeAsync();
            Debug.Assert(getStatusCode != null, nameof(getStatusCode) + " != null");
            getStatusCode.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            Debug.Assert(deleteStatusCode != null, nameof(deleteStatusCode) + " != null");
            deleteStatusCode.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}

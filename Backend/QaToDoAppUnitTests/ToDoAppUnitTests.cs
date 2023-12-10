using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using QaToDoApp.Controllers;
using QaToDoApp.Models;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using QaToDoApp;
using QaToDoApp.Models.Dto;
using QaToDoApp.Repository;
using Xunit;

namespace QaToDoAppUnitTests
{
    public class ToDoAppUnitTests
    {
        private readonly ToDoItemsController _toDoItemsController;

        public ToDoAppUnitTests()
        {
            _toDoItemsController = CreateController(nameof(ToDoAppUnitTests));
        }

        private ToDoItemsController CreateController(string testName)
        {
            var dbContext = DbContextMocker.GetToDoAppDbContext(testName);
            var repository = new ToDoItemRepository(dbContext);
            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingConfig()));
            var mapper = mockMapper.CreateMapper();
            return new ToDoItemsController(repository, mapper);
        }

        [Fact]
        public async Task TestGetTodoItemsAsync()
        {
            // Arrange
            var toDoItemsController = CreateController(nameof(TestGetTodoItemsAsync));

            // Act
            var getResponse = await toDoItemsController.GetTodoItems();
            var result = getResponse.Result as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItems = apiResponse?.Result as List<ToDoItemDto>;

            // Assert
            toDoItems.Should().NotBeNull().And.HaveCount(3);
            toDoItems.Should().OnlyContain(item => !item.Completed);
            toDoItems.Should().BeEquivalentTo(Enumerable.Range(1, 3).Select(i => new ToDoItemDto
                { Id = i, Text = $"ToDo item {i}", Completed = false }));
        }

        [Fact]
        public async Task TestGetTodoItemAsync()
        {
            // Arrange
            var toDoItemsController = CreateController(nameof(TestGetTodoItemAsync));

            // Act
            var getResponse = await toDoItemsController.GetToDoItem(1);
            var result = getResponse.Result as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            // Assert
            toDoItem.Should().NotBeNull();
            toDoItem!.Id.Should().Be(1);
            toDoItem.Text.Should().Be("ToDo item 1");
            toDoItem.Completed.Should().BeFalse();
        }

        [Fact]
        public async Task TestPostTodoItemAsync()
        {
            // Arrange
            var toDoItemsController = CreateController(nameof(TestPostTodoItemAsync));
            const string toDoItemText = "A new POST ToDo Item";
            var request = new ToDoForCreateDto { Text = toDoItemText };

            // Act
            var postResponse = await toDoItemsController.PostToDoItem(request);
            var result = postResponse.Result as CreatedAtRouteResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            // Assert
            toDoItem.Should().NotBeNull();
            toDoItemText.Should().Be(toDoItem!.Text);
            toDoItem.Completed.Should().BeFalse();
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public async Task TestPutTodoItemAsync()
        {
            // Arrange
            var toDoItemsController = CreateController(nameof(TestPutTodoItemAsync));
            const string toDoItemText = "A new PUT ToDo Item";
            var request = new ToDoForUpdateDto { Id = 1, Text = toDoItemText, Completed = true };

            // Act
            var putResponse = await toDoItemsController.PutToDoItem(1, request);
            var result = putResponse.Result as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            // Assert
            toDoItem.Should().NotBeNull();
            toDoItemText.Should().Be(toDoItem!.Text);
            toDoItem.Completed.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task TestPatchTodoItemAsync()
        {
            // Arrange
            var toDoItemsController = CreateController(nameof(TestPatchTodoItemAsync));
            const string toDoItemText = "A new PATCH ToDo Item";
            var request = new JsonPatchDocument<ToDoForUpdateDto>();
            request.Replace(t => t.Text, toDoItemText);
            request.Replace(t => t.Completed, true);

            // Act
            var patchResponse = await toDoItemsController.PatchToDoItem(1, request);
            var result = patchResponse as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            // Assert
            toDoItem.Should().NotBeNull();
            toDoItemText.Should().Be(toDoItem!.Text);
            toDoItem.Completed.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task TestDeleteTodoItemAsync()
        {
            // Arrange
            var toDoItemsController = CreateController(nameof(TestDeleteTodoItemAsync));
            const string toDoItemText = "A new DELETE ToDo Item";
            var request = new ToDoForCreateDto { Text = toDoItemText };

            // Act
            var postResponse = await toDoItemsController.PostToDoItem(request);
            var result = postResponse.Result as CreatedAtRouteResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;
            var deleteResponse = await toDoItemsController.DeleteToDoItem(toDoItem!.Id);

            // Assert
            deleteResponse.Result.Should().BeOfType<NoContentResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            var getResponse = await toDoItemsController.GetToDoItem(toDoItem.Id);
            getResponse.Result.Should().BeOfType<NotFoundObjectResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
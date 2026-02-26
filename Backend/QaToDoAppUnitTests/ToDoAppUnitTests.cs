using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using QaToDoApp;
using QaToDoApp.Controllers;
using QaToDoApp.Models;
using QaToDoApp.Models.Dto;
using QaToDoApp.Repository;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace QaToDoAppUnitTests
{
    public class ToDoAppUnitTests
    {
        private static ToDoItemsController CreateController(string testName)
        {
            var dbContext = DbContextMocker.GetToDoAppDbContext(testName);
            var repository = new ToDoItemRepository(dbContext);

            var loggerFactory = LoggerFactory.Create(_ => { });

            var mapperConfig = new MapperConfiguration(
                cfg => { cfg.AddProfile(new MappingConfig()); },
                loggerFactory
            );

            var mapper = mapperConfig.CreateMapper();

            return new ToDoItemsController(repository, mapper);
        }

        [Fact]
        public async Task TestGetTodoItemsAsync()
        {
            var toDoItemsController = CreateController(nameof(TestGetTodoItemsAsync));

            var getResponse = await toDoItemsController.GetTodoItems();
            var result = getResponse.Result as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItems = apiResponse?.Result as List<ToDoItemDto>;

            toDoItems.Should().NotBeNull().And.HaveCount(3);
            toDoItems.Should().OnlyContain(item => !item.Completed);
            toDoItems.Should().BeEquivalentTo(Enumerable.Range(1, 3).Select(i => new ToDoItemDto
            {
                Id = i,
                Text = $"ToDo item {i}",
                Completed = false
            }));
        }

        [Fact]
        public async Task TestGetTodoItemAsync()
        {
            var toDoItemsController = CreateController(nameof(TestGetTodoItemAsync));

            var getResponse = await toDoItemsController.GetToDoItem(1);
            var result = getResponse.Result as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            toDoItem.Should().NotBeNull();
            toDoItem!.Id.Should().Be(1);
            toDoItem.Text.Should().Be("ToDo item 1");
            toDoItem.Completed.Should().BeFalse();
        }

        [Fact]
        public async Task TestPostTodoItemAsync()
        {
            var toDoItemsController = CreateController(nameof(TestPostTodoItemAsync));
            const string toDoItemText = "A new POST ToDo Item";
            var request = new ToDoForCreateDto { Text = toDoItemText };

            var postResponse = await toDoItemsController.PostToDoItem(request);
            var result = postResponse.Result as CreatedAtRouteResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            toDoItem.Should().NotBeNull();
            toDoItemText.Should().Be(toDoItem!.Text);
            toDoItem.Completed.Should().BeFalse();
            result!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public async Task TestPutTodoItemAsync()
        {
            var toDoItemsController = CreateController(nameof(TestPutTodoItemAsync));
            const string toDoItemText = "A new PUT ToDo Item";
            var request = new ToDoForUpdateDto
            {
                Id = 1,
                Text = toDoItemText,
                Completed = true
            };

            var putResponse = await toDoItemsController.PutToDoItem(1, request);
            var result = putResponse.Result as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            toDoItem.Should().NotBeNull();
            toDoItemText.Should().Be(toDoItem!.Text);
            toDoItem.Completed.Should().BeTrue();
            result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task TestPatchTodoItemAsync()
        {
            var toDoItemsController = CreateController(nameof(TestPatchTodoItemAsync));
            const string toDoItemText = "A new PATCH ToDo Item";

            var request = new JsonPatchDocument<ToDoForUpdateDto>();
            request.Replace(t => t.Text, toDoItemText);
            request.Replace(t => t.Completed, true);

            var patchResponse = await toDoItemsController.PatchToDoItem(1, request);
            var result = patchResponse as OkObjectResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            toDoItem.Should().NotBeNull();
            toDoItemText.Should().Be(toDoItem!.Text);
            toDoItem.Completed.Should().BeTrue();
            result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task TestDeleteTodoItemAsync()
        {
            var toDoItemsController = CreateController(nameof(TestDeleteTodoItemAsync));
            const string toDoItemText = "A new DELETE ToDo Item";
            var request = new ToDoForCreateDto { Text = toDoItemText };

            var postResponse = await toDoItemsController.PostToDoItem(request);
            var result = postResponse.Result as CreatedAtRouteResult;
            var apiResponse = result?.Value as ApiResponse;
            var toDoItem = apiResponse?.Result as ToDoItemDto;

            var deleteResponse = await toDoItemsController.DeleteToDoItem(toDoItem!.Id);

            deleteResponse.Result.Should().BeOfType<NoContentResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var getResponse = await toDoItemsController.GetToDoItem(toDoItem.Id);
            getResponse.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using QaToDoApp;
using QaToDoApp.Models;
using QaToDoApp.Models.Dto;
using Xunit;

namespace QaToDoAppIntegrationTests
{
    public class ToDoAppTests : IAsyncLifetime
    {
        private CustomWebApplicationFactory<Startup> _factory = null!;
        private HttpClient _client = null!;

        public Task InitializeAsync()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            _factory.Dispose();
            return Task.CompletedTask;
        }

        private static async Task<T> DeserializeApiResponse<T>(HttpResponseMessage response)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            responseJson.Should().NotBeNullOrWhiteSpace("API response should contain JSON content");

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseJson);
            apiResponse.Should().NotBeNull("response should deserialize to ApiResponse");
            apiResponse!.Result.Should().NotBeNull("ApiResponse.Result should not be null");

            var resultJson = apiResponse.Result!.ToString();
            resultJson.Should().NotBeNullOrWhiteSpace("ApiResponse.Result should contain JSON");

            var result = JsonConvert.DeserializeObject<T>(resultJson!);
            result.Should().NotBeNull("Result JSON should deserialize to the expected type");

            return result!;
        }

        [Fact]
        public async Task GetAllTodoItemsTest()
        {
            var response = await _client.GetAsync("/api/ToDoItems");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var toDoItems = await DeserializeApiResponse<List<ToDoItemDto>>(response);

            toDoItems.Should().HaveCount(3);
            toDoItems.Should().OnlyContain(item => !item.Completed);
            toDoItems.Should().BeEquivalentTo(new[]
            {
                new ToDoItemDto { Id = 1, Text = "ToDo item 1", Completed = false },
                new ToDoItemDto { Id = 2, Text = "ToDo item 2", Completed = false },
                new ToDoItemDto { Id = 3, Text = "ToDo item 3", Completed = false }
            });
        }

        [Fact]
        public async Task GetTodoItemTest()
        {
            var response = await _client.GetAsync("/api/ToDoItems/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be("ToDo item 1");
            toDoItem.Completed.Should().BeFalse();
        }

        [Fact]
        public async Task PostTodoItemTest()
        {
            const string toDoItemText = "A new POST ToDo item";
            var request = new
            {
                Text = toDoItemText
            };

            var getAllBeforeResponse = await _client.GetAsync("/api/ToDoItems");
            getAllBeforeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var itemsBefore = await DeserializeApiResponse<List<ToDoItemDto>>(getAllBeforeResponse);
            itemsBefore.Should().HaveCount(3);

            var postResponse = await _client.PostAsync("/api/ToDoItems/", DbInit.GetStringContent(request));
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdItem = await DeserializeApiResponse<ToDoItemDto>(postResponse);

            var getAllAfterResponse = await _client.GetAsync("/api/ToDoItems");
            getAllAfterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var itemsAfter = await DeserializeApiResponse<List<ToDoItemDto>>(getAllAfterResponse);

            createdItem.Id.Should().BeGreaterThan(0);
            createdItem.Text.Should().Be(toDoItemText);
            createdItem.Completed.Should().BeFalse();

            itemsAfter.Should().HaveCount(4);
            itemsAfter.Should().ContainSingle(item =>
                item.Id == createdItem.Id &&
                item.Text == toDoItemText &&
                !item.Completed);
        }

        [Fact]
        public async Task PutTodoItemTest()
        {
            const string toDoItemText = "A new PUT ToDo item";
            var request = new
            {
                Id = 1,
                Text = toDoItemText,
                Completed = true
            };

            var response = await _client.PutAsync("/api/ToDoItems/1", DbInit.GetStringContent(request));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be(toDoItemText);
            toDoItem.Completed.Should().BeTrue();
        }

        [Fact]
        public async Task PatchTodoItemTest()
        {
            const string toDoItemText = "A new PATCH ToDo Item";
            var patchDocument = new[]
            {
                new
                {
                    path = "/text",
                    op = "replace",
                    value = toDoItemText
                }
            };

            var response = await _client.PatchAsync("/api/ToDoItems/1", DbInit.GetStringContent(patchDocument));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be(toDoItemText);
            toDoItem.Completed.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteTodoItemTest()
        {
            var postRequest = new
            {
                Text = "A new DELETE ToDo item"
            };

            var postResponse = await _client.PostAsync("/api/ToDoItems/", DbInit.GetStringContent(postRequest));
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdItem = await DeserializeApiResponse<ToDoItemDto>(postResponse);

            var deleteResponse = await _client.DeleteAsync($"/api/ToDoItems/{createdItem.Id}");
            var getDeletedResponse = await _client.GetAsync($"/api/ToDoItems/{createdItem.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
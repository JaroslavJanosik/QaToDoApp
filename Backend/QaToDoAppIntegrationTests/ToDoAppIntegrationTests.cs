using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using QaToDoApp;
using QaToDoApp.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QaToDoApp.Models.Dto;
using Xunit;

namespace QaToDoAppIntegrationTests
{
    public class ToDoAppTests
    {
        private readonly HttpClient _client;

        public ToDoAppTests()
        {
            var factory = new CustomWebApplicationFactory<Startup>();
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private async Task<T> DeserializeApiResponse<T>(HttpResponseMessage response)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var apiResponseObj = JsonConvert.DeserializeObject<ApiResponse>(responseJson);
            var resultJson = apiResponseObj.Result.ToString();
            return JsonConvert.DeserializeObject<T>(resultJson!);
        }

        [Fact]
        public async Task GetAllTodoItemsTest()
        {
            // Arrange
            var response = await _client.GetAsync("/api/ToDoItems");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act
            var toDoItems = await DeserializeApiResponse<List<ToDoItemDto>>(response);

            // Assert
            toDoItems.Should().NotBeNull().And.HaveCount(3);
            toDoItems.Should().OnlyContain(item => !item.Completed);
            toDoItems.Should().BeEquivalentTo(Enumerable.Range(1, 3).Select(i => new ToDoItemDto
                { Id = i, Text = $"ToDo item {i}", Completed = false }));
        }

        [Fact]
        public async Task GetTodoItemTest()
        {
            // Arrange
            var response = await _client.GetAsync("/api/ToDoItems/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act
            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            // Assert
            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be("ToDo item 1");
            toDoItem.Completed.Should().Be(false);
        }

        [Fact]
        public async Task PostTodoItemTest()
        {
            // Arrange
            const string toDoItemText = "A new POST ToDo item";
            var request = new
            {
                Url = "/api/ToDoItems/",
                Body = new
                {
                    Text = toDoItemText,
                }
            };

            // Act
            var response = await _client.PostAsync(request.Url, DbInit.GetStringContent(request.Body));
            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            toDoItem.Id.Should().Be(4);
            toDoItem.Text.Should().Be(toDoItemText);
            toDoItem.Completed.Should().Be(false);
        }

        [Fact]
        public async Task PutTodoItemTest()
        {
            // Arrange
            const string toDoItemText = "A new PUT ToDo item";
            var request = new
            {
                Url = "/api/ToDoItems/1",
                Body = new
                {
                    Id = 1,
                    Text = toDoItemText,
                    Completed = true
                }
            };

            // Act
            var response = await _client.PutAsync(request.Url, DbInit.GetStringContent(request.Body));
            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be(toDoItemText);
            toDoItem.Completed.Should().Be(true);
        }

        [Fact]
        public async Task PatchTodoItemTest()
        {
            // Arrange
            const string toDoItemText = "A new PATCH ToDo Item";
            var request = new
            {
                Url = "/api/ToDoItems/1",
                Body = new[]
                {
                    new
                    {
                        Path = "/text",
                        Op = "replace",
                        Value = toDoItemText
                    }
                }
            };

            // Act
            var response = await _client.PatchAsync(request.Url, DbInit.GetStringContent(request.Body));
            var toDoItem = await DeserializeApiResponse<ToDoItemDto>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            toDoItem.Id.Should().Be(1);
            toDoItem.Text.Should().Be(toDoItemText);
            toDoItem.Completed.Should().Be(false);
        }

        [Fact]
        public async Task DeleteTodoItemTest()
        {
            // Arrange
            var postRequest = new
            {
                Url = "/api/ToDoItems/",
                Body = new
                {
                    Text = "A new DELETE ToDo item"
                }
            };

            var postResponse = await _client.PostAsync(postRequest.Url, DbInit.GetStringContent(postRequest.Body));
            var postToDoItem = await DeserializeApiResponse<ToDoItemDto>(postResponse);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/ToDoItems/{postToDoItem.Id}");
            var getResponse = await _client.GetAsync($"/api/ToDoItems/{postToDoItem.Id}");

            // Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

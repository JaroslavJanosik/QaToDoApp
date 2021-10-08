using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using QaToDoApp;
using QaToDoApp.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace QaToDoAppIntegrationTests
{
    public class ToDoAppTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ToDoAppTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAllTodoItemsTest()
        {
            var response = await _client.GetAsync("/api/ToDoItems");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetTodoItemTest()
        {
            var response = await _client.GetAsync("/api/ToDoItems/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);            
        }

        [Fact]
        public async Task PostTodoItemTest()
        {
            var request = new
            {
                Url = "/api/ToDoItems/",
                Body = new
                {
                    Text = "A new POST ToDo item"
                }
            };

            var postResponse = await _client.PostAsync(request.Url, Utilities.GetStringContent(request.Body));
            var jsonFromPostResponse = await postResponse.Content.ReadAsStringAsync();
            var singleResponse = JsonConvert.DeserializeObject<ToDoItem>(jsonFromPostResponse);

            var getResponse = await _client.GetAsync(string.Format("/api/ToDoItems/{0}", singleResponse.Id));

            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteTodoItemTest()
        {
            var postRequest = new
            {
                Url = "/api/ToDoItems/",
                Body = new
                {
                    Text = "A new DELETE ToDo item"
                }
            };

            var postResponse = await _client.PostAsync(postRequest.Url, Utilities.GetStringContent(postRequest.Body));
            var jsonFromPostResponse = await postResponse.Content.ReadAsStringAsync();
            var singleResponse = JsonConvert.DeserializeObject<ToDoItem>(jsonFromPostResponse);

            var deleteResponse = await _client.DeleteAsync(string.Format("/api/ToDoItems/{0}", singleResponse.Id));
           
            var getResponse = await _client.GetAsync(string.Format("/api/ToDoItems/{0}", singleResponse.Id));

            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);           
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
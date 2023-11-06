using System.Collections.Generic;
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

        [Fact]
        public async Task GetAllTodoItemsTest()
        {
            var response = await _client.GetAsync("/api/ToDoItems");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            var toDoItems = JsonConvert.DeserializeObject<List<ToDoItem>>(content);
            
            toDoItems.Should().HaveCount(3);
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
        public async Task GetTodoItemTest()
        {
            var response = await _client.GetAsync("/api/ToDoItems/1");
            var content = await response.Content.ReadAsStringAsync();
            var toDoItems = JsonConvert.DeserializeObject<ToDoItem>(content);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            toDoItems.Id.Should().Be(1);
            toDoItems.Text.Should().Be("ToDo item 1");
            toDoItems.Completed.Should().Be(false);
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

            var postResponse = await _client.PostAsync(request.Url, DbInit.GetStringContent(request.Body));
            var jsonFromPostResponse = await postResponse.Content.ReadAsStringAsync();
            var singleResponse = JsonConvert.DeserializeObject<ToDoItem>(jsonFromPostResponse); 
            var getResponse = await _client.GetAsync($"/api/ToDoItems/{singleResponse.Id}");

            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await getResponse.Content.ReadAsStringAsync();
            var toDoItems = JsonConvert.DeserializeObject<ToDoItem>(content);
            
            toDoItems.Id.Should().Be(4);
            toDoItems.Text.Should().Be("A new POST ToDo item");
            toDoItems.Completed.Should().Be(false);
        }
        
        [Fact]
        public async Task PutTodoItemTest()
        {
            var request = new
            {
                Url = "/api/ToDoItems/1",
                Body = new
                {
                    Text = "A new PUT ToDo item",
                    Completed = true
                }
            };

            var putResponse = await _client.PutAsync(request.Url, DbInit.GetStringContent(request.Body));
            var jsonFromPutResponse = await putResponse.Content.ReadAsStringAsync();
            var putToDoItem = JsonConvert.DeserializeObject<ToDoItem>(jsonFromPutResponse);
            
            putToDoItem.Text.Should().Be("A new PUT ToDo item");
            putToDoItem.Completed.Should().Be(true);
            
            var getResponse = await _client.GetAsync(request.Url);

            putResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await getResponse.Content.ReadAsStringAsync();
            var getToDoItem = JsonConvert.DeserializeObject<ToDoItem>(content);
            
            getToDoItem.Id.Should().Be(1);
            getToDoItem.Text.Should().Be("A new PUT ToDo item");
            getToDoItem.Completed.Should().Be(true);
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

            var postResponse = await _client.PostAsync(postRequest.Url, DbInit.GetStringContent(postRequest.Body));
            var jsonFromPostResponse = await postResponse.Content.ReadAsStringAsync();
            var singleResponse = JsonConvert.DeserializeObject<ToDoItem>(jsonFromPostResponse);
            var deleteResponse = await _client.DeleteAsync($"/api/ToDoItems/{singleResponse.Id}");
            var getResponse = await _client.GetAsync($"/api/ToDoItems/{singleResponse.Id}");

            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);           
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
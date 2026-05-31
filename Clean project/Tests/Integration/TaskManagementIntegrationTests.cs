using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using Infrastructure;
using System.Net;

namespace Tests.Integration
{
    public class TaskManagementIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public TaskManagementIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task AuthFlow_RegisterLoginAndAccessProtectedEndpoint_ShouldWork()
        {
            // Step 1: Register a new user
            var registerRequest = new
            {
                Username = "integrationtestuser",
                Password = "password123"
            };

            var registerJson = JsonSerializer.Serialize(registerRequest);
            var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");

            var registerResponse = await _client.PostAsync("/api/auth/register", registerContent);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var registerResponseContent = await registerResponse.Content.ReadAsStringAsync();
            var registerResult = JsonSerializer.Deserialize<JsonElement>(registerResponseContent);
            
            string? token = null;
            if (registerResult.TryGetProperty("Token", out var tokenElement))
            {
                token = tokenElement.GetString();
            }
            else if (registerResult.TryGetProperty("token", out tokenElement))
            {
                token = tokenElement.GetString();
            }

            Assert.NotNull(token);

            // Step 2: Use token to access protected endpoint
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dashboardResponse = await _client.GetAsync("/api/task/dashboard");
            Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);

            var dashboardContent = await dashboardResponse.Content.ReadAsStringAsync();
            var dashboardResult = JsonSerializer.Deserialize<JsonElement>(dashboardContent);
            
            // Should have default task counts (all zeros for new user)
            Assert.True(dashboardResult.TryGetProperty("Pending", out _));
            Assert.True(dashboardResult.TryGetProperty("InProgress", out _));
            Assert.True(dashboardResult.TryGetProperty("Completed", out _));
        }

        [Fact]
        public async Task TaskCrudFlow_CreateReadUpdateDelete_ShouldWork()
        {
            // First, register and login to get a token
            var token = await RegisterAndGetToken("crudtestuser", "password123");
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Step 1: Create a task
            var createTaskRequest = new
            {
                Title = "Integration Test Task",
                Description = "This is a test task created during integration testing",
                Status = "Pending",
                Priority = 7,
                DueDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-ddTHH:mm:ss"),
                Category = "Testing"
            };

            var createJson = JsonSerializer.Serialize(createTaskRequest);
            var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");

            var createResponse = await _client.PostAsync("/api/task/create", createContent);
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

            // Step 2: Get all tasks to verify creation
            var getTasksResponse = await _client.GetAsync("/api/task/all");
            Assert.Equal(HttpStatusCode.OK, getTasksResponse.StatusCode);

            var tasksContent = await getTasksResponse.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<JsonElement[]>(tasksContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            Assert.Single(tasks);
            var createdTask = tasks[0];
            
            // Try different property name cases
            string? title = null;
            if (createdTask.TryGetProperty("Title", out var titleElement))
                title = titleElement.GetString();
            else if (createdTask.TryGetProperty("title", out titleElement))
                title = titleElement.GetString();
                
            Assert.Equal("Integration Test Task", title);

            int taskId = 0;
            if (createdTask.TryGetProperty("Id", out var idElement))
                taskId = idElement.GetInt32();
            else if (createdTask.TryGetProperty("id", out idElement))
                taskId = idElement.GetInt32();

            // Step 3: Update the task
            var updateTaskRequest = new
            {
                Title = "Updated Integration Test Task",
                Description = "This task has been updated",
                Status = "InProgress",
                Priority = 9,
                DueDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-ddTHH:mm:ss"),
                Category = "Testing"
            };

            var updateJson = JsonSerializer.Serialize(updateTaskRequest);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

            var updateResponse = await _client.PutAsync($"/api/task/update/{taskId}", updateContent);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            // Step 4: Get the specific task to verify update
            var getTaskResponse = await _client.GetAsync($"/api/task/{taskId}");
            Assert.Equal(HttpStatusCode.OK, getTaskResponse.StatusCode);

            var taskContent = await getTaskResponse.Content.ReadAsStringAsync();
            var updatedTask = JsonSerializer.Deserialize<JsonElement>(taskContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            // Try different property name cases
            string? updatedTitle = null;
            if (updatedTask.TryGetProperty("Title", out var updatedTitleElement))
                updatedTitle = updatedTitleElement.GetString();
            else if (updatedTask.TryGetProperty("title", out updatedTitleElement))
                updatedTitle = updatedTitleElement.GetString();
                
            string? updatedStatus = null;
            if (updatedTask.TryGetProperty("Status", out var statusElement))
                updatedStatus = statusElement.GetString();
            else if (updatedTask.TryGetProperty("status", out statusElement))
                updatedStatus = statusElement.GetString();
                
            int updatedPriority = 0;
            if (updatedTask.TryGetProperty("Priority", out var priorityElement))
                updatedPriority = priorityElement.GetInt32();
            else if (updatedTask.TryGetProperty("priority", out priorityElement))
                updatedPriority = priorityElement.GetInt32();
            
            Assert.Equal("Updated Integration Test Task", updatedTitle);
            Assert.Equal("InProgress", updatedStatus);
            Assert.Equal(9, updatedPriority);

            // Step 5: Delete the task
            var deleteResponse = await _client.DeleteAsync($"/api/task/delete/{taskId}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            // Step 6: Verify task is deleted (should return 500 when trying to get deleted task)
            var getDeletedTaskResponse = await _client.GetAsync($"/api/task/{taskId}");
            Assert.Equal(HttpStatusCode.InternalServerError, getDeletedTaskResponse.StatusCode);
        }

        [Fact]
        public async Task UnauthorizedAccess_ShouldReturn401()
        {
            // Try to access protected endpoint without token
            var response = await _client.GetAsync("/api/task/all");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task InvalidLogin_ShouldReturnUnauthorized()
        {
            var loginRequest = new
            {
                Username = "nonexistentuser",
                Password = "wrongpassword"
            };

            var loginJson = JsonSerializer.Serialize(loginRequest);
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", loginContent);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<string> RegisterAndGetToken(string username, string password)
        {
            var registerRequest = new
            {
                Username = username,
                Password = password
            };

            var registerJson = JsonSerializer.Serialize(registerRequest);
            var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");

            var registerResponse = await _client.PostAsync("/api/auth/register", registerContent);
            var registerResponseContent = await registerResponse.Content.ReadAsStringAsync();
            var registerResult = JsonSerializer.Deserialize<JsonElement>(registerResponseContent);
            
            // Try both "Token" and "token" property names
            if (registerResult.TryGetProperty("Token", out var tokenElement))
            {
                return tokenElement.GetString()!;
            }
            else if (registerResult.TryGetProperty("token", out tokenElement))
            {
                return tokenElement.GetString()!;
            }
            
            throw new Exception("Token not found in response");
        }
    }
}
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using TodoApp.Api;
using TodoApp.Core.Integration;
using Xunit;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Tests.Integration
{
    public class TodoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TodoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("IntegrationTests");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            }).CreateClient();

        }

        [Fact]
        public async Task GetAllTodos_ShouldReturnBadRequest_WhenInvalidModel()
        {
            var response = await _client.GetAsync("/api/todo");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTodo_ShouldReturnBadRequest_WhenInvalidModel()
        {
            var invalidTodo = new { Title = "" };

            var response = await _client.PostAsJsonAsync("/api/todo", invalidTodo);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

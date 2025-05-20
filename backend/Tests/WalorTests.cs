using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Http.Json;
using System.Text.Json;
using Domain;
using FluentAssertions;
using Infrastracture;
using WaloryBackend.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace Tests
{

    public class WalorIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public WalorIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RequireConfirmedEmail_Should_Forbid_When_EmailNotConfirmed()
        {
            // Arrange
            var user = await _factory.CreateUserAsync(emailConfirmed: false);
            _client.DefaultRequestHeaders.Authorization = await _factory.GetAuthHeaderAsync(user);

            // Act
            var response = await _client.GetAsync("/api/WalorInstances");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Can_Create_Update_Delete_WalorInstance_With_Valid_Schema()
        {
            // Arrange
            var user = await _factory.CreateUserAsync(emailConfirmed: true);
            _client.DefaultRequestHeaders.Authorization = await _factory.GetAuthHeaderAsync(user);

            var schemaJson = JsonDocument.Parse("""
        {
            "type": "object",
            "properties": {
                "name": {"type": "string"},
                "value": {"type": "number"}
            },
            "required": ["name", "value"]
        }
        """);

            var createTemplateResult = await _client.PostAsJsonAsync("/WalorTemplates", new
            {
                Category = "test",
                Content = schemaJson,
                Visibility = Visibility.Public
            });

            createTemplateResult.EnsureSuccessStatusCode();

            var context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            var templateId = context.Templates.First().Id;

            var collection = new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Test Collection",
                AuthorId = user.Id,
                WalorTemplateId = templateId
            };
            context.Collections.Add(collection);
            await context.SaveChangesAsync();

            var instanceData = JsonDocument.Parse(""" {"name":"Item1", "value":10} """);

            // Create WalorInstance
            var createResp = await _client.PostAsJsonAsync("/api/WalorInstances", new
            {
                CollectionId = collection.Id,
                Data = instanceData
            });
            createResp.EnsureSuccessStatusCode();

            var walorId = context.Walors.First().Id;

            // Update WalorInstance with valid data
            var updateData = JsonDocument.Parse(""" {"name":"Updated", "value":15} """);
            var updateResp = await _client.PutAsJsonAsync($"/api/WalorInstances/{walorId}", new
            {
                WalorInstanceId = walorId,
                Data = updateData
            });
            updateResp.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            // Delete WalorInstance
            var deleteResp = await _client.DeleteAsync($"/api/WalorInstances/{walorId}");
            deleteResp.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateWalorInstance_Should_Fail_If_Schema_Does_Not_Validate()
        {
            // Arrange
            var user = await _factory.CreateUserAsync(emailConfirmed: true);
            _client.DefaultRequestHeaders.Authorization = await _factory.GetAuthHeaderAsync(user);

            var schemaJson = JsonDocument.Parse("""
        {
            "type": "object",
            "properties": {
                "name": {"type": "string"},
                "value": {"type": "number"}
            },
            "required": ["name", "value"]
        }
        """);

            var createTemplateResult = await _client.PostAsJsonAsync("/WalorTemplates", new
            {
                Category = "test",
                Content = schemaJson,
                Visibility = Visibility.Public
            });

            createTemplateResult.EnsureSuccessStatusCode();

            var context = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            var templateId = context.Templates.First().Id;

            var collection = new Collection
            {
                Id = Guid.NewGuid(),
                Name = "Invalid Schema Collection",
                AuthorId = user.Id,
                WalorTemplateId = templateId
            };
            context.Collections.Add(collection);
            await context.SaveChangesAsync();

            var instanceData = JsonDocument.Parse(""" {"name":"Item1", "value":10} """);
            var createResp = await _client.PostAsJsonAsync("/api/WalorInstances", new
            {
                CollectionId = collection.Id,
                Data = instanceData
            });
            createResp.EnsureSuccessStatusCode();

            var walorId = context.Walors.First().Id;

            // Attempt update with invalid schema
            var invalidUpdate = JsonDocument.Parse(""" {"name":12345} """);
            var updateResp = await _client.PutAsJsonAsync($"/api/WalorInstances/{walorId}", new
            {
                WalorInstanceId = walorId,
                Data = invalidUpdate
            });

            updateResp.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }


}

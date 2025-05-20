using Domain;
using Infrastracture;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests
{
    public class UpdateWalorInstanceTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly DataContext _context;

        public UpdateWalorInstanceTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _context = factory.Services.GetRequiredService<DataContext>();
        }

        [Fact]
        public async Task UpdateWalorInstance_ValidJson_UpdatesSuccessfully()
        {
            // Arrange: Utwórz szablon z JSON Schema
            var schema = @"{
            ""type"": ""object"",
            ""properties"": {
                ""title"": {""type"": ""string""},
                ""value"": {""type"": ""number""}
            },
            ""required"": [""title"", ""value""]
        }";

            var template = new WalorTemplate
            {
                Id = Guid.NewGuid(),
                AuthorId = "user-1",
                Category = "Test",
                Content = JsonDocument.Parse(schema),
                Visibility = Visibility.Public
            };

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Stwórz kolekcję i instancję waloru
            var collection = new WalorCollection
            {
                Id = Guid.NewGuid(),
                WalorTemplateId = template.Id
            };

            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();

            var walor = new WalorInstance
            {
                Id = Guid.NewGuid(),
                TemplateId = template.Id,
                CollectionId = collection.Id,
                Data = JsonDocument.Parse(@"{ ""title"": ""Initial"", ""value"": 123 }")
            };

            _context.Walors.Add(walor);
            await _context.SaveChangesAsync();

            // Act: próbujemy zaktualizować walor
            var updateCommand = new UpdateWalorInstance.UpdateWalorInstanceCommand
            {
                WalorInstanceId = walor.Id,
                Data = JsonDocument.Parse(@"{ ""title"": ""Updated"", ""value"": 456 }")
            };

            var response = await _client.PutAsJsonAsync($"/api/WalorInstances/{walor.Id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updated = await _context.Walors.FindAsync(walor.Id);
            updated.Data.RootElement.GetProperty("title").GetString().Should().Be("Updated");
        }
    }

}

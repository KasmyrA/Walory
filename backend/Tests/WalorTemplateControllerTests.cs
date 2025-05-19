using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests
{
    public class WalorTemplateControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public WalorTemplateControllerTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTemplate_ShouldReturnOk()
        {
            var content = new
            {
                category = "Testowa",
                content = JsonDocument.Parse("""
            {
                "fields": [
                    { "name": "waga", "type": "number" },
                    { "name": "cena", "type": "number" }
                ]
            }
            """),
                visibility = Visibility.Private
            };

            var httpContent = new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/walortemplates", httpContent);
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, result);
        }

        [Fact]
        public async Task ImportTemplate_ShouldReturnOk()
        {
            // Get template ID (you can query or hardcode it if you know from seed)
            var templateId = await GetTemplateIdAsync();

            var response = await _client.PostAsync($"/api/walortemplates/{templateId}/import", null);
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, result);
        }

        private async Task<Guid> GetTemplateIdAsync()
        {
            // Use direct DbContext or temporary API method to expose templates for test
            // Example fallback:
            return (await Task.FromResult(Guid.NewGuid())); // Replace with actual call
        }
    }

}

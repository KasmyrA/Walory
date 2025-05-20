using Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class RequireConfirmedEmailTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public RequireConfirmedEmailTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UnconfirmedEmail_ShouldReturn_Forbidden()
        {
            var client = _factory.CreateClient();
            var email = "nl307893@student.polsl.pl";
            var password = "Test1234!";

            // Register user
            await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Name = "Test", Password = password });

            // Login (email NOT confirmed)
            await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });

            // Attempt to create collection
            var response = await client.PostAsJsonAsync("/collection", new
            {
                CollectionDTO = new
                {
                    Title = "Blocked",
                    Description = "Should fail",
                    Visibility = 0,
                    WalorTemplateId = Guid.NewGuid()
                }
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ConfirmedEmail_ShouldAllowAccess()
        {
            var client = _factory.CreateClient();
            var email = "user2@example.com";
            var password = "Test1234!";

            await client.PostAsJsonAsync("/api/auth/register", new { Email = email, Name = "Test", Password = password });

            // Confirm email manually
            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var user = await userManager.FindByEmailAsync(email);
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
            }

            // Login
            await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });

            // Try to create collection
            var response = await client.PostAsJsonAsync("/collection", new
            {
                CollectionDTO = new
                {
                    Title = "Accessible",
                    Description = "Should pass",
                    Visibility = 0,
                    WalorTemplateId = Guid.NewGuid()
                }
            });

            Assert.True(response.IsSuccessStatusCode);
        }
    }

}

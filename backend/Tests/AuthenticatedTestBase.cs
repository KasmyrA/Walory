using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public abstract class AuthenticatedTestBase : IClassFixture<TestWebApplicationFactory>
    {
        protected readonly HttpClient Client;
        protected readonly IServiceProvider Services;

        protected string TestEmail = "user2@example.com";
        protected string TestPassword = "Test1234!";

        protected AuthenticatedTestBase(TestWebApplicationFactory factory)
        {
            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            Services = factory.Services;

            EnsureLoggedIn().GetAwaiter().GetResult();
        }

        private async Task EnsureLoggedIn()
        {
            // Rejestracja użytkownika
            await Client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = TestEmail,
                Name = "BaseTest",
                Password = TestPassword
            });

            // Potwierdzenie emaila w bazie
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(TestEmail);
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);

            // Logowanie
            var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new
            {
                Email = TestEmail,
                Password = TestPassword
            });

            loginResponse.EnsureSuccessStatusCode();
        }
    }

}

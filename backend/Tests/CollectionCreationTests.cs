using Application.CQRS.Collection;
using Application.DTO;
using Domain;
using Infrastracture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests
{
    public class CollectionCreationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public CollectionCreationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Can_Create_Collection()
        {
            // Przygotuj HttpContext z zalogowanym userem
            await _factory.SetAuthenticatedUserAsync("user2@example.com");

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            var handler = new CreateCollection.CreateCollectionCommandHandler(context, userManager, httpContextAccessor);

            var user = await userManager.FindByEmailAsync("user2@example.com");

            var template = new WalorTemplate
            {
                Id = Guid.NewGuid(),
                Category = "Test",
                Content = JsonDocument.Parse("{\"type\":\"object\"}"),
                AuthorId = user.Id,
                Author = user
            };
            context.Templates.Add(template);
            await context.SaveChangesAsync();

            var command = new CreateCollection.CreateCollectionCommand
            {
                CollectionDTO = new CollectionDTO
                {
                    Title = "My Collection",
                    Description = "Test desc",
                    Visibility = Visibility.Private,
                    WalorTemplateId = template.Id
                }
            };

            var result = await handler.Handle(command, default);

            Assert.True(result.isSuccess);
        }
    }




}

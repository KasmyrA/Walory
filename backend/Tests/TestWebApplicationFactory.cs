using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Infrastracture;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _dbContainer;

    public TestWebApplicationFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithDatabase("testdb")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services =>
        {
            await _dbContainer.StartAsync();

            // Usuń domyślny kontekst
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DataContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Dodaj testowy kontekst
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            // Build i seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            await db.Database.EnsureCreatedAsync();
            await SeedTestDataAsync(db);
        });
    }

    private static async Task SeedTestDataAsync(DataContext db)
    {
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "test@test.com",
            Email = "test@test.com"
        };

        db.Users.Add(testUser);

        var template = new WalorTemplate
        {
            Id = Guid.NewGuid(),
            AuthorId = testUser.Id,
            Category = "Elektronika",
            Content = JsonDocument.Parse("""
            {
                "fields": [
                    { "name": "cena", "type": "number" },
                    { "name": "data_produkcji", "type": "date" }
                ]
            }
            """),
            Visibility = Visibility.Public
        };

        db.Templates.Add(template);
        await db.SaveChangesAsync();
    }

    public async Task StopAsync() => await _dbContainer.StopAsync();
}

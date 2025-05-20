using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Infrastracture;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
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
        builder.ConfigureServices(services =>
        {

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DataContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await context.Database.EnsureCreatedAsync();
        await Seed.SeedData(context, userManager);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    public async Task SetAuthenticatedUserAsync(string userEmail)
    {
        using var scope = Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        var user = await userManager.FindByEmailAsync(userEmail);
        if (user == null) throw new Exception($"User with email {userEmail} not found.");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Convert Guid to string
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = principal
        };
    }
}

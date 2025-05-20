using Domain;
using Infrastracture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    //public class WalorInstanceTests : IClassFixture<TestWebApplicationFactory>
    //{
    //    private readonly HttpClient _client;
    //    private readonly DataContext _context;

    //    public WalorInstanceTests(TestWebApplicationFactory factory)
    //    {
    //        _client = factory.CreateClient();
    //        _context = factory.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
    //    }

    //    private WalorInstance GetAnyWalorFromPublicCollection()
    //    {
    //        return _context.Walors
    //            .Include(w => w.Template)
    //            .Include(w => w.Collection)
    //            .First(w => w.Collection.Title == "My Collection");
    //    }

    //    [Fact]
    //    public async Task UpdateWalorInstance_WithValidData_ShouldSucceed()
    //    {
    //        // Arrange
    //        var walor = GetAnyWalorFromPublicCollection();
    //        var updateCommand = new
    //        {
    //            walorInstanceId = walor.Id,
    //            data = new
    //            {
    //                price = 200,
    //                productionDate = "2024-05-01",
    //                name = "Updated Walor"
    //            }
    //        };

    //        // Act
    //        var response = await _client.PutAsJsonAsync($"/walor-instance/{walor.Id}", updateCommand);

    //        // Assert
    //        var updated = await _context.Walors.FindAsync(walor.Id);
    //        var updatedJson = updated.Data.RootElement.GetProperty("name").GetString();
    //        Assert.Equal("Updated Walor", updatedJson);
    //    }

    //    [Fact]
    //    public async Task UpdateWalorInstance_MissingRequiredField_ShouldFail()
    //    {
    //        var walor = GetAnyWalorFromPublicCollection();
    //        var updateCommand = new
    //        {
    //            walorInstanceId = walor.Id,
    //            data = new
    //            {
    //                price = 300,
    //                productionDate = "2024-06-01"
    //                // brak name
    //            }
    //        };

    //        var response = await _client.PutAsJsonAsync($"/walor-instance/{walor.Id}", updateCommand);

    //        Assert.False(response.IsSuccessStatusCode);
    //        var errorText = await response.Content.ReadAsStringAsync();
    //        Assert.Contains("Walor data does not match the template", errorText);
    //    }

    //    [Fact]
    //    public async Task UpdateWalorInstance_WrongType_ShouldFail()
    //    {
    //        var walor = GetAnyWalorFromPublicCollection();
    //        var updateCommand = new
    //        {
    //            walorInstanceId = walor.Id,
    //            data = new
    //            {
    //                price = "invalid_string",
    //                productionDate = "2024-07-01",
    //                name = "Wrong Price"
    //            }
    //        };

    //        var response = await _client.PutAsJsonAsync($"/walor-instance/{walor.Id}", updateCommand);

    //        Assert.False(response.IsSuccessStatusCode);
    //        var errorText = await response.Content.ReadAsStringAsync();
    //        Assert.Contains("Walor data does not match the template", errorText);
    //    }

    //    [Fact]
    //    public async Task DeleteWalorInstance_ShouldSucceed()
    //    {
    //        var walor = GetAnyWalorFromPublicCollection();
    //        Assert.NotNull(walor);

    //        var response = await _client.DeleteAsync($"/walor-instance/{walor.Id}");

    //        var raw = await response.Content.ReadAsStringAsync();
    //        Console.WriteLine($"Delete response: {raw}");

    //        response.EnsureSuccessStatusCode(); // <- może failować jak id nie istnieje
    //        var deleted = await _context.Walors.FindAsync(walor.Id);
    //        Assert.Null(deleted);
    //    }
    //}

}

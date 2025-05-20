using Application.CQRS.Walor;
using Infrastracture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests
{
    public class UpdateWalorInstanceHandlerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly DataContext _context;

        public UpdateWalorInstanceHandlerTests(TestWebApplicationFactory fixture)
        {
            _context = fixture.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
        }

        [Fact]
        public async Task UpdateWalorInstance_WithValidData_ShouldSucceed()
        {
            // Arrange
            var walor = await _context.Walors.Include(w => w.Template).FirstAsync();
            var handler = new UpdateWalorInstance.Handler(_context);

            var updatedJson = JsonDocument.Parse("""
        {
            "price": 199.99,
            "productionDate": "2024-02-20",
            "name": "Updated Walor"
        }
        """);

            var command = new UpdateWalorInstance.UpdateWalorInstanceCommand
            {
                WalorInstanceId = walor.Id,
                Data = updatedJson
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            var updatedWalor = await _context.Walors.FindAsync(walor.Id);
            Assert.True(result.isSuccess);
            Assert.Equal("Updated Walor", updatedWalor.Data.RootElement.GetProperty("name").GetString());
        }

        [Fact]
        public async Task UpdateWalorInstance_MissingRequiredField_ShouldFail()
        {
            // Arrange
            var walor = await _context.Walors.Include(w => w.Template).FirstAsync();
            var handler = new UpdateWalorInstance.Handler(_context);

            var invalidJson = JsonDocument.Parse("""
        {
            "price": 199.99,
            "productionDate": "2024-02-20"
        }
        """); // Brakuje "name"

            var command = new UpdateWalorInstance.UpdateWalorInstanceCommand
            {
                WalorInstanceId = walor.Id,
                Data = invalidJson
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("does not match the template", result.Error);
        }

        [Fact]
        public async Task UpdateWalorInstance_MissingRequiredField_ShouldFail_WrongData()
        {
            // Arrange
            var walor = await _context.Walors.Include(w => w.Template).FirstAsync();
            var handler = new UpdateWalorInstance.Handler(_context);

            var invalidJson = JsonDocument.Parse("""
        {
            "price": "cos",
            "productionDate": "2024-02-20",
            "name": "Updated Walor"
        }
        """); // Changed price

            var command = new UpdateWalorInstance.UpdateWalorInstanceCommand
            {
                WalorInstanceId = walor.Id,
                Data = invalidJson
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("does not match the template", result.Error);
        }
    }

}

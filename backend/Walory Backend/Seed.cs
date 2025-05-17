using Domain;
using Infrastracture;

public static class Seed
{
    public static async Task SeedData(DataContext context)
    {
        if (!context.Users.Any())
        {
            /*context.Users.Add(new User { Name = "administrator", Email = "admin@admin.com", Password = "Zaq12wsx" });
            await context.SaveChangesAsync();*/
        }
    }
}

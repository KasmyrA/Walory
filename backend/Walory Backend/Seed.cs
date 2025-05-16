public static class Seed
{
    public static async Task SeedData(DataContext context)
    {
        if (!context.Users.Any())
        {
            context.Users.Add(new User { UserName = "admin", Email = "admin@example.com" });
            // Dodaj inne dane testowe wed³ug potrzeb
            await context.SaveChangesAsync();
        }
    }
}

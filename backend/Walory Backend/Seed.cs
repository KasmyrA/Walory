using Domain;
using Domain.Notificaiton;
using Infrastracture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

public static class Seed
{
    public static async Task SeedData(DataContext context, UserManager<User> userManager)
    {
        if (context.Users.Any())
            return;

        var users = GetSeedUsers();

        // Inicjalizacja kolekcji w u¿ytkownikach
        foreach (var user in users)
        {
            user.Collections ??= new List<Collection>();
            user.Templates ??= new List<WalorTemplate>();
        }

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, "Zaq!2wsx");
            if (!result.Succeeded)
            {
                Console.WriteLine(
                    $"Nie uda³o siê utworzyæ u¿ytkownika {user}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Pobierz u¿ytkowników do relacji
        var userEmails = users.Select(u => u.Email).ToList();
        var userDict = new Dictionary<string, User>();
        foreach (var email in userEmails)
        {
            var u = await userManager.FindByEmailAsync(email);
            if (u != null)
                userDict[email] = u;
        }

        // Skróty do u¿ytkowników
        var admin = userDict["admin@walor.com"];
        var user1 = userDict["user1@walor.com"];
        var creator = userDict["creator@walor.com"];
        var artist = userDict["artist@walor.com"];
        var collector = userDict["collector@walor.com"];
        var moderator = userDict["moderator@walor.com"];
        var developer = userDict["developer@walor.com"];
        var newUser = userDict["newuser@walor.com"];
        var testUser = userDict["testuser@walor.com"];

        // --- WALORY, SZABLONY, KOLEKCJE ---

        var templates = new List<WalorTemplate>
        {
            new WalorTemplate
            {
                Id = Guid.NewGuid(),
                Category = "Sztuka",
                Content = JsonDocument.Parse("{\"fields\": [\"Tytu³\", \"Opis\", \"Rok\"]}"),
                AuthorId = artist.Id,
                Author = artist,
                Visibility = Visibility.Public
            },
            new WalorTemplate
            {
                Id = Guid.NewGuid(),
                Category = "Technologia",
                Content = JsonDocument.Parse("{\"fields\": [\"Nazwa\", \"Wersja\", \"Opis\"]}"),
                AuthorId = developer.Id,
                Author = developer,
                Visibility = Visibility.Public
            }
        };

        var collections = new List<Collection>
        {
            new Collection
            {
                Id = Guid.NewGuid(),
                Title = "Kolekcja Sztuki",
                Description = "Wyj¹tkowe dzie³a cyfrowe",
                Visibility = Visibility.Public,
                OwnerId = artist.Id,
                Owner = artist,
                WalorTemplateId = templates[0].Id,
                WalorTemplate = templates[0],
                Walors = new List<WalorInstance>()
            },
            new Collection
            {
                Id = Guid.NewGuid(),
                Title = "Nowinki Technologiczne",
                Description = "Zbiór najnowszych technologii",
                Visibility = Visibility.Friends,
                OwnerId = developer.Id,
                Owner = developer,
                WalorTemplateId = templates[1].Id,
                WalorTemplate = templates[1],
                Walors = new List<WalorInstance>()
            }
        };

        var walors = new List<WalorInstance>
        {
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("{\"Tytu³\": \"Obraz 1\", \"Opis\": \"Abstrakcja\", \"Rok\": 2023}"),
                TemplateId = templates[0].Id,
                Template = templates[0],
                CollectionId = collections[0].Id,
                Collection = collections[0]
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("{\"Tytu³\": \"Obraz 2\", \"Opis\": \"Portret\", \"Rok\": 2022}"),
                TemplateId = templates[0].Id,
                Template = templates[0],
                CollectionId = collections[0].Id,
                Collection = collections[0]
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("{\"Tytu³\": \"Obraz 3\", \"Opis\": \"Pejza¿\", \"Rok\": 2021}"),
                TemplateId = templates[0].Id,
                Template = templates[0],
                CollectionId = collections[0].Id,
                Collection = collections[0]
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("{\"Nazwa\": \"AI Chip\", \"Wersja\": \"1.0\", \"Opis\": \"Nowoczesny uk³ad AI\"}"),
                TemplateId = templates[1].Id,
                Template = templates[1],
                CollectionId = collections[1].Id,
                Collection = collections[1]
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("{\"Nazwa\": \"Quantum CPU\", \"Wersja\": \"2.0\", \"Opis\": \"Procesor kwantowy\"}"),
                TemplateId = templates[1].Id,
                Template = templates[1],
                CollectionId = collections[1].Id,
                Collection = collections[1]
            }
        };

        // Przypisz walory do kolekcji
        collections[0].Walors.Add(walors[0]);
        collections[0].Walors.Add(walors[1]);
        collections[0].Walors.Add(walors[2]);
        collections[1].Walors.Add(walors[3]);
        collections[1].Walors.Add(walors[4]);

        // Przypisz kolekcje i szablony do u¿ytkowników
        artist.Collections.Add(collections[0]);
        artist.Templates.Add(templates[0]);
        developer.Collections.Add(collections[1]);
        developer.Templates.Add(templates[1]);

        context.Templates.AddRange(templates);
        context.Collections.AddRange(collections);
        context.Walors.AddRange(walors);

        // --- RESZTA SEEDOWANIA (przyjaŸnie, powiadomienia itd.) ---
        // ... (Twój dotychczasowy kod)

        await context.SaveChangesAsync();

        // Dodaj powiadomienia o nowych walorach
        foreach (var walor in walors)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = walor.Collection.OwnerId,
                Type = NotificationType.NewWalor,
                Message = $"Dodano nowy walor do kolekcji {walor.Collection.Title}",
                ReferenceId = walor.Id,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            context.Notifications.Add(notification);
        }

        await context.SaveChangesAsync();
    }

    // Upewnij siê, ¿e ta metoda istnieje i zwraca listê u¿ytkowników do seedowania
    private static List<User> GetSeedUsers()
    {
        // ... implementacja
        throw new NotImplementedException();
    }
}
    




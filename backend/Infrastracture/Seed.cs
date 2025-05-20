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


        if (await context.Users.AnyAsync()) return;

        var user1 = new User { UserName = "user1@example.com", Email = "user1@example.com", Name = "One", EmailConfirmed = false };
        var user2 = new User { UserName = "user2@example.com", Email = "user2@example.com", Name = "Two", EmailConfirmed = true };
        var user3 = new User { UserName = "user3@example.com", Email = "user3@example.com", Name = "Three", EmailConfirmed = true };
        var user4 = new User { UserName = "user4@example.com", Email = "user4@example.com", Name = "Four", EmailConfirmed = true };

        foreach (var user in new[] { user1, user2, user3, user4 })
        {
            await userManager.CreateAsync(user, "Test1234!");
        }

        context.UserFriends.AddRange(new[]
        {
            new UserFriend { UserId = user2.Id, FriendId = user3.Id },
            new UserFriend { UserId = user3.Id, FriendId = user2.Id }
        });

        var schemaJson = """
        {
          "type": "object",
          "properties": {
            "price": { "type": "number" },
            "productionDate": { "type": "string", "format": "date" },
            "name": { "type": "string" }
          },
          "required": ["price", "productionDate", "name"]
        }
        """;

        var content = JsonDocument.Parse(schemaJson);


        var templatePublic = new WalorTemplate
        {
            Id = Guid.NewGuid(),
            AuthorId = user2.Id,
            Category = "CategoryA",
            Content = content,
            Visibility = Visibility.Public
        };

        var templatePrivate = new WalorTemplate
        {
            Id = Guid.NewGuid(),
            AuthorId = user3.Id,
            Category = "CategoryB",
            Content = content,
            Visibility = Visibility.Private
        };

        var templateFriends = new WalorTemplate
        {
            Id = Guid.NewGuid(),
            AuthorId = user2.Id,
            Category = "CategoryC",
            Content = content,
            Visibility = Visibility.Friends
        };

        context.Templates.AddRange(templatePublic, templatePrivate, templateFriends);

        var collection1 = new Collection
        {
            Id = Guid.NewGuid(),
            Title = "My Collection",
            Description = "Sample description for my collection",
            Visibility = Visibility.Public,
            OwnerId = user2.Id,
            WalorTemplateId = templatePublic.Id,
            Walors = new List<WalorInstance>(),
            Comments = new List<Comment>(),
            Likes = new List<Like>()
        };
        var collection2 = new Collection
        {
            Id = Guid.NewGuid(),
            Title = "My Friends",
            Description = "Sample description for my friends",
            Visibility = Visibility.Friends,
            OwnerId = user2.Id,
            WalorTemplateId = templateFriends.Id,
            Walors = new List<WalorInstance>(),
            Comments = new List<Comment>(),
            Likes = new List<Like>()
        };
        var collection3 = new Collection
        {
            Id = Guid.NewGuid(),
            Title = "My Private",
            Description = "Sample description for my private",
            Visibility = Visibility.Private,
            OwnerId = user3.Id,
            WalorTemplateId = templatePrivate.Id,
            Walors = new List<WalorInstance>(),
            Comments = new List<Comment>(),
            Likes = new List<Like>()
        };

        for (int i = 0; i < 3; i++)
        {
            collection1.Walors.Add(new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse($$"""
                {
                  "price": {{i * 100 + 50}},
                  "productionDate": "2024-01-{{(i + 1):D2}}",
                  "name": "Walor #{{i + 1}}"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection1
            });
        }
        
        var originalWalors = new[] {
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 999.99,
                  "productionDate": "2020-12-24",
                  "name": "Z³oty zegarek"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection1
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 0,
                  "productionDate": "2010-05-01",
                  "name": "Pami¹tkowy bilet"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection1
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 12345.67,
                  "productionDate": "2022-07-15",
                  "name": "Samochód kolekcjonerski"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection1
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 12.5,
                  "productionDate": "2023-03-08",
                  "name": "Stara moneta"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection2
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 500,
                  "productionDate": "2018-11-11",
                  "name": "Limitowana figurka"
                }
                """),
                TemplateId = templateFriends.Id,
                Collection = collection2
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 1,
                  "productionDate": "2000-01-01",
                  "name": "Pierwszy znaczek"
                }
                """),
                TemplateId = templatePrivate.Id,
                Collection = collection3
            },
            new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse("""
                {
                  "price": 2500.75,
                  "productionDate": "2019-09-30",
                  "name": "Obraz olejny"
                }
                """),
                TemplateId = templatePrivate.Id,
                Collection = collection3
            }
        };

                // Dodaj oryginalne walory do odpowiednich kolekcji
                collection1.Walors.Add(originalWalors[0]);
                collection1.Walors.Add(originalWalors[1]);
                collection1.Walors.Add(originalWalors[2]);
                collection2.Walors.Add(originalWalors[3]);
                collection2.Walors.Add(originalWalors[4]);
                collection3.Walors.Add(originalWalors[5]);
                collection3.Walors.Add(originalWalors[6]);

        for (int i = 0; i < 9; i++)
        {
            collection2.Walors.Add(new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse($$"""
                {
                  "price": {{i * 100 + 50}},
                  "productionDate": "2024-01-{{(i + 1):D2}}",
                  "name": "Walor #{{i + 1}}"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection1
            });
        }

        for (int i = 0; i < 30; i++)
        {
            collection3.Walors.Add(new WalorInstance
            {
                Id = Guid.NewGuid(),
                Data = JsonDocument.Parse($$"""
                {
                  "price": {{i * 100 + 50}},
                  "productionDate": "2024-01-{{(i + 1):D2}}",
                  "name": "Walor #{{i + 1}}"
                }
                """),
                TemplateId = templatePublic.Id,
                Collection = collection1
            });
        }
        context.Collections.AddRange(collection1,collection2,collection3);

        // === Like ===
        context.Likes.Add(new Like
        {
            Id = Guid.NewGuid(),
            CollectionId = collection1.Id,
            UserId = user3.Id
        });
        context.Likes.Add(new Like
        {
            Id = Guid.NewGuid(),
            CollectionId = collection1.Id,
            UserId = user4.Id
        });

        // === Comment ===
        context.Comments.Add(new Comment
        {
            Id = Guid.NewGuid(),
            AuthorId = user3.Id,
            CollectionId = collection1.Id,
            Content = "Œwietna kolekcja!",
            CreatedAt = DateTime.UtcNow
        });

        // === Chat Message ===
        context.Messages.Add(new ChatMessage
        {
            Id = Guid.NewGuid(),
            SenderId = user2.Id,
            ReceiverId = user3.Id,
            Content = "Czeœæ! Jak siê masz?",
            SentAt = DateTime.UtcNow
        });

        // === Notifications ===
        context.Notifications.AddRange(new[]
        {
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user3.Id,
                Message = "Masz nowe polubienie",
                Type = NotificationType.MessageSent,
                IsRead = false
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user3.Id,
                Message = "Nowa wiadomoœæ",
                Type = NotificationType.MessageSent,
                IsRead = false
            },
                        new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                Message = "Przyjaciel dodaj wiadomoœæ",
                Type = NotificationType.FriendRequest,
                IsRead = false
            }
        });

        // === Friend Request ===
        context.FriendRequests.Add(new FriendRequest
        {
            Id = Guid.NewGuid(),
            SenderId = user4.Id,
            ReceiverId = user2.Id,
            IsAccepted = false,
            SentAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

    }


}
    




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

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, "Zaq!2wsx");
            if (!result.Succeeded)
            {
                // Mo¿esz dodaæ logowanie b³êdów, np. do konsoli lub pliku
                Console.WriteLine($"Nie uda³o siê utworzyæ u¿ytkownika {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
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

        // PrzyjaŸnie
        var friendships = new List<UserFriend>
        {
            new() { UserId = user1.Id, User = user1, FriendId = creator.Id, Friend = creator },
            new() { UserId = artist.Id, User = artist, FriendId = creator.Id, Friend = creator },
            new() { UserId = collector.Id, User = collector, FriendId = artist.Id, Friend = artist },
            new() { UserId = moderator.Id, User = moderator, FriendId = admin.Id, Friend = admin },
            new() { UserId = developer.Id, User = developer, FriendId = user1.Id, Friend = user1 }
        };

        // Relacje zwrotne
        friendships.AddRange(friendships.Select(f => new UserFriend
        {
            UserId = f.FriendId,
            User = f.Friend,
            FriendId = f.UserId,
            Friend = f.User
        }));

        // Zaproszenia do znajomych
        var pendingFriendRequests = new List<FriendRequest>
        {
            new() { SenderId = newUser.Id, Sender = newUser, ReceiverId = collector.Id, Receiver = collector, IsAccepted = false, SentAt = DateTime.UtcNow.AddDays(-2) },
            new() { SenderId = newUser.Id, Sender = newUser, ReceiverId = developer.Id, Receiver = developer, IsAccepted = false, SentAt = DateTime.UtcNow.AddDays(-1) },
            new() { SenderId = artist.Id, Sender = artist, ReceiverId = admin.Id, Receiver = admin, IsAccepted = false, SentAt = DateTime.UtcNow.AddHours(-12) }
        };

        // Dodaj nowe zaproszenia do znajomych
        pendingFriendRequests.AddRange(new List<FriendRequest>
        {
            new() { SenderId = testUser.Id, Sender = testUser, ReceiverId = user1.Id, Receiver = user1, IsAccepted = false, SentAt = DateTime.UtcNow.AddHours(-5) },
            new() { SenderId = artist.Id, Sender = artist, ReceiverId = collector.Id, Receiver = collector, IsAccepted = false, SentAt = DateTime.UtcNow.AddHours(-8) },
            new() { SenderId = admin.Id, Sender = admin, ReceiverId = newUser.Id, Receiver = newUser, IsAccepted = false, SentAt = DateTime.UtcNow.AddHours(-3) }
        });

        var acceptedFriendRequests = new List<FriendRequest>
        {
            new() { SenderId = user1.Id, Sender = user1, ReceiverId = creator.Id, Receiver = creator, IsAccepted = true, SentAt = DateTime.UtcNow.AddDays(-10) },
            new() { SenderId = moderator.Id, Sender = moderator, ReceiverId = admin.Id, Receiver = admin, IsAccepted = true, SentAt = DateTime.UtcNow.AddDays(-5) }
        };

        // Dodaj nowe zaakceptowane zaproszenia
        acceptedFriendRequests.AddRange(new List<FriendRequest>
        {
            new() { SenderId = developer.Id, Sender = developer, ReceiverId = artist.Id, Receiver = artist, IsAccepted = true, SentAt = DateTime.UtcNow.AddDays(-8) },
            new() { SenderId = collector.Id, Sender = collector, ReceiverId = moderator.Id, Receiver = moderator, IsAccepted = true, SentAt = DateTime.UtcNow.AddDays(-6) }
        });

        // Powiadomienia
        var notifications = pendingFriendRequests.Select(request => new Notification
        {
            UserId = request.ReceiverId,
            Type = NotificationType.FriendRequest,
            Message = $"{request.Sender.Name} sent you a friend request",
            IsRead = false,
            CreatedAt = request.SentAt,
            ReferenceId = request.Id
        }).ToList();

        // Dodaj nowe powiadomienia
        notifications.AddRange(new List<Notification>
        {
            new()
            {
                UserId = user1.Id,
                Type = NotificationType.MessageSent,
                Message = "Otrzyma³eœ now¹ wiadomoœæ od u¿ytkownika Content Creator",
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddHours(-12),
                ReferenceId = Guid.NewGuid()
            },
            new()
            {
                UserId = creator.Id,
                Type = NotificationType.MessageSent,
                Message = "Otrzyma³eœ now¹ wiadomoœæ od u¿ytkownika Administrator",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                ReferenceId = Guid.NewGuid()
            },
            new()
            {
                UserId = collector.Id,
                Type = NotificationType.FriendRequest,
                Message = "Digital Artist chce zostaæ twoim znajomym",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-8),
                ReferenceId = pendingFriendRequests.Last(pr => pr.ReceiverId == collector.Id).Id
            }
        });

        // Dodaj wiadomoœci czatu
        var chatMessages = new List<ChatMessage>
        {
            new() {
                SenderId = user1.Id,
                Sender = user1,
                ReceiverId = creator.Id,
                Receiver = creator,
                Content = "Czeœæ! Jak siê masz?",
                SentAt = DateTime.UtcNow.AddDays(-1).AddHours(-2),
                IsRead = true
            },
            new() {
                SenderId = creator.Id,
                Sender = creator,
                ReceiverId = user1.Id,
                Receiver = user1,
                Content = "Hej! Wszystko dobrze, dziêki. Co s³ychaæ?",
                SentAt = DateTime.UtcNow.AddDays(-1).AddHours(-1),
                IsRead = true
            },
            new() {
                SenderId = user1.Id,
                Sender = user1,
                ReceiverId = creator.Id,
                Receiver = creator,
                Content = "Zastanawiam siê nad stworzeniem nowej kolekcji. Masz jakieœ porady?",
                SentAt = DateTime.UtcNow.AddHours(-12),
                IsRead = false
            },
            new() {
                SenderId = admin.Id,
                Sender = admin,
                ReceiverId = creator.Id,
                Receiver = creator,
                Content = "Dzieñ dobry! Widzia³em twoj¹ najnowsz¹ kolekcjê - wygl¹da œwietnie!",
                SentAt = DateTime.UtcNow.AddHours(-6),
                IsRead = false
            },
            new() {
                SenderId = moderator.Id,
                Sender = moderator,
                ReceiverId = admin.Id,
                Receiver = admin,
                Content = "Muszê zg³osiæ problem z now¹ funkcj¹ systemu.",
                SentAt = DateTime.UtcNow.AddHours(-3),
                IsRead = true
            },
            new() {
                SenderId = admin.Id,
                Sender = admin,
                ReceiverId = moderator.Id,
                Receiver = moderator,
                Content = "Mo¿esz opisaæ dok³adniej o co chodzi?",
                SentAt = DateTime.UtcNow.AddHours(-2),
                IsRead = false
            }
        };

        context.UserFriends.AddRange(friendships);
        context.FriendRequests.AddRange(pendingFriendRequests);
        context.FriendRequests.AddRange(acceptedFriendRequests);
        context.Notifications.AddRange(notifications);
        context.Messages.AddRange(chatMessages);
        await context.SaveChangesAsync();
    }

    private static List<User> GetSeedUsers() => new()
    {
        new User { UserName = "admin@walor.com", Email = "admin@walor.com", Name = "Administrator", Description = "System administrator account", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "user1@walor.com", Email = "user1@walor.com", Name = "Regular User", Description = "Standard user account", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "creator@walor.com", Email = "creator@walor.com", Name = "Content Creator", Description = "User who creates templates and collections", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "artist@walor.com", Email = "artist@walor.com", Name = "Digital Artist", Description = "Creates visual templates and collections", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "collector@walor.com", Email = "collector@walor.com", Name = "Collector", Description = "Specializes in collecting rare templates", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "moderator@walor.com", Email = "moderator@walor.com", Name = "Content Moderator", Description = "Helps maintain quality content", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "developer@walor.com", Email = "developer@walor.com", Name = "Developer", Description = "Creates technical templates", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "newuser@walor.com", Email = "newuser@walor.com", Name = "New User", Description = "Just joined the platform", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() },
        new User { UserName = "testuser@walor.com", Email = "testuser@walor.com", Name = "Test User", Description = "Test user account", Friends = new List<UserFriend>(), Collections = new List<Collection>(), Templates = new List<WalorTemplate>() }
    };
    
}




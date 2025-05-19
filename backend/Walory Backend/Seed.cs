using Domain;
using Domain.Notificaiton;
using Infrastracture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class Seed
{
    public static async Task SeedData(DataContext context, UserManager<User> userManager)
    {
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    UserName = "admin@walor.com",
                    Email = "admin@walor.com",
                    Name = "Administrator",
                    Description = "System administrator account",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                new User
                {
                    UserName = "user1@walor.com",
                    Email = "user1@walor.com",
                    Name = "Regular User",
                    Description = "Standard user account",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                new User
                {
                    UserName = "creator@walor.com",
                    Email = "creator@walor.com",
                    Name = "Content Creator",
                    Description = "User who creates templates and collections",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                // 5 additional users
                new User
                {
                    UserName = "artist@walor.com",
                    Email = "artist@walor.com",
                    Name = "Digital Artist",
                    Description = "Creates visual templates and collections",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                new User
                {
                    UserName = "collector@walor.com",
                    Email = "collector@walor.com",
                    Name = "Collector",
                    Description = "Specializes in collecting rare templates",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                new User
                {
                    UserName = "moderator@walor.com",
                    Email = "moderator@walor.com",
                    Name = "Content Moderator",
                    Description = "Helps maintain quality content",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                new User
                {
                    UserName = "developer@walor.com",
                    Email = "developer@walor.com",
                    Name = "Developer",
                    Description = "Creates technical templates",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                },
                new User
                {
                    UserName = "newuser@walor.com",
                    Email = "newuser@walor.com",
                    Name = "New User",
                    Description = "Just joined the platform",
                    Friends = new List<UserFriend>(),
                    Collections = new List<Collection>(),
                    Templates = new List<WalorTemplate>()
                }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Zaq12wsx");
            }

            // Retrieve created users for relationships
            var admin = await userManager.FindByEmailAsync("admin@walor.com");
            var user1 = await userManager.FindByEmailAsync("user1@walor.com");
            var creator = await userManager.FindByEmailAsync("creator@walor.com");
            var artist = await userManager.FindByEmailAsync("artist@walor.com");
            var collector = await userManager.FindByEmailAsync("collector@walor.com");
            var moderator = await userManager.FindByEmailAsync("moderator@walor.com");
            var developer = await userManager.FindByEmailAsync("developer@walor.com");
            var newUser = await userManager.FindByEmailAsync("newuser@walor.com");
            
            // Create friendships between users
            var friendships = new List<UserFriend>
            {
                // Original friendship
                new UserFriend
                {
                    UserId = user1.Id,
                    User = user1,
                    FriendId = creator.Id,
                    Friend = creator
                },
                // New friendships
                new UserFriend
                {
                    UserId = artist.Id,
                    User = artist,
                    FriendId = creator.Id,
                    Friend = creator
                },
                new UserFriend
                {
                    UserId = collector.Id,
                    User = collector,
                    FriendId = artist.Id,
                    Friend = artist
                },
                new UserFriend
                {
                    UserId = moderator.Id,
                    User = moderator,
                    FriendId = admin.Id,
                    Friend = admin
                },
                new UserFriend
                {
                    UserId = developer.Id,
                    User = developer,
                    FriendId = user1.Id,
                    Friend = user1
                }
            };
            
            // Add reciprocal friendships (for bidirectional relationships)
            var reciprocalFriendships = new List<UserFriend>();
            foreach (var friendship in friendships)
            {
                reciprocalFriendships.Add(new UserFriend
                {
                    UserId = friendship.FriendId,
                    User = friendship.Friend,
                    FriendId = friendship.UserId,
                    Friend = friendship.User
                });
            }
            
            friendships.AddRange(reciprocalFriendships);
            
            // Add friend requests (not yet accepted)
            var pendingFriendRequests = new List<FriendRequest>
            {
                new FriendRequest
                {
                    SenderId = newUser.Id,
                    Sender = newUser,
                    ReceiverId = collector.Id,
                    Receiver = collector,
                    IsAccepted = false,
                    SentAt = DateTime.UtcNow.AddDays(-2)
                },
                new FriendRequest
                {
                    SenderId = newUser.Id,
                    Sender = newUser,
                    ReceiverId = developer.Id,
                    Receiver = developer,
                    IsAccepted = false,
                    SentAt = DateTime.UtcNow.AddDays(-1)
                },
                new FriendRequest
                {
                    SenderId = artist.Id,
                    Sender = artist,
                    ReceiverId = admin.Id,
                    Receiver = admin,
                    IsAccepted = false,
                    SentAt = DateTime.UtcNow.AddHours(-12)
                }
            };
            
            // Add already accepted friend requests to demonstrate complete workflow
            var acceptedFriendRequests = new List<FriendRequest>
            {
                new FriendRequest
                {
                    SenderId = user1.Id,
                    Sender = user1,
                    ReceiverId = creator.Id,
                    Receiver = creator,
                    IsAccepted = true,
                    SentAt = DateTime.UtcNow.AddDays(-10)
                },
                new FriendRequest
                {
                    SenderId = moderator.Id,
                    Sender = moderator,
                    ReceiverId = admin.Id,
                    Receiver = admin,
                    IsAccepted = true,
                    SentAt = DateTime.UtcNow.AddDays(-5)
                }
            };
            
            // Create notification for friend requests
            var notifications = new List<Notification>();
            foreach (var request in pendingFriendRequests)
            {
                notifications.Add(new Notification
                {
                    UserId = request.ReceiverId,
                    Type = NotificationType.FriendRequest,
                    Message = $"{request.Sender.Name} sent you a friend request",
                    IsRead = false,
                    CreatedAt = request.SentAt,
                    ReferenceId = request.Id
                });
            }
            
            // Save all relationships to the database
            context.UserFriends.AddRange(friendships);
            context.FriendRequests.AddRange(pendingFriendRequests);
            context.FriendRequests.AddRange(acceptedFriendRequests);
            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();
        }
    }
}

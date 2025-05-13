using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser
    {
        public Guid Id { get; set; }
        public string Name { get;set; }
        public string Email { get;set; }
        public string Password { get; set; }

        public string? Description { get; set; }

        public ICollection<UserFriend> Friends { get; set; }

        public ICollection<Collection> Collections { get; set; }

        public ICollection<WalorTemplate> Templates { get; set; }
    }
}


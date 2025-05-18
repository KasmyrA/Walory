using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get;set; }
        public string? Description { get; set; }
        public string? EmailConfirmationCode { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public string? EmailChangeCode { get; set; }
        public string? PendingNewEmail { get; set; }
        public ICollection<UserFriend> Friends { get; set; }

        public ICollection<Collection> Collections { get; set; }

        public ICollection<WalorTemplate> Templates { get; set; }
    }
}


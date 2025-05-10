
namespace Walory_Backend
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get;set; }
        public string Email { get;set; }
        public string Password { get; set; }

        public string? Description { get; set; }

        public string ProfilePicUrl { get; set; }

        public ICollection<Walor> Collection { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public enum Visibility { Public, Private, Friends }
    public class Collection
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Visibility Visibility { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; }

        public Guid WalorTemplateId { get; set; }
        public WalorTemplate WalorTemplate { get; set; }
        public ICollection<WalorInstance> Walors { get; set; } = new List<WalorInstance>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }

}

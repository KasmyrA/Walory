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

        public ICollection<Walor> Walors { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }

}

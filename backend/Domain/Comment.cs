using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public User Author { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? CollectionId { get; set; }
        public Collection Collection { get; set; }

    }

}

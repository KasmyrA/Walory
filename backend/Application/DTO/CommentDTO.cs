using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CommentDTO
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CollectionId { get; set; }
        public AuthorDto Author { get; set; }
    }
}

using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CollectionDTO
    {
        public Guid CollectionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public Visibility Visibility { get; set; }
        public Guid WalorTemplateId { get; set; }
        public AuthorDto? Author { get; set; }
        public List<WalorInstanceDto>? WalorInstance { get; set; } 
    }
}

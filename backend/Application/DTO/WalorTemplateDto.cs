using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class WalorTemplateDto
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public JsonDocument Content { get; set; }
        public Visibility Visibility { get; set; }
        public AuthorDto Author { get; set; }
    }

}

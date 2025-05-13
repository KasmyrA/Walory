using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain
{
    public class WalorTemplate
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public JsonDocument Content { get; set; } 

        public Guid AuthorId { get; set; }
        public User Author { get; set; }

        public Visibility Visibility { get; set; } 
    }
}

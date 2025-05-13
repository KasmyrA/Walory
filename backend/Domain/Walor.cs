using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class Walor
    {
        public Guid Id { get; set; }
        public string Category { get; set; }

        public JsonDocument Content { get; set; } // pole JSON

        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }

    }

}

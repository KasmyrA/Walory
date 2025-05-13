using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class WalorInstance
    {
        public Guid Id { get; set; }

        public JsonDocument Data { get; set; } 

        public Guid TemplateId { get; set; }
        public WalorTemplate Template { get; set; }

        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
    }

}

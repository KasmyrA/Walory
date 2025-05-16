using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class WalorInstanceDto
    {
        public Guid Id { get; set; }
        public JsonDocument Data { get; set; }
        public Guid TemplateId { get; set; }
    }

}

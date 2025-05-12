using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Like
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }

}

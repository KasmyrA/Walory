
using Infrastracture;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class GetWalorImageQuery : IRequest<byte[]?>
    {
        public Guid WalorId { get; set; }
    }
    public class GetWalorImageQueryHandler : IRequestHandler<GetWalorImageQuery, byte[]?>
    {
        private readonly DataContext _context;

        public GetWalorImageQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<byte[]?> Handle(GetWalorImageQuery request, CancellationToken cancellationToken)
        {
            var walor = await _context.Walors.FindAsync(request.WalorId);
            return walor?.Image;
        }
    }


}

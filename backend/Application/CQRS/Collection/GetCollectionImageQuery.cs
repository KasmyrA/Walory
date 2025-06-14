using Infrastracture;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Collection
{
    public class GetCollectionImageQuery : IRequest<byte[]?>
    {
        public Guid CollectionId { get; set; }
    }
    public class GetCollectionImageQueryHandler : IRequestHandler<GetCollectionImageQuery, byte[]?>
    {
        private readonly DataContext _context;

        public GetCollectionImageQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<byte[]?> Handle(GetCollectionImageQuery request, CancellationToken cancellationToken)
        {
            var collection = await _context.Collections.FindAsync(request.CollectionId);
            return collection?.Image;
        }
    }

}

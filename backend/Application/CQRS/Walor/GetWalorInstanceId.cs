using Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class GetWalorInstanceById
    {
        public class Query : IRequest<WalorInstanceDto>
        {
            public Guid WalorInstanceId { get; set; }
        }

        public class Handler : IRequestHandler<Query, WalorInstanceDto>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<WalorInstanceDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var walor = await _context.WalorInstances
                    .Include(w => w.Template)
                    .Include(w => w.Collection).ThenInclude(c => c.Owner)
                    .FirstOrDefaultAsync(w => w.Id == request.WalorInstanceId, cancellationToken);

                if (walor == null) return null;

                return new WalorInstanceDto
                {
                    Id = walor.Id,
                    Data = walor.Data,
                    CollectionId = walor.CollectionId,
                    TemplateId = walor.TemplateId,
                    Author = new AuthorDto
                    {
                        Id = walor.Collection.Owner.Id,
                        Email = walor.Collection.Owner.Email,
                        FullName = walor.Collection.Owner.FullName
                    }
                };
            }
        }
    }

}

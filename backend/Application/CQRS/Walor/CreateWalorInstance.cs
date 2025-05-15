using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class CreateWalorInstance
    {
        public class Command : IRequest<Result<Guid>>
        {
            public Guid CollectionId { get; set; }
            public JsonDocument Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var collection = await _context.Collections
                    .Include(c => c.WalorTemplate)
                    .FirstOrDefaultAsync(c => c.Id == request.CollectionId);

                if (collection == null)
                    return Result<Guid>.Failure("Kolekcja nie istnieje");

                var walor = new WalorInstance
                {
                    Id = Guid.NewGuid(),
                    CollectionId = collection.Id,
                    TemplateId = collection.WalorTemplateId,
                    Data = request.Data
                };

                _context.WalorInstances.Add(walor);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(walor.Id);
            }
        }
    }

}

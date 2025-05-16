using Domain;
using Infrastracture;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        public class Command : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; }
            public JsonDocument Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var collection = await _context.Collections
                    .Include(c => c.WalorTemplate)
                    .FirstOrDefaultAsync(c => c.Id == request.CollectionId);

                if (collection == null)
                    return Result<Unit>.Failure("Kolekcja nie istnieje");

                var walor = new WalorInstance
                {
                    CollectionId = collection.Id,
                    TemplateId = collection.WalorTemplateId,
                    Data = request.Data
                };

                _context.Walors.Add(walor);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Error");
            }
        }
    }

}

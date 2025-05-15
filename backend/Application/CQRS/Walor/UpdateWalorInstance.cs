using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class UpdateWalorInstance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid WalorInstanceId { get; set; }
            public JsonDocument Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var walor = await _context.WalorInstances.FindAsync(request.WalorInstanceId);

                if (walor == null)
                    return Result<Unit>.Failure("Walor nie znaleziony");

                walor.Data = request.Data;

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Nie udało się zaktualizować");
            }
        }
    }

}

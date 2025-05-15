using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class DeleteWalorInstance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid WalorInstanceId { get; set; }
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
                if (walor == null) return Result<Unit>.Failure("Nie znaleziono waloru");

                _context.WalorInstances.Remove(walor);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Nie udało się usunąć");
            }
        }
    }

}

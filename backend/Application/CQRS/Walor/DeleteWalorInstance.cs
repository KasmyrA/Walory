using Infrastracture;
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
        public class DeleteWalorInstanceCommand : IRequest<Result<Unit>>
        {
            public Guid WalorInstanceId { get; set; }
        }

        public class Handler : IRequestHandler<DeleteWalorInstanceCommand, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(DeleteWalorInstanceCommand request, CancellationToken cancellationToken)
            {
                var walor = await _context.Walors.FindAsync(request.WalorInstanceId);
                if (walor == null) return Result<Unit>.Failure("Walor not found");

                _context.Walors.Remove(walor);
                var success = await _context.SaveChangesAsync(cancellationToken) > 0;

                return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Couldn't delete walor");
            }
        }
    }

}

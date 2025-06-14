
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Walor
{
    public class UploadWalorImageCommand : IRequest<Result<Unit>>
    {
        public Guid WalorId { get; set; }
        public IFormFile ImageFile { get; set; }
    }
    public class UploadWalorImageCommandHandler : IRequestHandler<UploadWalorImageCommand, Result<Unit>>
    {
        private readonly DataContext _context;

        public UploadWalorImageCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(UploadWalorImageCommand request, CancellationToken cancellationToken)
        {
            var walor = await _context.Walors.FindAsync(request.WalorId);
            if (walor == null)
                return Result<Unit>.Failure("Walor not found");

            using var ms = new MemoryStream();
            await request.ImageFile.CopyToAsync(ms);
            walor.Image = ms.ToArray();

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to upload walor image");
        }
    }

}

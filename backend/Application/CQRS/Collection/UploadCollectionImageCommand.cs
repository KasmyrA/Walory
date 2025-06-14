using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Collection
{
    public class UploadCollectionImageCommand : IRequest<Result<Unit>>
    {
        public Guid CollectionId { get; set; }
        public IFormFile ImageFile { get; set; }
    }
    public class UploadCollectionImageCommandHandler : IRequestHandler<UploadCollectionImageCommand, Result<Unit>>
    {
        private readonly DataContext _context;

        public UploadCollectionImageCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(UploadCollectionImageCommand request, CancellationToken cancellationToken)
        {
            var collection = await _context.Collections.FindAsync(request.CollectionId);
            if (collection == null)
                return Result<Unit>.Failure("Collection not found");

            using var ms = new MemoryStream();
            await request.ImageFile.CopyToAsync(ms);
            collection.Image = ms.ToArray();

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to upload collection image");
        }
    }

}

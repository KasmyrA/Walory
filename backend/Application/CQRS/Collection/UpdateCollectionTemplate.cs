using Infrastracture;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Collection
{
    public class UpdateCollectionTemplate
    {
        public class UpdateCollectionTemplateCommand : IRequest<Result<Unit>>
        {
            public Guid CollectionId { get; set; }
            public Guid NewWalorTemplateId { get; set; }
        }

        public class UpdateCollectionTemplateCommandHandler : IRequestHandler<UpdateCollectionTemplateCommand, Result<Unit>>
        {
            private readonly DataContext _context;

            public UpdateCollectionTemplateCommandHandler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(UpdateCollectionTemplateCommand request, CancellationToken cancellationToken)
            {
                var collection = await _context.Collections
                    .Include(c => c.Walors)
                    .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

                if (collection == null)
                {
                    return Result<Unit>.Failure("Collection not found");
                }

                var templateExists = await _context.Templates.AnyAsync(t => t.Id == request.NewWalorTemplateId, cancellationToken);

                if (!templateExists)
                    return Result<Unit>.Failure("Template not found");

                collection.WalorTemplateId = request.NewWalorTemplateId;


                _context.Collections.Update(collection);


                var affectedRows = await _context.Walors
                    .Where(w => w.CollectionId == request.CollectionId)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(w => w.TemplateId, request.NewWalorTemplateId),
                        cancellationToken);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;



                if (!result) return Result<Unit>.Failure("Failed to delete notification");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}

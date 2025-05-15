using Application.DTO;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Show
{
    public class GetPublicCollections
    {
        public class Query : IRequest<PaginatedList<CollectionDto>>
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        public class Handler : IRequestHandler<Query, PaginatedList<CollectionDto>>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<PaginatedList<CollectionDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Collections
                    .Include(c => c.Owner)
                    .Include(c => c.WalorTemplate)
                    .Where(c => c.Visibility == Visibility.Public)
                    .OrderByDescending(c => c.Title)
                    .Select(c => new CollectionDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        Category = c.WalorTemplate.Category,
                        Visibility = c.Visibility,
                        Owner = new AuthorDto
                        {
                            Id = c.Owner.Id,
                            Email = c.Owner.Email,
                            FullName = c.Owner.FullName
                        }
                    });

                return await PaginatedList<CollectionDto>.CreateAsync(query, request.PageNumber, request.PageSize);
            }
        }
    }

}

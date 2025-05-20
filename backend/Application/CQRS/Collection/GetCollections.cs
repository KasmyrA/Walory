using Application.DTO;
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
    public class GetCollections
    {
        public class Query : IRequest<PaginatedList<CollectionDTO>>
        {
            public string Category { get; set; }
            public string SortOrder { get; set; } = "asc";
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        public class Handler : IRequestHandler<Query, PaginatedList<CollectionDTO>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<PaginatedList<CollectionDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Collections
                    .Include(c => c.WalorTemplate)
                    .Include(c => c.Owner)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Category))
                {
                    query = query.Where(c => c.WalorTemplate.Category == request.Category);
                }

                query = request.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.WalorTemplate.Category)
                    : query.OrderBy(c => c.WalorTemplate.Category);

                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(c => new CollectionDTO
                    {
                        CollectionId = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        Visibility = c.Visibility,
                        WalorTemplateId = c.WalorTemplateId,
                    })
                    .ToListAsync(cancellationToken);

                var count = await query.CountAsync(cancellationToken);

                return new PaginatedList<CollectionDTO>(items, count, request.PageNumber, request.PageSize);
            }
        }
    }


}

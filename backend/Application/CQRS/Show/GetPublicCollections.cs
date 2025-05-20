using Application.DTO;
using Domain;
using Infrastracture;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Show
{
    public class GetPublicCollections
    {
        public class Query : IRequest<PaginatedList<CollectionDTO>>
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public string? Category { get; set; }
            public string SortOrder { get; set; } = "asc";
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
                    .Include(c => c.Owner)
                    .Include(c => c.WalorTemplate)
                    .Include(c => c.Walors)
                    .Where(c => c.Visibility == Visibility.Public)
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
                       Category = c.WalorTemplate.Category,
                       Visibility = c.Visibility,
                       WalorTemplateId = c.WalorTemplateId,
                       Author = new AuthorDto
                       {
                           Id = c.Owner.Id,
                           Email = c.Owner.Email,
                           Name = c.Owner.Name
                       },
                       WalorInstance = c.Walors.Select(wi => new WalorInstanceDto
                       {
                           Id = wi.Id,
                           TemplateId = wi.TemplateId,
                           Data = wi.Data
                       }).ToList()
                   })
                   .ToListAsync(cancellationToken);

                var count = await query.CountAsync(cancellationToken);

                return new PaginatedList<CollectionDTO>(items, count, request.PageNumber, request.PageSize);
            }
        }
    }

}

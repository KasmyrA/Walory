using Application.DTO;
using Domain;
using Infrastracture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Show
{
    public class GetPrivateCollections
    {
        public class Query : IRequest<PaginatedList<CollectionDTO>>
        {
            public string? Category { get; set; }
            public string SortOrder { get; set; } = "asc";
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        public class Handler : IRequestHandler<Query, PaginatedList<CollectionDTO>>
        {
            private readonly DataContext _context;
            private readonly IHttpContextAccessor _http;
            private readonly UserManager<User> _userManager;

            public Handler(DataContext context, IHttpContextAccessor http, UserManager<User> userManager)
            {
                _context = context;
                _http = http;
                _userManager = userManager;
            }

            public async Task<PaginatedList<CollectionDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var query = _context.Collections
                    .Include(c => c.Walors).
                    Where(c => c.Owner.Id == user.Id)
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

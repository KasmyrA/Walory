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
    public class GetPrivateTemplates
    {
        public class Query : IRequest<PaginatedList<WalorTemplateDto>>
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        public class Handler : IRequestHandler<Query, PaginatedList<WalorTemplateDto>>
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

            public async Task<PaginatedList<WalorTemplateDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var query = _context.Templates
                    .Include(c => c.Author).
                    Where(c => c.Author.Id == user.Id)
                                       .Select(t => new WalorTemplateDto
                                       {
                                           Id = t.Id,
                                           Category = t.Category,
                                           Content = t.Content,
                                           Visibility = t.Visibility,
                                           Author = new AuthorDto
                                           {
                                               Id = t.Author.Id,
                                               Email = t.Author.Email,
                                               Name = t.Author.Name
                                           }
                                       });


                return await PaginatedList<WalorTemplateDto>.CreateAsync(query, request.PageNumber, request.PageSize);


            }
        }
    }
}

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
    public class GetAccessibleTemplates
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
                var friendIds = await _context.UserFriends
                    .Where(f => f.UserId == user.Id)
                    .Select(f => f.FriendId)
                    .ToListAsync();

                var query = _context.Templates
                    .Include(t => t.Author)
                    .Where(t =>
                        t.Visibility == Visibility.Public ||
                        (t.Visibility == Visibility.Friends && friendIds.Contains(t.AuthorId)))
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

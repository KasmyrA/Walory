using Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Collection
{
    public class GetCollections2
    {
        public class GetCollectionsHandler : IRequestHandler<GetCollectionsQuery, PaginatedList<CollectionDto>>
        {
            private readonly AppDbContext _context;
            private readonly IUserContextService _userContext;

            public GetCollectionsHandler(AppDbContext context, IUserContextService userContext)
            {
                _context = context;
                _userContext = userContext;
            }

            public async Task<PaginatedList<CollectionDto>> Handle(GetCollectionsQuery request, CancellationToken cancellationToken)
            {
                var userId = _userContext.UserId;
                var query = _context.Collections
                    .Include(c => c.Author)
                    .AsQueryable();

                // Filtrowanie po kategorii
                if (!string.IsNullOrEmpty(request.Category))
                {
                    query = query.Where(c => c.Category == request.Category);
                }

                // Filtrowanie po widoczności
                switch (request.Visibility)
                {
                    case VisibilityFilter.Public:
                        query = query.Where(c => c.IsPublic);
                        break;
                    case VisibilityFilter.Private:
                        query = query.Where(c => c.AuthorId == userId && !c.IsPublic);
                        break;
                    case VisibilityFilter.Friends:
                        var friendIds = await _context.Friends
                            .Where(f => f.UserId == userId)
                            .Select(f => f.FriendId)
                            .ToListAsync(cancellationToken);
                        query = query.Where(c => friendIds.Contains(c.AuthorId) && c.IsPublic);
                        break;
                }

                // Sortowanie
                query = request.SortBy?.ToLower() switch
                {
                    "name" => request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "createdat" => request.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };

                // Paginacja
                var totalItems = await query.CountAsync(cancellationToken);
                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                    .Select(c => new CollectionDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Category = c.Category,
                        CreatedAt = c.CreatedAt,
                        Author = new AuthorDto
                        {
                            Id = c.Author.Id,
                            Email = c.Author.Email,
                            FullName = c.Author.FullName
                        }
                    })
                    .ToListAsync(cancellationToken);

                return new PaginatedList<CollectionDto>(items, totalItems, request.PageNumber, request.PageSize);
            }
        }

    }
}

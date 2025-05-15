using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GetCollectionsQuery : IRequest<PaginatedList<CollectionDto>>
    {
        public string? Category { get; set; }
        public string? SortBy { get; set; } // Np. "createdAt", "name"
        public bool SortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public VisibilityFilter Visibility { get; set; } = VisibilityFilter.Public;
    }
}

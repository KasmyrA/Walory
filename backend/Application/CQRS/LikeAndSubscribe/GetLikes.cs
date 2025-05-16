using Infrastracture;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.LikeAndSubscribe
{
    public class GetLikes
    {
        public class GetLikesCountQuery : IRequest<int>
        {
            public Guid CollectionId { get; set; }
        }

        public class GetLikesCountHandler : IRequestHandler<GetLikesCountQuery, int>
        {
            private readonly DataContext _context;

            public GetLikesCountHandler(DataContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetLikesCountQuery request, CancellationToken cancellationToken)
            {
                return await _context.Likes
                    .CountAsync(l => l.CollectionId == request.CollectionId, cancellationToken);
            }
        }

    }
}

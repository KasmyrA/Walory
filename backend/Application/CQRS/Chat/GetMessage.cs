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

namespace Application.CQRS.Chat
{
    public class GetMessages
    {
        public class Query : IRequest<List<ChatMessageDto>>
        {
            public Guid FriendId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<ChatMessageDto>>
        {
            private readonly DataContext _context;
            private readonly UserManager<User> _userManager;

            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(DataContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<List<ChatMessageDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                return await _context.Messages
                    .Where(m => (m.SenderId == user.Id && m.ReceiverId == request.FriendId)
                             || (m.SenderId == request.FriendId && m.ReceiverId == user.Id))
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        Content = m.Content,
                        SentAt = m.SentAt
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }
}

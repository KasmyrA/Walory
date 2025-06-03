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

namespace Application.CQRS.Notification
{
    public class NotificationRequest
    {
        public class Query : IRequest<Result<List<NotificationDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<NotificationDto>>>
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

            public async Task<Result<List<NotificationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == user.Id)
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new NotificationDto
                    {
                        Id = n.Id,
                        Message = n.Message,
                        Type = n.Type,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        ReferenceId = n.ReferenceId
                    })
                    .ToListAsync();

                return Result<List<NotificationDto>>.Success(notifications);
            }
        }
    }
}

using Infrastracture;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Chat
{
    public class GetPhotoQuery : IRequest<byte[]?>
    {
        public Guid UserId { get; set; }
    }
    public class GetPhotoQueryHandler : IRequestHandler<GetPhotoQuery, byte[]?>
    {
        private readonly DataContext _context;

        public GetPhotoQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<byte[]?> Handle(GetPhotoQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            return user?.AvatarImage;
        }
    }


}

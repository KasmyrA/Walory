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
using static System.Net.WebRequestMethods;

namespace Application.CQRS.PDF
{
    public class GetCategoryList
    {
        public class Query : IRequest<List<string>>
        {

        }

        public class Handler : IRequestHandler<Query, List<string>>
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


            public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_http.HttpContext.User);

                var query = _context.Collections.Where(c => c.OwnerId ==  user.Id).Select(c => c.WalorTemplate.Category).Where(category => !string.IsNullOrEmpty(category)).Distinct().ToListAsync(cancellationToken);
                
               return await query; 

            }
        }
    }
}

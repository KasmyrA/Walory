using FluentEmail.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailService
    {
        private readonly IFluentEmail _email;

        public EmailService(IFluentEmail email)
        {
            _email = email;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _email
                .To(to)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();
        }
    }

}

using MediatR;
using MusicWebAppBackend.Infrastructure.Models;

namespace MusicWebAppBackend.Infrastructure.Commands
{
    public class SendEmailCommand : IRequest<string>
    {
        public string ViewName { get; set; }
        public EmailContent Model { get; set; }
    }
}

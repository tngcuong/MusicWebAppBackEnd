using System.Net;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class CustomHttpStatus 
    {
        public HttpStatusCode StatusCode { get; private set; }
        public object Data { get; private set; }

        public CustomHttpStatus(HttpStatusCode statusCode, object data)
        {
            StatusCode = statusCode;
            Data = data;
        }
    }
}

using MusicWebAppBackend.Infrastructure.Models.Paging;
using System.Net;

namespace MusicWebAppBackend.Infrastructure.ViewModels
{
    public class PagedResponse<T> : ResponseBase
    {
        public PageList<T> Data { get; set; }

        public static PagedResponse<T> OK(PageList<T> data, string message = "")
        {
            return new PagedResponse<T>
            {
                Success = true,
                ErrorCode = HttpStatusCode.OK,
                Message = message,
                Data = data
            };
        }

        public static PagedResponse<T> BadRequest(string message)
        {
            return new PagedResponse<T>
            {
                Success = false,
                ErrorCode = HttpStatusCode.BadRequest,
                Message = message
            };
        }

        public static PagedResponse<T> NotFound(string message)
        {
            return new PagedResponse<T>
            {
                Success = false,
                ErrorCode = HttpStatusCode.NotFound,
                Message = message
            };
        }

        public static PagedResponse<T> ErrorProcessData(string[] messages, string message = "")
        {
            return new PagedResponse<T>
            {
                Success = false,
                ErrorCode = HttpStatusCode.InternalServerError,
                Messages = messages,
                Message = message
            };
        }
    }
}

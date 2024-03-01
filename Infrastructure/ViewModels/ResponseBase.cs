using MusicWebAppBackend.Infrastructure.Models.Paging;
using System.Net;

namespace MusicWebAppBackend.Infrastructure.ViewModels
{
    public class ResponseBase
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string[]? Messages { get; set; }
        public HttpStatusCode ErrorCode { get; set; }

        public static T BadRequest<T>(string message) where T : ResponseBase, new()
        {
            return new T { Success = false, ErrorCode = HttpStatusCode.BadRequest, Message = message };
        }

        public static T ErrorProcessData<T>(string[] messages, string message = "") where T : ResponseBase, new()
        {
            return new T { Success = false, ErrorCode = HttpStatusCode.InternalServerError, Messages = messages, Message = message };
        }

        public static T OK<T>(string message) where T : ResponseBase, new()
        {
            return new T { Success = true, ErrorCode = HttpStatusCode.OK, Message = message };
        }

        public static T NotFound<T>(string message) where T : ResponseBase, new()
        {
            return new T { Success = false, ErrorCode = HttpStatusCode.NotFound, Message = message };
        }
    }

    public static class ResponseBaseExtensions
    {
        public static T ToChild<T>(this ResponseBase model) where T : ResponseBase, new()
        {
            return new T { Success = model.Success, ErrorCode = model.ErrorCode, Message = model.Message };
        }
    }
}

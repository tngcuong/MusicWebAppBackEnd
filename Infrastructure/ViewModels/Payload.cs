using System.Net;

namespace MusicWebAppBackend.Infrastructure.ViewModels
{
    public class Payload<T> 
    {
        public Payload(T? content = default, HttpStatusCode errCode = 0, string errMsg = "")
        {
            if (content == null && errMsg == null)
                throw new Exception($"At least {nameof(content)} or error message should has value");

            Content = content;
            Message = errMsg;
            ErrorCode = errCode;
            Success = content != null;
        }

        public bool Success { get; }
        public string Message { get; }
        public HttpStatusCode ErrorCode { get; }
        public T? Content { get; }

        public static Payload<T> NotFound(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.NotFound , string.IsNullOrEmpty(message) ? "Item not found!" : message);
        }

        public static Payload<T> BadRequest(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.BadRequest, string.IsNullOrEmpty(message) ? "Bad Request!" : message);
        }

        public static Payload<T> Dublicated(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.Forbidden, string.IsNullOrEmpty(message) ? "Duplicated data!" : message);
        }

        public static Payload<T> NoContent(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.NoContent, string.IsNullOrEmpty(message) ? "Have no data!" : message);
        }

        public static Payload<T> Successfully(T data, string message = "")
        {
            return new Payload<T>(data, HttpStatusCode.OK, string.IsNullOrEmpty(message) ? "OK" : message);
        }

        public static Payload<List<T>> SuccessfullyLists(List<T> newUser)
        {
            return new Payload<List<T>>(newUser, HttpStatusCode.OK, "OK");
        }

        //     public static Payload<List<object>> SuccessViewData(List<object> newUser, int page, int pageSize, int totalCount, int totalPage)
        //     {
        //ViewData data = new ViewData(newUser, page, pageSize, totalCount, totalPage);
        //         return new Payload<List<object>>(data);
        //     }

        public static Payload<T> CreatedFail(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.InternalServerError, string.IsNullOrEmpty(message) ? "Created Fail!" : message);
        }

        public static Payload<T> UpdatedFail(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.InternalServerError, string.IsNullOrEmpty(message) ? "Updated Fail!" : message);
        }

        public static Payload<T> DeletedFail(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.InternalServerError, string.IsNullOrEmpty(message) ? "Deleted Fail!" : message);
        }

        public static Payload<T> RequestInvalid(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.Forbidden, string.IsNullOrEmpty(message) ? "Request Invalid!" : message);
        }
        public static Payload<T> ErrorInProcessing(string message = "")
        {
            return new Payload<T>(default, HttpStatusCode.InternalServerError, string.IsNullOrEmpty(message) ? "Error in processing!" : message);
        }
    }
}

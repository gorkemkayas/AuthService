namespace AuthService.API.Models
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }

        protected ApiResult(bool success, string? message = null, string? errorCode = null)
        {
            Success = success;
            Message = message;
            ErrorCode = errorCode;
        }

        public static ApiResult Ok(string? message = null)
            => new ApiResult(true, message);

        public static ApiResult Fail(string message, string errorCode)
            => new ApiResult(false, message, errorCode);
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        private ApiResult(T data, string? message)
            : base(true, message)
        {
            Data = data;
        }

        private ApiResult(string message, string errorCode)
            : base(false, message, errorCode)
        {
        }

        public static ApiResult<T> Ok(T data, string? message = null)
            => new ApiResult<T>(data, message);

        public static ApiResult<T> Fail(string message, string errorCode)
            => new ApiResult<T>(message, errorCode);
    }


}

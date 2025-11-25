namespace AuthService.Application.Results
{
    public class ServiceResult
    {
        public bool Success { get; protected set; }
        public string? Message { get; protected set; }
        public string? ErrorCode { get; protected set; }
        public static ServiceResult Ok(string message = "Operation completed successfully.")
        => new ServiceResult { Success = true, Message = message, ErrorCode = null };
        public static ServiceResult Fail(string message, string? errorCode = null)
        => new ServiceResult { Success = false, Message = message, ErrorCode = errorCode };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }
        public static ServiceResult<T> Ok(T data, string message = "Operation completed successfully.")
        => new ServiceResult<T> { Success = true, Data = data, Message = message, ErrorCode = null };
        public static new ServiceResult<T> Fail(string message, string? errorCode = null)
        => new ServiceResult<T> { Success = false, Data = default, Message = message, ErrorCode = errorCode };
    }
}

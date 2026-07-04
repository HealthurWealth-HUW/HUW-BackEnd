namespace HealthUrWealth.Api.Common
{
    public sealed class ApiError
    {
        public string? Field { get; init; }
        public string Message { get; init; } = string.Empty;

        public ApiError() { }

        public ApiError(string message, string? field = null)
        {
            Message = message;
            Field = field;
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public List<ApiError> Errors { get; init; } = new();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string? TraceId { get; init; }

        public static ApiResponse<T> Ok(
            T data,
            string message = "Request successful.",
            int statusCode = 200,
            string? traceId = null)
            => new()
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = new(),
                TraceId = traceId
            };

        public static ApiResponse<T> Fail(
            string message,
            int statusCode = 400,
            List<ApiError>? errors = null,
            string? traceId = null)
            => new()
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors ?? new(),
                TraceId = traceId
            };
    }
}

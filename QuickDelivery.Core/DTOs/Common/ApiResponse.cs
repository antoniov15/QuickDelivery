using System;
using System.Collections.Generic;

namespace QuickDelivery.Core.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int StatusCode { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default(T),
                Message = message,
                Errors = errors,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResult(string message, string error, int statusCode = 400)
        {
            return ErrorResult(message, new List<string> { error }, statusCode);
        }
    }

    // Non-generic version for situations where no data is returned
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResult(string message = "Operation completed successfully")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                StatusCode = 200,
                Timestamp = DateTime.UtcNow
            };
        }

        public static new ApiResponse ErrorResult(string message, List<string>? errors = null, int statusCode = 400)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow
            };
        }

        public static new ApiResponse ErrorResult(string message, string error, int statusCode = 400)
        {
            return ErrorResult(message, new List<string> { error }, statusCode);
        }
    }
}
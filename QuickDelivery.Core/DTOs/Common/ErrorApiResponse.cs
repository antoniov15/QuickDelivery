using System;

namespace QuickDelivery.Core.DTOs.Common
{
    public class ErrorApiResponse
    {
        public bool Success { get; set; } = false;
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? DeveloperMessage { get; set; }
        public string? TraceId { get; set; }
    }
}
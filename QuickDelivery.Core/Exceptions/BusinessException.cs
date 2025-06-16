namespace QuickDelivery.Core.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message, object? details = null)
            : base(400, "BUSINESS_ERROR", message, details)
        {
        }

        public BusinessException(string message, Exception innerException, object? details = null)
            : base(400, "BUSINESS_ERROR", message, details, innerException)
        {
        }
    }
}
namespace QuickDelivery.Core.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "Access forbidden.")
            : base(403, "FORBIDDEN", message)
        {
        }
    }
}
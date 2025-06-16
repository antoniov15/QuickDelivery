namespace QuickDelivery.Core.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(401, "UNAUTHORIZED", message)
        {
        }
    }
}
namespace QuickDelivery.Core.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string resource, object identifier)
            : base(404, "NOT_FOUND", $"{resource} with identifier '{identifier}' was not found.", new { Resource = resource, Identifier = identifier })
        {
        }

        public NotFoundException(string message)
            : base(404, "NOT_FOUND", message)
        {
        }
    }
}
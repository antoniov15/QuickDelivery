using System.Collections.Generic;

namespace QuickDelivery.Core.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(string message, IEnumerable<string>? errors = null)
            : base(400, "VALIDATION_ERROR", message, errors)
        {
        }

        public ValidationException(IEnumerable<string> errors)
            : base(400, "VALIDATION_ERROR", "One or more validation errors occurred.", errors)
        {
        }
    }
}
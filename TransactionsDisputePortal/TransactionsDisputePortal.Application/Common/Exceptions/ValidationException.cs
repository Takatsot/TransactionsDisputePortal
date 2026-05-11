using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule or validation is violated
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

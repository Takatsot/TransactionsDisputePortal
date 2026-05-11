using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested entity is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"{name} with key '{key}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}

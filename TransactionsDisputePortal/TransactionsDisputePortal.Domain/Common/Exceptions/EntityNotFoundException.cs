using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Domain.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when an entity is not found
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, object key) 
            : base($"{entityName} with key '{key}' was not found.")
        {
            EntityName = entityName;
            Key = key;
        }

        public string EntityName { get; }
        public object Key { get; }
    }
}

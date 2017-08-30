using System;

namespace TT.Domain.CreationPolices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
        Inherited = false, AllowMultiple = false)]
    public sealed class CreationPolicyAttribute : Attribute
    {
        public CreationPolicy Policy { get; }

        public CreationPolicyAttribute(CreationPolicy policy)
        {
            this.Policy = policy;
        }
    }
}
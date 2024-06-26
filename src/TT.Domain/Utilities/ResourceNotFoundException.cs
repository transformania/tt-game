using System;
using System.Reflection;

namespace TT.Domain.Utilities
{
    public class ResourceNotFoundException : Exception
    {
        public Assembly Assembly { get; private set; }

        public string ResourceName { get; private set; }

        public ResourceNotFoundException(Assembly assembly, string resourceName) 
            : this($"{resourceName} not found in {assembly.FullName}", assembly, resourceName)
        {}

        public ResourceNotFoundException(string message, Assembly assembly, string resourceName) : base(message)
        {
            Assembly = assembly;
            ResourceName = resourceName;
        }

        public ResourceNotFoundException(string message, Exception inner, Assembly assembly, string resourceName) : base(message, inner)
        {
            Assembly = assembly;
            ResourceName = resourceName;
        }
    }
}
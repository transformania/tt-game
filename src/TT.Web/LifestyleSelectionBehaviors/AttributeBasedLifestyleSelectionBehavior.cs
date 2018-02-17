using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using SimpleInjector;
using System.Reflection;
using TT.Domain.CreationPolices;

namespace TT.Web.LifestyleSelectionBehaviors
{
    public class AttributeBasedLifestyleSelectionBehavior : ILifestyleSelectionBehavior
    {
        private readonly IDictionary<Type, CreationPolicy> defaultLifestyles;
        private const CreationPolicy defaultPolicy = CreationPolicy.Transient;

        public AttributeBasedLifestyleSelectionBehavior() : this(new Dictionary<IEnumerable<Type>, CreationPolicy>())
        {
        }

        public AttributeBasedLifestyleSelectionBehavior(IDictionary<IEnumerable<Type>, CreationPolicy> defaultLifestyles)
        {
            var dict = new Dictionary<Type, CreationPolicy>();

            foreach (var kvp in defaultLifestyles)
            {
                foreach (var key in kvp.Key)
                {
                    dict.Add(key, kvp.Value);
                }
            }

            this.defaultLifestyles = dict;
        }

        public Lifestyle SelectLifestyle(Type implementationType)
        {
            return GetPolicy(implementationType).ToLifestyle();
        }

        private CreationPolicy GetPolicy(Type type)
        {
            CreationPolicy typeDefaultLifestyle;

            return type.GetCustomAttribute<CreationPolicyAttribute>()?.Policy ?? (defaultLifestyles.TryGetValue(type, out typeDefaultLifestyle) ? typeDefaultLifestyle : defaultPolicy);
        }
    }
}
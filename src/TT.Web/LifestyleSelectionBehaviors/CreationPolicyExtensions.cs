using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TT.Domain.CreationPolices;

namespace TT.Web.LifestyleSelectionBehaviors
{
    public static class CreationPolicyExtensions
    {
        public static Lifestyle ToLifestyle(this CreationPolicy policy) =>
            policy == CreationPolicy.Singleton ? Lifestyle.Singleton :
            policy == CreationPolicy.Scoped ? Lifestyle.Scoped :
            Lifestyle.Transient;
    }
}
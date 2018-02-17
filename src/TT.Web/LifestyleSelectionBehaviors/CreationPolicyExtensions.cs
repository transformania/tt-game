using SimpleInjector;
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
using TT.Domain.Abstract;

namespace TT.Domain
{
    /// <summary>
    /// The DomainRegistry forms a top-level boundary for interacting with entities by 
    /// hosting our Root object and any other cross-domain objects as required in future.
    /// </summary>
    public static class DomainRegistry
    {
        public static IRoot Root { get; set; }
        public static IDomainRepository Repository { get; set; }
    }
}
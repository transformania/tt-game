using System;
using TT.Domain.Abstract;

namespace TT.Domain
{
    /// <summary>
    /// The DomainRegistry forms a top-level boundary for interacting with entities by 
    /// hosting our Root object and any other cross-domain objects as required in future.
    /// </summary>
    public static class DomainRegistry
    {
        [ThreadStatic]
        private static IRoot _root;

        [ThreadStatic]
        private static IDomainRepository _repository;

        public static IRoot Root
        {
            get { return _root ?? (_root = new Root()); }
            set { _root = value; }
        }
        
        public static IDomainRepository Repository
        {
            get { return _repository ?? (_repository = new DomainRepository(new DomainContext())); }
            set { _repository = value; }
        }
    }
}
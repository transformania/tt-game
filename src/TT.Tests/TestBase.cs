using Highway.Data;
using NUnit.Framework;
using TT.Domain;

namespace TT.Tests
{
    public class TestBase
    {
        protected IDataContext DataContext { get; private set; }
        protected IDomainRepository Repository { get; private set; }
        
        [SetUp]
        public virtual void SetUp()
        {
            DataContext = new InMemoryDataContextWithGenerator();
            Repository = new DomainRepository(DataContext);

            DomainRegistry.Repository = Repository;
            DomainRegistry.ConfigureMapper();
        }
    }
}
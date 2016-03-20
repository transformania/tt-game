using Highway.Data;
using Highway.Data.Contexts;
using NUnit.Framework;
using TT.Domain;

namespace TT.Tests
{
    public class TestBase
    {
        protected IDataContext DataContext { get; private set; }
        protected IDomainRepository Repository { get; private set; }
        
        [SetUp]
        public void SetUp()
        {
            DataContext = new InMemoryDataContext();
            Repository = new DomainRepository(DataContext);

            DomainRegistry.Repository = Repository;
        }
    }
}
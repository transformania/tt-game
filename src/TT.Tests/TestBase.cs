using AutoMapper;
using Highway.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TT.Domain;
using TT.Tests.Utilities;

namespace TT.Tests
{
    [TestFixture]
    public class TestBase
    {
        protected IDataContext DataContext { get; private set; }
        protected IDomainRepository Repository { get; private set; }
        protected IMapper Mapper { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            DataContext = new InMemoryDataContextWithGenerator();
            Repository = new DomainRepository(DataContext);

            DomainRegistry.Repository = Repository;

            var mapper = GetMapper();
            Mapper = mapper;
            DomainRegistry.SetMapperFunc(() => mapper);
        }

        private static IMapper GetMapper()
        {
            return new MapBuilder()
                .BuildMapper();
        }
    }
}
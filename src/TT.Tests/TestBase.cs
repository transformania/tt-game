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

        public virtual IMapper GetMapper()
        {
            return new MapBuilder()
                .AddAssemblies(GetMapperAssemblies())
                .AddProfileTypes(GetMapperProfileTyes())
                .AddProfileInstances(GetMapperProfileInstances())
                .BuildMapper();
        }

        public virtual IEnumerable<Assembly> GetMapperAssemblies()
        {
            return new List<Assembly> { typeof(DomainRegistry).Assembly };
        }

        public virtual IEnumerable<Type> GetMapperProfileTyes()
        {
            return new List<Type>() { };
        }

        public virtual IEnumerable<Profile> GetMapperProfileInstances()
        {
            return new List<Profile>() { };
        }
    }
}
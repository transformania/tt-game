using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;

namespace TT.Tests.Domain.Mapper
{
    [TestFixture]
    public class AutoMapperTests : TestBase
    {
        [Test]
        public void should_not_throw_error_if_mappings_valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(DomainRegistry).Assembly);
            });

            var action = new Action(() => { mapperConfiguration.AssertConfigurationIsValid(); });

            action.ShouldNotThrow();
        }
    }
}

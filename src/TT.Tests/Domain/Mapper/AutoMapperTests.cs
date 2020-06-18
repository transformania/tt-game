using AutoMapper;
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
            Assert.That(() => Mapper.ConfigurationProvider.AssertConfigurationIsValid(), Throws.Nothing);
        }
    }
}

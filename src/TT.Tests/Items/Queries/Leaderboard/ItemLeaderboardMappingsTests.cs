using AutoMapper;
using NUnit.Framework;
using TT.Domain.Items.Queries.Leaderboard;
using TT.Tests.Utilities;

namespace TT.Tests.Items.Queries.Leaderboard
{
    [TestFixture]
    public class ItemLeaderboardMappingsTests : TestBase
    {
        public override IMapper GetMapper()
        {
            return new MapBuilder()
                .AddProfileInstances(new ItemLeaderboardMappings())
                .BuildMapper();
        }

        [Test]
        public void MappingsAreValid()
        {
            Assert.That(() => Mapper.ConfigurationProvider.AssertConfigurationIsValid(), Throws.Nothing);
        }
    }
}

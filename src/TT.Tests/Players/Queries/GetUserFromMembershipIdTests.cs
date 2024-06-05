using NUnit.Framework;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class GetUserFromMembershipIdTests : TestBase
    {
        [Test]
        public void Should_not_return_null()
        {
            var user = new UserBuilder()
                .With(p => p.Id, "guid")
                .BuildAndSave();

            var cmd = new GetUserFromMembershipId { UserId = user.Id };
            Assert.IsNotNull(DomainRegistry.Repository.FindSingle(cmd));
        }
    }
}

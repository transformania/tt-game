using NUnit.Framework;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class GetUserFromUsernameTests : TestBase
    {
        [Test]
        public void Should_not_return_null()
        {
            var user = new UserBuilder()
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            var cmd = new GetUserFromUsername { Username = user.UserName };
            Assert.IsNotNull(DomainRegistry.Repository.FindSingle(cmd));
        }
    }
}

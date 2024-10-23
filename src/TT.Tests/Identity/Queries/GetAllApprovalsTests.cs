using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class GetAllApprovalsTests : TestBase
    {
        [Test]
        public void should_get_users_with_approval()
        {
            // approved
            new UserBuilder()
                .With(u => u.Approved, true)
                .BuildAndSave();

            // not approved
            new UserBuilder()
                .With(u => u.Approved, false)
                .BuildAndSave();

            var users = DomainRegistry.Repository.Find(new GetAllApprovals()).ToArray();

            Assert.That(users[0].Approved, Is.EqualTo(true));
        }

        public void should_get_users_without_approval()
        {
            // approved
            new UserBuilder()
                .With(u => u.Approved, true)
                .BuildAndSave();

            // not approved
            new UserBuilder()
                .With(u => u.Approved, false)
                .BuildAndSave();

            var users = DomainRegistry.Repository.Find(new GetAllApprovals()).ToArray();

            Assert.That(users[1].Approved, Is.EqualTo(false));
        }
    }
}

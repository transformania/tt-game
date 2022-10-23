using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class IsOwnershipVisibilityAllowedTests : TestBase
    {
        [Test]
        public void return_true_allow_ownership_visibility()
        {
            var user = new UserBuilder()
                .With(u => u.AllowOwnershipVisibility, true)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsOwnershipVisibilityEnabled { UserId = user.Id }), Is.True);
        }

        [Test]
        public void return_false_allow_ownership_visibility()
        {
            var user = new UserBuilder()
                .With(u => u.AllowOwnershipVisibility, false)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsOwnershipVisibilityEnabled { UserId = user.Id }), Is.False);
        }

        [Test]
        public void throw_exception_if_userid_doesnt_exist()
        {
            var cmd = new IsOwnershipVisibilityEnabled { UserId = "abcde" };
            Assert.That(() => Repository.FindSingle(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id abcde does not exist"));
        }
    }
}

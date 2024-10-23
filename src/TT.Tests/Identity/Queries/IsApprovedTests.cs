using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class IsApprovedTests : TestBase
    {
        [Test]
        public void return_true_approve_toggle()
        {
            var user = new UserBuilder()
                .With(u => u.Approved, true)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsApproved { UserId = user.Id }), Is.True);
        }

        [Test]
        public void return_false_approve_toggle()
        {
            var user = new UserBuilder()
                .With(u => u.Approved, false)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsApproved { UserId = user.Id }), Is.False);
        }

        [Test]
        public void throw_exception_if_userid_doesnt_exist()
        {
            var cmd = new IsOnlineToggled { UserId = "abcde" };
            Assert.That(() => Repository.FindSingle(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id abcde does not exist"));
        }
    }
}

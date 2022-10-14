using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class IsPvPLockedTests : TestBase
    {
        [Test]
        public void return_true_pvp_lock()
        {
            var user = new UserBuilder()
                .With(u => u.PvPLock, true)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsPvPLocked { UserId = user.Id }), Is.True);
        }

        [Test]
        public void return_false_pvp_lock()
        {
            var user = new UserBuilder()
                .With(u => u.PvPLock, false)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsPvPLocked { UserId = user.Id }), Is.False);
        }

        [Test]
        public void throw_exception_if_userid_doesnt_exist()
        {
            var cmd = new IsPvPLocked { UserId = "abcde" };
            Assert.That(() => Repository.FindSingle(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id abcde does not exist"));
        }
    }
}

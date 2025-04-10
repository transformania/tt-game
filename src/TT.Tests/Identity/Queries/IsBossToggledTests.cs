using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class IsBossDisabledTests : TestBase
    {
        [Test]
        public void return_true_boss_toggle()
        {
            var user = new UserBuilder()
                .With(u => u.BossDisable, true)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsBossDisabled { UserId = user.Id }), Is.True);
        }

        [Test]
        public void return_false_boss_toggle()
        {
            var user = new UserBuilder()
                .With(u => u.BossDisable, false)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new IsBossDisabled { UserId = user.Id }), Is.False);
        }

        [Test]
        public void throw_exception_if_userid_doesnt_exist()
        {
            var cmd = new IsBossDisabled { UserId = "abcde" };
            Assert.That(() => Repository.FindSingle(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id abcde does not exist"));
        }
    }
}

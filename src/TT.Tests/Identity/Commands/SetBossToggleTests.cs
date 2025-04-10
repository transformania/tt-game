using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class SetBossDisableTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_boss_toggle(bool setBossDisable)
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.BossDisable, !setBossDisable)
                .BuildAndSave();

            var cmd = new SetBossDisable { UserId = "user", BossDisable = setBossDisable };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<User>().First().BossDisable, Is.EqualTo(setBossDisable));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            new UserBuilder()
                .With(p => p.Id, "user")
                .BuildAndSave();

            var cmd = new SetBossDisable { UserId = "fake", BossDisable = true };
            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id 'fake' could not be found"));
        }

    }
}

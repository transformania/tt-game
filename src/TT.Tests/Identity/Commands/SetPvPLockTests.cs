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
    public class SetPvPLockTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_pvp_lock(bool setPvPLock)
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.PvPLock, !setPvPLock)
                .BuildAndSave();

            var cmd = new SetPvPLock { UserId = "user", PvPLock = setPvPLock };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<User>().First().PvPLock, Is.EqualTo(setPvPLock));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            new UserBuilder()
                .With(p => p.Id, "user")
                .BuildAndSave();

            var cmd = new SetPvPLock { UserId = "fake", PvPLock = true };
            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id 'fake' could not be found"));
        }

    }
}

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
    public class AllowChaosChangesTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_allow_chaos_changes(bool allowChanges)
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.AllowChaosChanges, !allowChanges)
                .BuildAndSave();

            var cmd = new AllowChaosChanges {UserId = "user", ChaosChangesEnabled = allowChanges};
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<User>().First().AllowChaosChanges, Is.EqualTo(allowChanges));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            new UserBuilder()
                .With(p => p.Id, "user")
                .BuildAndSave();

            var cmd = new AllowChaosChanges { UserId = "fake", ChaosChangesEnabled = true };
            Assert.That(()=> Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id 'fake' could not be found"));
        }

    }
}

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
    public class SetOwnershipVisibilityTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_ownership_visibility(bool setOwnershipVisibility)
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.AllowOwnershipVisibility, !setOwnershipVisibility)
                .BuildAndSave();

            var cmd = new SetOwnershipVisibility { UserId = "user", OwnershipVisibilityEnabled = setOwnershipVisibility };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<User>().First().AllowOwnershipVisibility, Is.EqualTo(setOwnershipVisibility));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            new UserBuilder()
                .With(p => p.Id, "user")
                .BuildAndSave();

            var cmd = new SetOwnershipVisibility { UserId = "fake", OwnershipVisibilityEnabled = true };
            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id 'fake' could not be found"));
        }

    }
}

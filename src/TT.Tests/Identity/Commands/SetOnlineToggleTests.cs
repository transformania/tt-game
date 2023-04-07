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
    public class SetOnlineToggleTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_online_toggle(bool setOnlineToggle)
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.OnlineToggle, !setOnlineToggle)
                .BuildAndSave();

            var cmd = new SetOnlineToggle { UserId = "user", OnlineToggle = setOnlineToggle };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<User>().First().OnlineToggle, Is.EqualTo(setOnlineToggle));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            new UserBuilder()
                .With(p => p.Id, "user")
                .BuildAndSave();

            var cmd = new SetOnlineToggle { UserId = "fake", OnlineToggle = true };
            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id 'fake' could not be found"));
        }

    }
}

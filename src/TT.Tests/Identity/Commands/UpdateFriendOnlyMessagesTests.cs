using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class UpdateFriendOnlyMessagesTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_friend_only_messages(bool setFriendOnlyMessages)
        {
            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "100").BuildAndSave())
                .With(p => p.FriendOnlyMessages, false)
                .BuildAndSave();

            var cmd = new UpdateFriendOnlyMessages { UserId = "100", FriendOnlyMessages = setFriendOnlyMessages };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "100").FriendOnlyMessages, Is.EqualTo(setFriendOnlyMessages));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "100").BuildAndSave())
                .With(p => p.FriendOnlyMessages, false)
                .BuildAndSave();

            var cmd = new UpdateFriendOnlyMessages { UserId = "200", FriendOnlyMessages = true };
            Assert.That(()=> Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id '200' could not be found"));
        }

    }
}

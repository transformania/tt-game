using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{
    [TestFixture()]
    public class ChangeBossEnableAfterDefeatTests : TestBase
    {
        [Test]
        public void should_put_player_into_true_when_not()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.BossEnableAfterDefeat, false)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeBossEnableAfterDefeat
                { MembershipId = player.User.Id, BossEnableAfterDefeat = true}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").BossEnableAfterDefeat, Is.True);
        }

        [Test]
        public void should_put_player_into_false_when_in()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.BossEnableAfterDefeat, true)
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeBossEnableAfterDefeat
                { MembershipId = player.User.Id, BossEnableAfterDefeat = false}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").BossEnableAfterDefeat, Is.False);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeBossEnableAfterDefeat { MembershipId = "fake", BossEnableAfterDefeat = true};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with MembershipID 'fake' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeBossEnableAfterDefeat { MembershipId = null, BossEnableAfterDefeat = true };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("MembershipID is required!"));
        }
    }
}

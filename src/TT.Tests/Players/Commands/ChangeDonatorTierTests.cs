using System.Collections.Generic;
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
    [TestFixture]
    public class ChangeDonatorTierTests : TestBase
    {
        [Test]
        public void Should_change_player_donator_tier_with_friendly_message()
        {
            var player = new PlayerBuilder()
                .With(n => n.Id, 1)
                .With( p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .BuildAndSave();

            var cmd = new ChangeDonatorTier{ UserId = player.User.Id, Tier = 3};

            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var changedPlayer = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);

            Assert.That(changedPlayer.DonatorLevel, Is.EqualTo(3));
            Assert.That(changedPlayer.PlayerLogs.First().Message,
                Is.EqualTo(
                    "<b>An admin has set your donator status to Tier 3.  <span class='good'>Thank you for supporting Transformania Time!</span></b>"));
        }

        [Test]
        public void Should_change_player_donator_tier_with_default_message()
        {
            var player = new PlayerBuilder()
                .With(n => n.Id, 1)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .BuildAndSave();

            var cmd = new ChangeDonatorTier { UserId = player.User.Id, Tier = 0 };

            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var changedPlayer = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);

            Assert.That(changedPlayer.DonatorLevel, Is.EqualTo(0));
            Assert.That(changedPlayer.PlayerLogs.First().Message,
                Is.EqualTo("<b>An admin has set your donator status to Tier 0.</b>"));
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeDonatorTier { UserId = "fakeuser", Tier = 0 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with user ID 'fakeuser' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_user_not_provided()
        {
            var cmd = new ChangeDonatorTier { UserId = null, Tier = 0 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("userId is required"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(4)]
        public void Should_throw_exception_if_tier_out_of_bounds(int tier)
        {
            var cmd = new ChangeDonatorTier { UserId = "user", Tier = tier };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Tier must be an integer between 0 and 3."));
        }
    }
}

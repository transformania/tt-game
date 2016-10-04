using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Players;
using TT.Domain.Entities.Players;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Players
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

            DomainRegistry.Repository.Execute(cmd);

            var changedPlayer = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);

            changedPlayer.DonatorLevel.Should().Be(3);
            changedPlayer.PlayerLogs.First().Message.Should().Be("<b>An admin has set your donator status to Tier 3.  <span class='good'>Thank you for supporting Transformania Time!</span></b>");

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

            DomainRegistry.Repository.Execute(cmd);

            var changedPlayer = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);

            changedPlayer.DonatorLevel.Should().Be(0);
            changedPlayer.PlayerLogs.First().Message.Should().Be("<b>An admin has set your donator status to Tier 0.</b>");

        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeDonatorTier { UserId = "fakeuser", Tier = 0 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with user ID 'fakeuser' could not be found");
        }

        [Test]
        public void Should_throw_exception_if_user_not_provided()
        {
            var cmd = new ChangeDonatorTier { UserId = null, Tier = 0 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("userId is required");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(4)]
        public void Should_throw_exception_if_tier_out_of_bounds(int tier)
        {
            var cmd = new ChangeDonatorTier { UserId = "user", Tier = tier };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Tier must be an integer between 0 and 3.");
        }
    }
}

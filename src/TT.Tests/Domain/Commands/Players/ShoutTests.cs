using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Players;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Entities.Players;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Players
{
    [TestFixture]
    public class ShoutTests : TestBase
    {
        [Test]
        public void should_shout()
        {
            var player = new PlayerBuilder()
               .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .BuildAndSave())
               .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
               .With(p => p.ShoutsRemaining, 1)
               .BuildAndSave();

            DomainRegistry.Repository.Execute(new Shout {Message = "Hello world!", UserId = player.User.Id});

            var playerLoaded = DataContext.AsQueryable<Player>().First();

            playerLoaded.ShoutsRemaining.Should().Be(0);
            playerLoaded.PlayerLogs.First().Message.Should().Be("You shouted 'Hello world!' at Street: 200 Main Street.");

            var locationLog = DataContext.AsQueryable<LocationLog>().First();
            locationLog.Message.Should().Be("<span class='playerShoutNotification'>John Doe shouted <b>\"Hello world!\"</b> here.</span>");
        }

        [Test]
        public void should_throw_exception_if_user_not_provided()
        {
            var cmd = new Shout { Message = "Hello world!", UserId = null };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("userId is required");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void should_throw_exception_if_message_not_provided(string shout)
        {
            var cmd = new Shout { Message = shout, UserId = "abcde" };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("A shout message is required");
        }

        [Test]
        public void should_throw_exception_if_message_too_long()
        {
            var cmd = new Shout { Message = "Yes this is a very long shout, too long for someone to reasonably want to say as a shout and stuff whoooo", UserId = "abcde" };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("A shout must contain 100 characters or fewer.");
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new Shout { Message = "Hello world!", UserId = "abcde" };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with user ID 'abcde' could not be found");
        }

        [Test]
        public void should_throw_exception_if_player_already_shouted()
        {

            var player = new PlayerBuilder()
               .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .BuildAndSave())
               .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
               .With(p => p.ShoutsRemaining, 0)
               .BuildAndSave();

            var cmd = new Shout { Message = "Hello world!", UserId = player.User.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You can only shout once per turn.");
        }

        [Test]
        public void should_throw_exception_if_player_not_animate()
        {

            var player = new PlayerBuilder()
               .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .BuildAndSave())
               .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
               .With(p => p.Mobility, PvPStatics.MobilityInanimate)
               .BuildAndSave();

            var cmd = new Shout { Message = "Hello world!", UserId = player.User.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You must be animate in order to shout!");
        }

    }
}

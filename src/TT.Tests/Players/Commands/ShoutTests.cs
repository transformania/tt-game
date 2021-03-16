using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Exceptions;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{
    [TestFixture]
    public class ShoutTests : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CharacterPrankProcedures.HUSHED_EFFECT = 123;
        }

        [TearDown]
        public void TearDown()
        {
            CharacterPrankProcedures.HUSHED_EFFECT = null;
        }

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

            Assert.That(() => DomainRegistry.Repository.Execute(new Shout {Message = "Hello world!", UserId = player.User.Id}), Throws.Nothing);

            var playerLoaded = DataContext.AsQueryable<Player>().First();

            Assert.That(playerLoaded.ShoutsRemaining, Is.EqualTo(0));
            Assert.That(playerLoaded.PlayerLogs.First().Message,
                Is.EqualTo("You shouted 'Hello world!' at Street: 200 Main Street."));

            Assert.That(DataContext.AsQueryable<LocationLog>().First().Message,
                Is.EqualTo(
                    "<span class='playerShoutNotification'>John Doe shouted <b>\"Hello world!\"</b> here.</span>"));
        }

        [Test]
        public void should_throw_exception_if_user_not_provided()
        {
            var cmd = new Shout { Message = "Hello world!", UserId = null };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("userId is required"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void should_throw_exception_if_message_not_provided(string shout)
        {
            var cmd = new Shout { Message = shout, UserId = "abcde" };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("A shout message is required"));
        }

        [Test]
        public void should_throw_exception_if_message_too_long()
        {
            var cmd = new Shout { Message = "Yes this is a very long shout, too long for someone to reasonably want to say as a shout and stuff whoooo", UserId = "abcde" };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("A shout must contain 100 characters or fewer."));
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new Shout { Message = "Hello world!", UserId = "abcde" };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with user ID 'abcde' could not be found"));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You can only shout once per turn."));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You must be animate in order to shout!"));
        }
    }
}

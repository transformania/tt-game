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
    public class TakeBusTests : TestBase
    {
        private Player player;

        [SetUp]
        public void Init()
        {
            player = new PlayerBuilder()
                .With(p => p.FirstName, "John")
                .With(p => p.LastName, "Doe")
                .With(n => n.Id, 1)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.ActionPoints, 120)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();
        }


        [Test]
        [Ignore("Loading play buffs from the sql stored procedure does not work in testing.  Mock that method out?")]
        public void should_move_player_when_takes_bus()
        {
            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };

            DomainRegistry.Repository.Execute(cmd);

            player = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);
            player.Money.Should().Be(975);
            player.ActionPoints.Should().Be(117);
            player.Location.Should().Be(LocationsStatics.STREET_160_SUNNYGLADE_DRIVE);
            player.PlayerLogs.First().Message.Should().Be("You took the bus from <b>Street: 270 W. 9th Avenue</b> to <b>Street: 160 Sunnyglade Drive</b> for <b>25</b> Arpeyjis.");

            var originLog = DataContext.AsQueryable<LocationLog>().First(l => l.dbLocationName == LocationsStatics.STREET_270_WEST_9TH_AVE);
            var destinationLog = DataContext.AsQueryable<LocationLog>().First(l => l.dbLocationName == LocationsStatics.STREET_160_SUNNYGLADE_DRIVE);
            originLog.Message.Should().Be("John Doe got on a bus headed toward Street: 160 Sunnyglade Drive.");
            destinationLog.Message.Should().Be("John Doe arrived via bus from Street: 270 W. 9th Avenue.");

        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {

            var cmd = new TakeBus { playerId = 987, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with Id '987' could not be found");
        }

        [Test]
        public void Should_throw_exception_if_player_not_animate()
        {

           var player = new PlayerBuilder()
                .With(n => n.Id, 73472)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)

                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You must be animate in order to take the bus.");
        }

        [Test]
        public void Should_throw_exception_if_destination_not_provided()
        {

            var cmd = new TakeBus { playerId = 987 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Destination is required.");
        }

        [Test]
        public void Should_throw_exception_if_destination_is_same_as_origin()
        {

            var cmd = new TakeBus { playerId = player.Id, destination = player.Location};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You can't take the bus to the location you're already at.");
        }

        [Test]
        public void Should_throw_exception_if_player_recently_has_attacked()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 3)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddMinutes(-14))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You have been in combat too recently to take a bus.");
        }

        [Test]
        public void Should_throw_exception_if_player_recently_has_been_attacked()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 3)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatAttackedTimestamp, DateTime.UtcNow.AddMinutes(-14))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You have been in combat too recently to take a bus.");
        }

        [Test]
        public void Should_throw_exception_if_player_in_duel()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 4)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .With(p => p.InDuel, 5)
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You cannot take the bus whilst in a duel.");
        }

        [Test]
        public void Should_throw_exception_if_player_in_quest()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 5)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .With(p => p.InQuest, 5)
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You cannot take the bus whilst in a quest.");
        }

        [Test]
        public void Should_throw_exception_if_player_cannot_afford_ticket()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 6)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 3)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.ActionPoints, 120)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You can't afford this bus ticket!");
        }

        [Test]
        public void Should_throw_exception_if_player_not_starting_at_bus_stop()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 7)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, "college_sciences")
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You aren't at a valid bus stop.");
        }

        [Test]
        public void Should_throw_exception_if_player_not_going_to_bus_stop()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 8)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_160_SUNNYGLADE_DRIVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = "college_sciences" };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Your destination is not a valid bus stop.");
        }

        [Test]
        [TestCase(0)]
        [TestCase(2.99)]
        public void Should_throw_exception_if_player_has_insufficient_AP(decimal actionPoints)
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 236)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.ActionPoints, actionPoints)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_160_SUNNYGLADE_DRIVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_230_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You don't have enough AP to take the bus.");
        }

        [Test]
        [Ignore("Not sure how to give a mock player effects...")]
        public void Should_throw_exception_if_player_is_mind_controlled_with_forced_march()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 9)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                //.With(p => p.Effects, new EffectBuilder().BuildAndSave())
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You can't ride the bus while under the Forced March! mind control spell.");
        }

        [Test]
        [Ignore("Not sure how to test this")]
        public void Should_throw_exception_if_player_is_immobilized()
        {

            player = new PlayerBuilder()
                .With(n => n.Id, 10)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You can't ride the bus while immobilized.");
        }
    }
}

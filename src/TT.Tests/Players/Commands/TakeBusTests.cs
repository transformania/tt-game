﻿using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Effects;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
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
                .With(p => p.ActionPoints, TurnTimesStatics.GetActionPointReserveLimit())
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();
        }


        [Test]
        public void should_move_player_when_takes_bus()
        {
            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };

            DomainRegistry.Repository.Execute(cmd);

            player = DataContext.AsQueryable<Player>().First(p => p.Id == player.Id);
            Assert.That(player.Money, Is.EqualTo(970));
            Assert.That(player.ActionPoints, Is.EqualTo(TurnTimesStatics.GetActionPointReserveLimit() - 3));
            Assert.That(player.Location, Is.EqualTo(LocationsStatics.STREET_160_SUNNYGLADE_DRIVE));
            Assert.That(player.PlayerLogs.First().Message,
                Is.EqualTo(
                    "You took the bus from <b>Street: 270 W. 9th Avenue</b> to <b>Street: 160 Sunnyglade Drive</b> for <b>30</b> Arpeyjis."));

            Assert.That(
                DataContext.AsQueryable<LocationLog>()
                    .First(l => l.dbLocationName == LocationsStatics.STREET_270_WEST_9TH_AVE).Message,
                Is.EqualTo("John Doe got on a bus headed toward Street: 160 Sunnyglade Drive.")); // Moved from
            Assert.That(
                DataContext.AsQueryable<LocationLog>()
                    .First(l => l.dbLocationName == LocationsStatics.STREET_160_SUNNYGLADE_DRIVE).Message,
                Is.EqualTo("John Doe arrived via bus from Street: 270 W. 9th Avenue.")); // Moved to
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new TakeBus { playerId = 987, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with Id '987' could not be found"));
        }

        [Test]
        [TestCase(PvPStatics.MobilityInanimate)]
        [TestCase(PvPStatics.MobilityPet)]
        public void Should_throw_exception_if_player_not_animate(string mobility)
        {
           var inanimatePlayer = new PlayerBuilder()
                .With(n => n.Id, 73472)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, mobility)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)

                .BuildAndSave();

            var cmd = new TakeBus { playerId = inanimatePlayer.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You must be animate in order to take the bus."));
        }

        [Test]
        public void Should_throw_exception_if_destination_not_provided()
        {
            var cmd = new TakeBus { playerId = 987 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Destination is required."));
        }

        [Test]
        public void Should_throw_exception_if_destination_is_same_as_origin()
        {
            var cmd = new TakeBus { playerId = player.Id, destination = player.Location};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You can't take the bus to the location you're already at."));
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
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddMinutes(-11))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You have been in combat too recently to take a bus."));
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
                .With(p => p.LastCombatAttackedTimestamp, DateTime.UtcNow.AddMinutes(-11))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You have been in combat too recently to take a bus."));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You cannot take the bus whilst in a duel."));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You cannot take the bus whilst in a quest."));
        }

        [Test]
        public void Should_throw_exception_if_player_cannot_afford_ticket()
        {
            player = new PlayerBuilder()
                .With(n => n.Id, 6)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 3)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.ActionPoints, TurnTimesStatics.GetActionPointReserveLimit())
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You can't afford this bus ticket!"));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You aren't at a valid bus stop."));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Your destination is not a valid bus stop."));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You don't have enough AP to take the bus."));
        }

        [Test]
        public void Should_throw_exception_if_player_is_mind_controlled_with_forced_march()
        {
            player = new PlayerBuilder()
                .With(n => n.Id, 9)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var effectSourceMarch = new EffectSourceBuilder()
                .With(e => e.Id, MindControlStatics.MindControl__Movement_DebuffEffectSourceId)
                .BuildAndSave();

            var effectMarch = new EffectBuilder()
                .With(e => e.EffectSource, effectSourceMarch)
                .BuildAndSave();

            player.Effects.Add(effectMarch);

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You can't ride the bus while under the Forced March! mind control spell."));
        }

        [Test]
        public void Should_throw_exception_if_player_is_immobilized()
        {
            player = new PlayerBuilder()
                .With(n => n.Id, 10)
                .With(p => p.User, new UserBuilder().BuildAndSave())
                .With(p => p.Money, 1000)
                .With(p => p.MoveActionPointDiscount, -999)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var cmd = new TakeBus { playerId = player.Id, destination = LocationsStatics.STREET_160_SUNNYGLADE_DRIVE };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You can't ride the bus while immobilized."));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.TFEnergies.Entities;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.TFEnergies;

namespace TT.Tests.Players.Commands
{
    [TestFixture]
    public class CleanseTests : TestBase
    {
        private BuffBox buffs;

        [SetUp]
        public void Init()
        {
            buffs = new BuffBox();
        }

        [Test]
        public void should_cleanse_player()
        {

            var TFEnergies = new List<TFEnergy>
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var stats = new List<Stat>
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__BusRides).With(t => t.Amount, 20).BuildAndSave()
            };

            var player = new PlayerBuilder()
               .With(p => p.Id, 100)
               .With(p => p.Health, 0)
               .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .With(u => u.Id, "bob")
                    .BuildAndSave())
               .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
               .With(p => p.TFEnergies, TFEnergies)
               .BuildAndSave();

            Assert.That(() => DomainRegistry.Repository.Execute(new Cleanse {PlayerId = 100, Buffs = buffs}),
                Throws.Nothing);

            var playerLoaded = DataContext.AsQueryable<Player>().First();

            Assert.That(playerLoaded.PlayerLogs.First().Message,
                Is.EqualTo("You cleansed at Street: 200 Main Street."));
            Assert.That(playerLoaded.Health, Is.EqualTo(100));
            Assert.That(playerLoaded.LastActionTimestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);

            var locationLog = DataContext.AsQueryable<LocationLog>().First();
            Assert.That(locationLog.dbLocationName, Is.EqualTo(player.Location));
            Assert.That(locationLog.Message,
                Is.EqualTo("<span class='playerCleansingNotification'>John Doe cleansed here.</span>"));

            var stat = playerLoaded.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesCleansed);
            Assert.That(stat, Is.Not.Null);
            Assert.That(stat.Owner.Id, Is.EqualTo("bob"));
            Assert.That(stat.Amount, Is.EqualTo(1));
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 3, Buffs = buffs};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID '3' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_player_has_insufficient_AP()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.ActionPoints, PvPStatics.CleanseCost - 1)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You don't have enough action points to cleanse!"));
        }

        [Test]
        public void should_throw_exception_if_player_not_animate()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You must be animate in order to cleanse!"));
        }


        [Test]
        public void should_throw_exception_if_player_has_insufficient_mana()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.Mana, PvPStatics.CleanseManaCost - 1)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You don't have enough mana to cleanse!"));
        }

        [Test]
        public void should_throw_exception_if_player_has_cleansed_or_meditated_too_much()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.CleansesMeditatesThisRound, PvPStatics.MaxCleansesMeditatesPerUpdate)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You have cleansed and meditated the maximum number of times this update."));
        }

        [Test]
        public void should_throw_exception_if_player_id_not_provided()
        {

            var cmd = new Cleanse { Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player ID is required!"));
        }

        [Test]
        public void should_throw_exception_if_buffs_not_provided()
        {

            var cmd = new Cleanse { PlayerId = 100  };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Buffs are required!"));
        }

        [Test]
        public void should_skip_AP_validation_for_bot()
        {
            var TFEnergies = new List<TFEnergy>
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var stats = new List<Stat>
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__BusRides).BuildAndSave()
            };

            new PlayerBuilder()
               .With(p => p.Id, 100)
               .With(p => p.Level, 1)
               .With(p => p.Health, 0)
               .With(p => p.MaxHealth, 100)
               .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .With(u => u.Id, "bob")
                    .BuildAndSave())
               .With(p => p.Mobility, PvPStatics.MobilityFull)
               .With(p => p.ActionPoints, PvPStatics.CleanseCost  -1)
               .With(p => p.Mana, PvPStatics.CleanseManaCost - 1)
               .With(p => p.CleansesMeditatesThisRound, PvPStatics.MaxCleansesMeditatesPerUpdate)
               .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
               .With(p => p.TFEnergies, TFEnergies)
               .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new Cleanse {PlayerId = 100, Buffs = buffs, NoValidate = true}),
                Throws.Nothing);
            Assert.That(DataContext.AsQueryable<Player>().First().Health, Is.GreaterThan(0));
        }
    }
}

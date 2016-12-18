using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Players;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Entities.Players;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.TFEnergies;

namespace TT.Tests.Domain.Commands.Players
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

            var TFEnergies = new List<TFEnergy>()
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var stats = new List<Stat>()
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

            DomainRegistry.Repository.Execute(new Cleanse { PlayerId = 100, Buffs = buffs });

            var playerLoaded = DataContext.AsQueryable<Player>().First();

            playerLoaded.PlayerLogs.First().Message.Should().Be("You cleansed at Street: 200 Main Street.");
            playerLoaded.Health.Should().Be(8);

            var locationLog = DataContext.AsQueryable<LocationLog>().First();
            locationLog.dbLocationName.Should().Be(player.Location);
            locationLog.Message.Should().Be("<span class='playerCleansingNotification'>John Doe cleansed here.</span>");

            var stat = playerLoaded.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesCleansed);
            stat.Owner.Id.Should().Be("bob");
            stat.Amount.Should().Be(1);
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 3, Buffs = buffs};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with ID '3' could not be found");
        }

        [Test]
        public void should_throw_exception_if_player_has_insufficient_AP()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.ActionPoints, PvPStatics.CleanseCost - 1)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You don't have enough action points to cleanse!");
        }

        [Test]
        public void should_throw_exception_if_player_not_animate()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You must be animate in order to cleanse!");
        }


        [Test]
        public void should_throw_exception_if_player_has_insufficient_mana()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.Mana, PvPStatics.CleanseManaCost - 1)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You don't have enough mana to cleanse!");
        }

        [Test]
        public void should_throw_exception_if_player_has_cleansed_or_meditated_too_much()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.CleansesMeditatesThisRound, PvPStatics.MaxCleansesMeditatesPerUpdate)
                .BuildAndSave();

            var cmd = new Cleanse { PlayerId = 100, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You have cleansed and meditated the maximum number of times this update.");
        }

        [Test]
        public void should_throw_exception_if_player_id_not_provided()
        {

            var cmd = new Cleanse { Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player ID is required!");
        }

        [Test]
        public void should_throw_exception_if_buffs_not_provided()
        {

            var cmd = new Cleanse { PlayerId = 100  };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Buffs are required!");
        }

        [Test]
        public void should_skip_AP_validation_for_bot()
        {

            var TFEnergies = new List<TFEnergy>()
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var stats = new List<Stat>()
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

            DomainRegistry.Repository.Execute(new Cleanse { PlayerId = 100, Buffs = buffs, NoValidate = true});
            var playerLoaded = DataContext.AsQueryable<Player>().First();

            playerLoaded.Health.Should().BeGreaterThan(0);


        }
    }
}

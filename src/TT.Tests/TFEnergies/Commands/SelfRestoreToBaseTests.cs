using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Domain.TFEnergy.Commands;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.TFEnergies;

namespace TT.Tests.TFEnergies.Commands
{
    public class SelfRestoreToBaseTests : TestBase
    {

        private Player player;
        private BuffBox buffs;
        private FormSource originalForm;
        private FormSource currentForm;

        [SetUp]
        public void Init()
        {
            originalForm = new FormSourceBuilder()
                .With(n => n.Id, 1)
                .With(n => n.FriendlyName, "Base Form")
                .With(n => n.Gender, PvPStatics.GenderFemale)
                .BuildAndSave();

            currentForm = new FormSourceBuilder()
                .With(n => n.Id, 2)
                .With(n => n.FriendlyName, "Current Form")
                .With(n => n.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            player = new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.FormSource, currentForm)
                .With(p => p.OriginalFormSource, originalForm)
                .With(p => p.ActionPoints, 50)
                .With(p => p.Mana, 25)
                .BuildAndSave();

            buffs = new BuffBox();
        }

        [Test]
        public void should_fully_restore_player_to_base_when_has_enough()
        {

            var restoredPlayer = new PlayerBuilder()
                .With(p => p.Id, 35)
                .With(p => p.FormSource, currentForm)
                .With(p => p.OriginalFormSource, originalForm)
                .With(p => p.ActionPoints, 50)
                .With(p => p.Mana, 25)
                .With(p => p.LastActionTimestamp, DateTime.UtcNow.AddHours(-1))
                .With(p => p.SelfRestoreEnergy,
                    new SelfRestoreEnergyBuilder()
                    .With(s => s.Amount, 99)
                    .With(s => s.Timestamp, DateTime.UtcNow.AddHours(-3))
                    .BuildAndSave()
                    )
                .BuildAndSave();

            var cmd = new SelfRestoreToBase { PlayerId = restoredPlayer.Id, Buffs = buffs };

            DomainRegistry.Repository.Execute(cmd);

            var playerLoaded = DataContext.AsQueryable<Player>().Where(p => p.Id == restoredPlayer.Id).First();
            playerLoaded.Id.Should().Be(restoredPlayer.Id);
            playerLoaded.Gender.Should().Be(originalForm.Gender);
            playerLoaded.FormSource.Id.Should().Be(originalForm.Id);
            playerLoaded.FormSource.FriendlyName.Should().Be(originalForm.FriendlyName);
            playerLoaded.ActionPoints.Should().Be(47);
            playerLoaded.Mana.Should().Be(15);
            playerLoaded.CleansesMeditatesThisRound.Should().Be(1);
            playerLoaded.PlayerLogs.First().Message.Should().Be("<span class='meditate'>With this final cast, you manage to restore yourself back to your base form as a <b>Base Form</b>!<span>");
            playerLoaded.SelfRestoreEnergy.Amount.Should().Be(0);
            playerLoaded.SelfRestoreEnergy.Timestamp.Should().BeCloseTo(DateTime.UtcNow, 10000);
            playerLoaded.LastActionTimestamp.Should().BeCloseTo(DateTime.UtcNow, 10000);
        }

        [Test]
        public void should_add_energy_when_not_enough()
        {

            var insufficientEnergyPlayer = new PlayerBuilder()
                .With(p => p.Id, 74)
                .With(p => p.FormSource, currentForm)
                .With(p => p.OriginalFormSource, originalForm)
                .With(p => p.ActionPoints, 50)
                .With(p => p.Mana, 25)
                .With(p => p.LastActionTimestamp, DateTime.UtcNow.AddHours(-1))
                .BuildAndSave();

            var cmd = new SelfRestoreToBase { PlayerId = insufficientEnergyPlayer.Id, Buffs = buffs };

            DomainRegistry.Repository.Execute(cmd);

            var playerLoaded = DataContext.AsQueryable<Player>().Where(p => p.Id == insufficientEnergyPlayer.Id).First();
            playerLoaded.Id.Should().Be(insufficientEnergyPlayer.Id);
            playerLoaded.Gender.Should().Be(currentForm.Gender);
            playerLoaded.FormSource.Id.Should().Be(currentForm.Id);
            playerLoaded.FormSource.FriendlyName.Should().Be(currentForm.FriendlyName);
            playerLoaded.ActionPoints.Should().Be(47);
            playerLoaded.Mana.Should().Be(15);
            playerLoaded.CleansesMeditatesThisRound.Should().Be(1);
            playerLoaded.PlayerLogs.First().Message.Should().Be("You rest and attempt to restore yourself to your base form.  [+10, 10/100].  Keep trying and you'll find yourself in a familiar form in no time...");
            playerLoaded.SelfRestoreEnergy.Amount.Should().Be(10);
            playerLoaded.SelfRestoreEnergy.Timestamp.Should().BeCloseTo(DateTime.UtcNow, 10000);
            playerLoaded.LastActionTimestamp.Should().BeCloseTo(DateTime.UtcNow, 10000);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new SelfRestoreToBase { PlayerId = 999, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Player with ID 999 could not be found");
        }

        [Test]
        public void Should_throw_exception_if_buffs_not_provided()
        {
            var cmd = new SelfRestoreToBase { PlayerId = player.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Buffs are required.");
        }

        [Test]
        public void Should_throw_exception_if_player_not_enough_ap()
        {
            player = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.FormSource, currentForm)
                .With(p => p.OriginalFormSource, originalForm)
                .With(p => p.ActionPoints, 1)
                .With(p => p.Mana, 25)
                .BuildAndSave();

            var cmd = new SelfRestoreToBase { PlayerId = player.Id, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You don't have enough action points to do this.  You have 1 and need 3.");
        }

        [Test]
        public void Should_throw_exception_if_player_not_enough_mana()
        {
            player = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.FormSource, currentForm)
                .With(p => p.OriginalFormSource, originalForm)
                .With(p => p.ActionPoints, 100)
                .With(p => p.Mana, 1)
                .BuildAndSave();

            var cmd = new SelfRestoreToBase { PlayerId = player.Id, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You don't have enough mana to do this.  You have 1 and need 10.");
        }

        [Test]
        public void Should_throw_exception_if_player_cleansed_too_much()
        {
            player = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.FormSource, currentForm)
                .With(p => p.OriginalFormSource, originalForm)
                .With(p => p.ActionPoints, 100)
                .With(p => p.Mana, 25)
                .With(p => p.CleansesMeditatesThisRound, 3)
                .BuildAndSave();

            var cmd = new SelfRestoreToBase { PlayerId = player.Id, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You have cleansed and meditated too many times this turn.");
        }

    }
}

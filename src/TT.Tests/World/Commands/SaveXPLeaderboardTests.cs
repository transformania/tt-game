using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Statics;
using TT.Domain.World.Commands;
using TT.Tests.Builders.Covenants;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Game;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.World;

namespace TT.Tests.World.Commands
{
    [TestFixture]
    public class SaveXPLeaderboardTests : TestBase
    {

        [Test]
        public void can_save_xp_leaderboard()
        {

            new WorldBuilder()
                .With(i => i.RoundNumber, "Alpha Round 13")
                .With(i => i.TurnNumber, 5000)
                .With(i => i.RoundDuration, 5000)
                .With(i => i.ChaosMode, false)
                .BuildAndSave();

            var form1 = new FormSourceBuilder()
                .With(f => f.Id, 100)
                .With(f => f.FriendlyName, "Moonwalker")
                .BuildAndSave();

            var form2 = new FormSourceBuilder()
                .With(f => f.Id, 51)
                .With(f => f.FriendlyName, "Werewolf")
                .BuildAndSave();

            var covenant = new CovenantBuilder()
                .With(c => c.Id, 912)
                .With(c => c.Name, "Shoemakers United")
                .BuildAndSave();

            var secondPlace = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "One")
                .With(p => p.Id, 50)
                .With(p => p.GameMode, (int)PvPStatics.GameModes.PvP)
                .With(p => p.Level, 14)
                .With(p => p.XP, 100)
                .With(p => p.FormSource, form2)
                .With(p => p.Gender, PvPStatics.GenderFemale)
                .BuildAndSave();

            var firstPlaceHighestLevel = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "Two")
                .With(p => p.Id, 55)
                .With(p => p.GameMode, (int)PvPStatics.GameModes.PvP)
                .With(p => p.Level, 15)
                .With(p => p.XP, 100)
                .With(p => p.FormSource, form1)
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .With(p => p.GameMode, (int)PvPStatics.GameModes.PvP)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var thirdPlaceTiedLevelLowerXP = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "Four")
                .With(p => p.Id, 65)
                .With(p => p.GameMode, (int)PvPStatics.GameModes.Protection)
                .With(p => p.Level, 14)
                .With(p => p.XP, 99)
                .With(p => p.FormSource, form1)
                .With(p => p.Covenant, covenant)
                .BuildAndSave();


            DomainRegistry.Repository.Execute(new SaveXpLeaderboards { RoundNumber = 13 });

            var leaders = DataContext.AsQueryable<TT.Domain.World.Entities.XpLeaderboardEntry>();

            leaders.Count().Should().Be(3);

            var first = leaders.ElementAt(0);
            first.PlayerName.Should()
                .Be($"{firstPlaceHighestLevel.FirstName} {firstPlaceHighestLevel.LastName}");
            first.FormName.Should().Be(form1.FriendlyName);
            first.FormSource.Id.Should().Be(form1.Id);
            first.Level.Should().Be(firstPlaceHighestLevel.Level);
            first.Mobility.Should().Be(firstPlaceHighestLevel.Mobility);
            first.CovenantName.Should().Be(null);
            first.Sex.Should().Be(firstPlaceHighestLevel.Gender);

            var second = leaders.ElementAt(1);
            second.PlayerName.Should().Be($"{secondPlace.FirstName} {secondPlace.LastName}");
            second.FormName.Should().Be(form2.FriendlyName);
            second.FormSource.Id.Should().Be(form2.Id);
            second.Sex.Should().Be(secondPlace.Gender);

            var third = leaders.ElementAt(2);
            third.PlayerName.Should()
                .Be($"{thirdPlaceTiedLevelLowerXP.FirstName} {thirdPlaceTiedLevelLowerXP.LastName}");
            third.CovenantName.Should().Be(covenant.Name);

        }

        [Test]
        public void should_throw_error_if_not_last_round()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "Alpha Round 13")
                .With(i => i.TurnNumber, 350)
                .With(i => i.RoundDuration, 5000)
                .With(i => i.ChaosMode, false)
                .BuildAndSave();

            Action action = () => Repository.Execute(new SaveXpLeaderboards { RoundNumber = 13 });
            action.ShouldThrowExactly<DomainException>().WithMessage("Unable to save XP leaderboards at this time.  It is turn 350 and needs to be turn 5000.");
        }

        [Test]
        public void should_throw_error_if_in_chaos()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "Alpha Round 13")
                .With(i => i.TurnNumber, 5000)
                .With(i => i.RoundDuration, 5000)
                .With(i => i.ChaosMode, true)
                .BuildAndSave();

            Action action = () => Repository.Execute(new SaveXpLeaderboards { RoundNumber = 13 });
            action.ShouldThrowExactly<DomainException>().WithMessage("Unable to save XP leaderboards at this time.  The game is currently in chaos mode.");
        }

        [Test]
        public void should_throw_error_if_entries_already_exist()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "Alpha Round 13")
                .With(i => i.TurnNumber, 5000)
                .With(i => i.RoundDuration, 5000)
                .With(i => i.ChaosMode, false)
                .BuildAndSave();

            new XPLeaderboardEntryBuilder()
                .With(e => e.Id, 1)
                .With(e => e.RoundNumber, 13)
                .BuildAndSave();

            Action action = () => Repository.Execute(new SaveXpLeaderboards { RoundNumber = 13 });
            action.ShouldThrowExactly<DomainException>().WithMessage("There are already existing XP leaderboard entries for round 13.");
        }

        [Test]
        public void should_throw_error_if_round_number_not_set()
        {
            Action action = () => Repository.Execute(new SaveXpLeaderboards());
            action.ShouldThrowExactly<DomainException>().WithMessage("Round Number must be set!");
        }

        [Test]
        public void should_throw_error_if_no_world_data_found()
        {
            Action action = () => Repository.Execute(new SaveXpLeaderboards { RoundNumber = 13 });
            action.ShouldThrowExactly<DomainException>().WithMessage("No world data found.");
        }
    }
}

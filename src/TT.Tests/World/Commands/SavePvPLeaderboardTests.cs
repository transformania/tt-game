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
    public class SavePvPLeaderboardTests : TestBase
    {

        [Test]
        public void can_save_pvp_leaderboard()
        {

            var form1 = new FormSourceBuilder()
                .With(f => f.Id, 100)
                .With(f => f.FriendlyName, "Moonwalker")
                .BuildAndSave();

            var form2 = new FormSourceBuilder()
                .With(f => f.Id, 51)
                .With(f => f.FriendlyName, "Werewolf")
                .BuildAndSave();

            new WorldBuilder()
                .With(i => i.RoundNumber, "Alpha Round 13")
                .With(i => i.TurnNumber, 5000)
                .With(i => i.RoundDuration, 5000)
                .With(i => i.ChaosMode, false)
                .BuildAndSave();

            var covenant = new CovenantBuilder()
                .With(c => c.Id, 912)
                .With(c => c.Name, "Shoemakers United")
                .BuildAndSave();

            var secondPlace = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "One")
                .With(p => p.Id, 50)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.PvPScore, 5)
                .With(p => p.Level, 10)
                .With(p => p.XP, 100)
                .With(p => p.FormSource, form2)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var firstPlaceHighestDungeonScore = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "Two")
                .With(p => p.Id, 55)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.PvPScore, 100)
                .With(p => p.Level, 3)
                .With(p => p.XP, 15)
                .With(p => p.FormSource, form1)
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .With(p => p.Gender, PvPStatics.GenderFemale)
                .BuildAndSave();

            var fourthPlaceNoDungeonScoreLowerLevel = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "Three")
                .With(p => p.Id, 60)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.PvPScore, 0)
                .With(p => p.Level, 13)
                .With(p => p.XP, 50)
                .With(p => p.FormSource, form1)
                .BuildAndSave();

            var thirdPlaceNoDungeonScoreHigherLevel = new PlayerBuilder()
                .With(p => p.FirstName, "Person")
                .With(p => p.LastName, "Four")
                .With(p => p.Id, 65)
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.PvPScore, 0)
                .With(p => p.Level, 15)
                .With(p => p.XP, 50)
                .With(p => p.FormSource, form1)
                .With(p => p.Covenant, covenant)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new SavePvPLeaderboards { RoundNumber = 13 });

            var leaders = DataContext.AsQueryable<TT.Domain.World.Entities.PvPLeaderboardEntry>();

            leaders.Count().Should().Be(4);

            var first = leaders.ElementAt(0);
            first.PlayerName.Should()
                .Be($"{firstPlaceHighestDungeonScore.FirstName} {firstPlaceHighestDungeonScore.LastName}");
            first.FormName.Should().Be(form1.FriendlyName);
            first.FormSource.Id.Should().Be(form1.Id);
            first.Level.Should().Be(firstPlaceHighestDungeonScore.Level);
            first.Mobility.Should().Be(firstPlaceHighestDungeonScore.Mobility);
            first.CovenantName.Should().Be(null);
            first.Sex.Should().Be(firstPlaceHighestDungeonScore.Gender);

            var second = leaders.ElementAt(1);
            second.PlayerName.Should().Be($"{secondPlace.FirstName} {secondPlace.LastName}");
            second.FormName.Should().Be(form2.FriendlyName);
            second.FormSource.Id.Should().Be(form2.Id);
            second.Sex.Should().Be(secondPlace.Gender);

            var third = leaders.ElementAt(2);
            third.PlayerName.Should()
                .Be($"{thirdPlaceNoDungeonScoreHigherLevel.FirstName} {thirdPlaceNoDungeonScoreHigherLevel.LastName}");
            third.CovenantName.Should().Be(covenant.Name);

            var fourth = leaders.ElementAt(3);
            fourth.PlayerName.Should()
                .Be($"{fourthPlaceNoDungeonScoreLowerLevel.FirstName} {fourthPlaceNoDungeonScoreLowerLevel.LastName}");

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

            Action action = () => Repository.Execute(new SavePvPLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("Unable to save PvP leaderboards at this time.  It is turn 350 and needs to be turn 5000.");
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

            Action action = () => Repository.Execute(new SavePvPLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("Unable to save PvP leaderboards at this time.  The game is currently in chaos mode.");
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

            new PvPLeaderboardEntryBuilder()
                .With(e => e.Id, 1)
                .With(e => e.RoundNumber, 13)
                .BuildAndSave();

            Action action = () => Repository.Execute(new SavePvPLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("There are already existing PvP leaderboard entries for round 13.");
        }

        [Test]
        public void should_throw_error_if_round_number_not_set()
        {
            Action action = () => Repository.Execute(new SavePvPLeaderboards());
            action.Should().ThrowExactly<DomainException>().WithMessage("Round Number must be set!");
        }

        [Test]
        public void should_throw_error_if_no_world_data_found()
        {
            Action action = () => Repository.Execute(new SavePvPLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("No world data found.");
        }
    }
}

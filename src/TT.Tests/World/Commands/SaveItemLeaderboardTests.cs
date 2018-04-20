using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Statics;
using TT.Domain.World.Commands;
using TT.Tests.Builders.Game;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.World;

namespace TT.Tests.World.Commands
{
    [TestFixture]
    public class SaveItemLeaderboardTests : TestBase
    {

        [Test]
        public void can_save_pvp_leaderboard()
        {

            new WorldBuilder()
                .With(i => i.RoundNumber, "Alpha Round 13")
                .With(i => i.TurnNumber, 5000)
                .With(i => i.RoundDuration, 5000)
                .With(i => i.ChaosMode, false)
                .BuildAndSave();

            var itemSource1 = new ItemSourceBuilder()
                .With(f => f.Id, 100)
                .With(f => f.FriendlyName, "Cute Panties")
                .With(f => f.ItemType, PvPStatics.ItemType_Underpants)
                .BuildAndSave();

            var itemSource2 = new ItemSourceBuilder()
                .With(f => f.Id, 172)
                .With(f => f.FriendlyName, "Balloon")
                .With(f => f.ItemType, PvPStatics.ItemType_Accessory)
                .BuildAndSave();

            var thirdPlaceTiedOnLevel = new ItemBuilder()
                .With(i => i.Id, 1234)
                .With(i => i.ItemSource, itemSource1)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.FirstName, "Gary")
                    .With(p => p.LastName, "Underwear")
                    .With(p => p.ItemXP, new InanimateXPBuilder()
                        .With(i => i.Amount, 199)
                        .BuildAndSave())
                    .BuildAndSave())
                .With(i => i.Level, 5)
                .BuildAndSave();

            var secondPlace = new ItemBuilder()
                .With(i => i.Id, 55)
                .With(i => i.ItemSource, itemSource2)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.FirstName, "Susan")
                    .With(p => p.LastName, "Panties")
                    .With(p => p.ItemXP, new InanimateXPBuilder()
                        .With(i => i.Amount, 200)
                        .BuildAndSave())
                    .BuildAndSave())
                .With(i => i.Level, 5)
                .BuildAndSave();

            var firstPlace = new ItemBuilder()
                .With(i => i.Id, 50)
                .With(i => i.ItemSource, itemSource1)
                .With(i => i.FormerPlayer, new PlayerBuilder()
                    .With(p => p.FirstName, "Bob")
                    .With(p => p.LastName, "Panties")
                    .With(p => p.ItemXP, new InanimateXPBuilder()
                        .With(i => i.Amount, 200)
                        .BuildAndSave())
                    .BuildAndSave())
                .With(i => i.Level, 10)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new SaveItemLeaderboards { RoundNumber = 13 });

            var leaders = DataContext.AsQueryable<TT.Domain.World.Entities.ItemLeaderboardEntry>();

            leaders.Count().Should().Be(3);

            var first = leaders.ElementAt(0);
            first.PlayerName.Should()
                .Be($"{firstPlace.FormerPlayer.FirstName} {firstPlace.FormerPlayer.LastName}");
            first.ItemName.Should().Be(itemSource1.FriendlyName);
            first.ItemSource.Id.Should().Be(itemSource1.Id);
            first.Level.Should().Be(firstPlace.Level);
            first.ItemType.Should().Be(itemSource1.ItemType);

            var second = leaders.ElementAt(1);
            second.PlayerName.Should().Be($"{secondPlace.FormerPlayer.FirstName} {secondPlace.FormerPlayer.LastName}");
            second.ItemName.Should().Be(itemSource2.FriendlyName);
            second.ItemSource.Id.Should().Be(itemSource2.Id);

            var third = leaders.ElementAt(2);
            third.PlayerName.Should()
                .Be($"{thirdPlaceTiedOnLevel.FormerPlayer.FirstName} {thirdPlaceTiedOnLevel.FormerPlayer.LastName}");
        }

        [Test]
        public void should_throw_error_if_not_last_round()
        {
            var world = new WorldBuilder()
                 .With(i => i.RoundNumber, "Alpha Round 13")
                 .With(i => i.TurnNumber, 350)
                 .With(i => i.RoundDuration, 5000)
                 .With(i => i.ChaosMode, false)
                 .BuildAndSave();

            Action action = () => Repository.Execute(new SaveItemLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("Unable to save Item/Pet leaderboards at this time.  It is turn 350 and needs to be turn 5000.");
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

            Action action = () => Repository.Execute(new SaveItemLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("Unable to save Item/Pet leaderboards at this time.  The game is currently in chaos mode.");
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

            new ItemLeaderboardEntryBuilder()
                .With(e => e.Id, 1)
                .With(e => e.RoundNumber, 13)
                .BuildAndSave();

            Action action = () => Repository.Execute(new SaveItemLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("There are already existing Item/Pet leaderboard entries for round 13.");
        }

        [Test]
        public void should_throw_error_if_round_number_not_set()
        {
            Action action = () => Repository.Execute(new SaveItemLeaderboards());
            action.Should().ThrowExactly<DomainException>().WithMessage("Round Number must be set!");
        }

        [Test]
        public void should_throw_error_if_no_world_data_found()
        {
            Action action = () => Repository.Execute(new SaveItemLeaderboards { RoundNumber = 13 });
            action.Should().ThrowExactly<DomainException>().WithMessage("No world data found.");
        }
    }
}

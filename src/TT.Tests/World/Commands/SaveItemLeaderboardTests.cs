using System.Linq;
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

            Assert.That(leaders, Has.Exactly(3).Items);

            var first = leaders.ElementAt(0);
            Assert.That(first.PlayerName,
                Is.EqualTo($"{firstPlace.FormerPlayer.FirstName} {firstPlace.FormerPlayer.LastName}"));
            Assert.That(first.ItemName, Is.EqualTo(itemSource1.FriendlyName));
            Assert.That(first.ItemSource.Id, Is.EqualTo(itemSource1.Id));
            Assert.That(first.Level, Is.EqualTo(firstPlace.Level));
            Assert.That(first.ItemType, Is.EqualTo(itemSource1.ItemType));

            var second = leaders.ElementAt(1);
            Assert.That(second.PlayerName,
                Is.EqualTo($"{secondPlace.FormerPlayer.FirstName} {secondPlace.FormerPlayer.LastName}"));
            Assert.That(second.ItemName, Is.EqualTo(itemSource2.FriendlyName));
            Assert.That(second.ItemSource.Id, Is.EqualTo(itemSource2.Id));

            var third = leaders.ElementAt(2);
            Assert.That(third.PlayerName,
                Is.EqualTo(
                    $"{thirdPlaceTiedOnLevel.FormerPlayer.FirstName} {thirdPlaceTiedOnLevel.FormerPlayer.LastName}"));
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

            Assert.That(() => Repository.Execute(new SaveItemLeaderboards {RoundNumber = 13}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    "Unable to save Item/Pet leaderboards at this time.  It is turn 350 and needs to be turn 5000."));
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

            Assert.That(() => Repository.Execute(new SaveItemLeaderboards {RoundNumber = 13}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    "Unable to save Item/Pet leaderboards at this time.  The game is currently in chaos mode."));
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

            Assert.That(() => Repository.Execute(new SaveItemLeaderboards {RoundNumber = 13}),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("There are already existing Item/Pet leaderboard entries for round 13."));
        }

        [Test]
        public void should_throw_error_if_round_number_not_set()
        {
            Assert.That(() => Repository.Execute(new SaveItemLeaderboards()),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Round Number must be set!"));
        }

        [Test]
        public void should_throw_error_if_no_world_data_found()
        {
            Assert.That(() => Repository.Execute(new SaveItemLeaderboards {RoundNumber = 13}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("No world data found."));
        }
    }
}

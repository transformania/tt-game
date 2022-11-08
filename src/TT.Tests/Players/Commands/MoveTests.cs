using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Entities.MindControl;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Tests.Builders.Effects;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.MindControl;
using TT.Tests.Builders.Players;
using Item = TT.Domain.Items.Entities.Item;

namespace TT.Tests.Players.Commands
{
    [TestFixture]
    public class MoveTests : TestBase
    {

        private Player player;
        private string destination;
        private List<Stat> stats;
        private List<Item> items;

        [SetUp]
        public void Init()
        {
            var item1 = new ItemBuilder()
                .With(i => i.dbLocationName, LocationsStatics.STREET_230_SUNNYGLADE_DRIVE)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave())
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.dbLocationName, LocationsStatics.STREET_230_SUNNYGLADE_DRIVE)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();
            items = new List<Item> {item1, item2};

            stats = new List<Stat>
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMovedAsAnimate).With(t => t.Amount, 88).BuildAndSave(),
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMovedAsPet).With(t => t.Amount, 88).BuildAndSave()
            };

            player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.ActionPoints, TurnTimesStatics.GetActionPointReserveLimit())
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .With(p => p.Items, items)
                .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .BuildAndSave();

            destination = "coffee_shop_patio";
        }

        [Test]
        public void can_move_as_animate()
        {
            Assert.That(() => DomainRegistry.Repository.Execute(new Move {PlayerId = 50, destination = destination}),
                Throws.Nothing);

            Assert.That(
                DataContext.AsQueryable<LocationLog>()
                    .First(l => l.dbLocationName == LocationsStatics.STREET_200_MAIN_STREET).Message,
                Is.EqualTo("John Doe left toward Carolyne's Coffee Shop (Patio)")); // Moved from

            Assert.That(DataContext.AsQueryable<LocationLog>().First(l => l.dbLocationName == destination).Message,
                Is.EqualTo("John Doe entered from Street: 200 Main Street")); // Moved to

            Assert.That(DataContext.AsQueryable<PlayerLog>().First(p => p.Owner.Id == 50).Message,
                Is.EqualTo("You moved from <b>Street: 200 Main Street</b> to <b>Carolyne's Coffee Shop (Patio)</b>."));

            Assert.That(player.Location, Is.EqualTo(destination));
            
            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMovedAsAnimate).Amount,
                Is.EqualTo(89));
            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMovedAsPet).Amount,
                Is.EqualTo(88));
        }

        [Test]
        public void can_move_as_animate_with_direction_override()
        {
            Assert.That(() => DomainRegistry.Repository.Execute(new Move {PlayerId = 50, destination = destination, Direction = "East"}),
                Throws.Nothing);

            Assert.That(
                DataContext.AsQueryable<LocationLog>()
                    .First(l => l.dbLocationName == LocationsStatics.STREET_200_MAIN_STREET).Message,
                Is.EqualTo("John Doe left toward Carolyne's Coffee Shop (Patio)")); // Moved from

            Assert.That(DataContext.AsQueryable<LocationLog>().First(l => l.dbLocationName == destination).Message,
                Is.EqualTo("John Doe entered from Street: 200 Main Street")); // Moved to

            Assert.That(DataContext.AsQueryable<PlayerLog>().First(p => p.Owner.Id == 50).Message,
                Is.EqualTo("You moved <b>East</b>."));

            Assert.That(player.Location, Is.EqualTo(destination));
            
            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMovedAsAnimate).Amount,
                Is.EqualTo(89));
            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMovedAsPet).Amount,
                Is.EqualTo(88));
        }


        [Test]
        public void can_move_as_animal()
        {
            player = new PlayerBuilder()
                .With(p => p.Id, 55)
                .With(p => p.ActionPoints, TurnTimesStatics.GetActionPointReserveLimit())
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .With(p => p.Item, new ItemBuilder()
                    .With(i => i.Id, 500)
                    .With(i => i.dbLocationName, "someplace")
                    .BuildAndSave()
                )
                .BuildAndSave();

            Assert.That(() => DomainRegistry.Repository.Execute(new Move {PlayerId = 55, destination = destination}),
                Throws.Nothing);

            Assert.That(
                DataContext.AsQueryable<LocationLog>()
                    .First(l => l.dbLocationName == LocationsStatics.STREET_200_MAIN_STREET).Message,
                Is.EqualTo("John Doe (feral) left toward Carolyne's Coffee Shop (Patio)")); // Moved from

            Assert.That(
                DataContext.AsQueryable<LocationLog>().First(l => l.dbLocationName == destination).Message,
                Is.EqualTo("John Doe (feral) entered from Street: 200 Main Street")); // Moved to

            Assert.That(DataContext.AsQueryable<PlayerLog>().First(p => p.Owner.Id == 55).Message,
                Is.EqualTo("You moved from <b>Street: 200 Main Street</b> to <b>Carolyne's Coffee Shop (Patio)</b>."));

            Assert.That(player.Location, Is.EqualTo(destination));
            Assert.That(player.Item.dbLocationName, Is.EqualTo(destination));
            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMovedAsAnimate).Amount,
                Is.EqualTo(88));
            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMovedAsPet).Amount,
                Is.EqualTo(89));

        }

        [Test]
        public void feral_animal_cannot_be_immobilized()
        {
            player = new PlayerBuilder()
                .With(p => p.Id, 55)
                .With(p => p.ActionPoints, TurnTimesStatics.GetActionPointReserveLimit())
                .With(p => p.MoveActionPointDiscount, -999)  // Makes animate form immobile
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .With(p => p.Item, new ItemBuilder()
                    .With(i => i.Id, 500)
                    .With(i => i.dbLocationName, "someplace")
                    .BuildAndSave()
                )
                .BuildAndSave();

            Assert.That(() => DomainRegistry.Repository.Execute(new Move { PlayerId = 55, destination = destination }),
                Throws.Nothing);
        }

        [Test]
        public void should_throw_exception_if_player_not_provided()
        {
            var cmd = new Move { PlayerId = 0, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player ID is required!"));
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new Move { PlayerId = 1234, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID '1234' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_destination_not_provided()
        {
            var cmd = new Move { PlayerId = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Destination must be specified"));
        }

        [Test]
        public void should_throw_exception_if_destination_invalid()
        {
            var cmd = new Move { PlayerId = 50, destination = "fakePlace" };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Location with dbName 'fakePlace' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_player_has_insufficient_ap()
        {
            var cmd = new Move { PlayerId = 50, destination = destination };
            player.SetActionPoints(0);
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You don't have enough action points to move."));
        }

        [Test]
        public void should_throw_exception_if_player_is_inanimate()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You can't move because you are currently inanimate!"));
        }

        [Test]
        public void should_throw_exception_if_player_is_not_feral_animal()
        {

            var petPlayer = new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .BuildAndSave();

            var item = new ItemBuilder()
                .With(p => p.FormerPlayer, petPlayer)
                .With(p => p.Owner, new PlayerBuilder()
                    .With(p => p.FirstName, "Bob")
                    .With(p => p.LastName, "Smith")
                    .BuildAndSave())
                .BuildAndSave();

            petPlayer.SetItem(item);
            item.SetFormerPlayer(petPlayer);

            var cmd = new Move { PlayerId = 51, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You can't move because you are a non-feral pet owned by Bob Smith."));
        }

        [Test]
        public void should_throw_exception_if_player_is_immobilized()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.MoveActionPointDiscount, -999)
                .With(p => p.Items, items)
                .BuildAndSave();
            var cmd = new Move { PlayerId = 51, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You can't move since you have been immobilized!"));
        }

        [Test]
        public void should_throw_exception_if_player_is_carrying_too_much_with_1_extra_item()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.ExtraInventory, -5)
                .With(p => p.Items, items)
                .BuildAndSave();
            
            var cmd = new Move { PlayerId = 51, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You are carrying too much to move.  You need to drop at least 1 item."));
        }

        [Test]
        public void should_throw_exception_if_player_is_carrying_too_much_with_2_extra_items()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.ExtraInventory, -6)
                .With(p => p.Items, items)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You are carrying too much to move.  You need to drop at least 2 items."));
        }

        [Test]
        [Ignore("TODO")]
        public void should_throw_exception_if_player_is_mind_controlled()
        {

            var mindControl = new VictimMindControlBuilder()
                .With(v => v.FormSourceId, MindControlStatics.MindControl__MovementFormSourceId)
                .With(v => v.TurnsRemaining, 3)
                .BuildAndSave();

            var mindControlList = new List<VictimMindControl>
            {
                mindControl
            };

            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
                .With(p => p.VictimMindControls, mindControlList)
                .BuildAndSave();

            var effectSourceMarch = new EffectSourceBuilder()
                .With(e => e.Id, MindControlStatics.MindControl__Movement_DebuffEffectSourceId)
                .BuildAndSave();

            var effectMarch = new EffectBuilder()
                .With(e => e.EffectSource, effectSourceMarch)
                .BuildAndSave();

            player.Effects.Add(effectMarch);
            
            var cmd = new Move { PlayerId = 51, destination = destination };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    "You try to move but discover you cannot!  Some other mage has partial control of your mind, disabling your ability to move on your own!"));
        }

        [Test]
        public void should_throw_exception_if_player_has_attacked_too_recently()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, destination = destination };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .Matches("You are resting from a recent attack\\.  You must wait .* more seconds before moving\\."));
        }

        [Test]
        public void should_throw_exception_if_player_is_in_a_duel()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.InDuel, 100)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, destination = destination };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must finish your duel before you can move again."));
        }

        [Test]
        public void should_throw_exception_if_player_is_in_a_quest()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.InQuest, 100)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, destination = destination };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must end your quest before you can move again."));
        }

        [Test]
        public void should_throw_exception_if_cant_move_to_specified_location()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, destination = destination };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    "You cannot move directly from Street: 70 E. 9th Avenue to Carolyne's Coffee Shop (Patio)."));
        }
    }
}

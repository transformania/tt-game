using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
using TT.Domain.ViewModels;
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
        private BuffBox buffs;
        private string destination;
        private List<Stat> stats;

        [SetUp]
        public void Init()
        {
            buffs = new BuffBox();

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

            var items = new List<Item>();

            items.Add(item1);
            items.Add(item2);

            stats = new List<Stat>()
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 88).BuildAndSave()
            };

            player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.ActionPoints, 120)
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
            DomainRegistry.Repository.Execute(new Move { PlayerId = 50, Buffs = buffs, destination = destination });

            var fromLocationLog = DataContext.AsQueryable<LocationLog>()
                .FirstOrDefault(l => l.dbLocationName == LocationsStatics.STREET_200_MAIN_STREET);

            fromLocationLog.Message.Should().Be("John Doe left toward Carolyne's Coffee Shop (Patio)");

            var toLocationLog = DataContext.AsQueryable<LocationLog>()
                .FirstOrDefault(l => l.dbLocationName == destination);

            toLocationLog.Message.Should().Be("John Doe entered from Street: 200 Main Street");

            var playerLogs = DataContext.AsQueryable<PlayerLog>().First(p => p.Owner.Id == 50);
            playerLogs.Message.Should().Be("You moved from <b>Street: 200 Main Street</b> to <b>Carolyne's Coffee Shop (Patio)</b>.");

            player.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount.Should()
                .Be(89);

        }

        [Test]
        public void can_move_as_animal()
        {

            player = new PlayerBuilder()
                .With(p => p.Id, 55)
                .With(p => p.ActionPoints, 120)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .With(p => p.Location, LocationsStatics.STREET_200_MAIN_STREET)
                .With(p => p.Mobility, PvPStatics.MobilityPet)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new Move { PlayerId = 55, Buffs = buffs, destination = destination });

            var fromLocationLog = DataContext.AsQueryable<LocationLog>()
                .FirstOrDefault(l => l.dbLocationName == LocationsStatics.STREET_200_MAIN_STREET);

            fromLocationLog.Message.Should().Be("John Doe (feral) left toward Carolyne's Coffee Shop (Patio)");

            var toLocationLog = DataContext.AsQueryable<LocationLog>()
                .FirstOrDefault(l => l.dbLocationName == destination);

            toLocationLog.Message.Should().Be("John Doe (feral) entered from Street: 200 Main Street");

            var playerLogs = DataContext.AsQueryable<PlayerLog>().First(p => p.Owner.Id == 55);
            playerLogs.Message.Should().Be("You moved from <b>Street: 200 Main Street</b> to <b>Carolyne's Coffee Shop (Patio)</b>.");

            player.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount.Should()
                .Be(89);

        }

        [Test]
        public void should_throw_exception_if_player_not_provided()
        {
            var cmd = new Move { PlayerId = 0, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("Player ID is required!");
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new Move { PlayerId = 1234, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("Player with ID '1234' could not be found");
        }

        [Test]
        public void should_throw_exception_if_destination_not_provided()
        {
            var cmd = new Move { PlayerId = 50, Buffs = buffs };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("Destination must be specified");
        }

        [Test]
        public void should_throw_exception_if_destination_invalid()
        {
            var cmd = new Move { PlayerId = 50, Buffs = buffs, destination = "fakePlace" };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("Location with dbName 'fakePlace' could not be found");
        }

        [Test]
        public void should_throw_exception_if_buffs_not_provided()
        {
            var cmd = new Move { PlayerId = 5, Buffs = null, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("Buffs are required!");
        }

        [Test]
        public void should_throw_exception_if_player_has_insufficient_ap()
        {
            var cmd = new Move { PlayerId = 50, Buffs = buffs, destination = destination };
            player.SetActionPoints(0);
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You don't have enough action points to move.");
        }

        [Test]
        public void should_throw_exception_if_player_is_inanimate()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You can't move because you are currently inanimate!");
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

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You can't move because you are a non-feral pet owned by Bob Smith.");
        }

        [Test]
        public void should_throw_exception_if_player_is_immobilized()
        {
            buffs.FromEffects_MoveActionPointDiscount = -999;
            var cmd = new Move { PlayerId = 50, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You can't move since you have been immobilized!");
        }

        [Test]
        public void should_throw_exception_if_player_is_carrying_too_much_with_1_extra_item()
        {
            buffs.FromForm_ExtraInventorySpace = -5; // limit of 1
            var cmd = new Move { PlayerId = 50, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You are carrying too much to move.  You need to drop at least 1 item.");
        }

        [Test]
        public void should_throw_exception_if_player_is_carrying_too_much_with_2_extra_items()
        {
            buffs.FromForm_ExtraInventorySpace = -6; // limit of 0
            var cmd = new Move { PlayerId = 50, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You are carrying too much to move.  You need to drop at least 2 items.");
        }

        [Test]
        [Ignore("TODO")]
        public void should_throw_exception_if_player_is_mind_controlled()
        {

            var mindControl = new VictimMindControlBuilder()
                .With(v => v.Type, MindControlStatics.MindControl__Movement)
                .With(v => v.TurnsRemaining, 3)
                .BuildAndSave();

            var mindControlList = new List<VictimMindControl>();
            mindControlList.Add(mindControl);

            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.VictimMindControls, mindControlList)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };
            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You try to move but discover you cannot!  Some other mage has partial control of your mind, disabling your ability to move on your own!");
        }

        [Test]
        public void should_throw_exception_if_player_has_attacked_too_recently()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };

            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You are resting from a recent attack.  You must wait 44 more seconds before moving.");
        }

        [Test]
        public void should_throw_exception_if_player_is_in_a_duel()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.InDuel, 100)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };

            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You must finish your duel before you can move again.");
        }

        [Test]
        public void should_throw_exception_if_player_is_in_a_quest()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.InQuest, 100)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };

            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You must end your quest before you can move again.");
        }

        [Test]
        public void should_throw_exception_if_cant_move_to_specified_location()
        {
            new PlayerBuilder()
                .With(p => p.Id, 51)
                .BuildAndSave();

            var cmd = new Move { PlayerId = 51, Buffs = buffs, destination = destination };

            var action = new Action(() => { Repository.Execute(cmd); });
            action.Should().ThrowExactly<DomainException>().WithMessage("You cannot move directly from Street: 70 E. 9th Avenue to Carolyne's Coffee Shop (Patio).");
        }

    }
}

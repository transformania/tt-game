using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{

    [TestFixture]
    public class GiveRuneTests : TestBase
    {

        private Player owner;
        private ItemSource runeSource;

        [SetUp]
        public void Init()
        {
            owner = new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            runeSource = new ItemSourceBuilder()
                .With(i => i.Id, 500)
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.FriendlyName, "Rune of Power")
                .BuildAndSave();

        }

        [Test]
        public void can_give_rune()
        {
            DomainRegistry.Repository.Execute(new GiveRune { PlayerId = owner.Id, ItemSourceId = runeSource.Id});

            owner.Items.Count.Should().Be(1);
            var rune = owner.Items.First();
            rune.ItemSource.Id.Should().Be(runeSource.Id);
            rune.ItemSource.FriendlyName.Should().Be(runeSource.FriendlyName);
            rune.IsEquipped.Should().Be(false);
            rune.Owner.Id.Should().Be(owner.Id);

        }

        [Test]
        public void throw_exception_if_playerId_not_provided()
        {
            var cmd = new GiveRune { ItemSourceId = runeSource.Id};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("PlayerId is required!");
        }

        [Test]
        public void throw_exception_if_itemSourceId_not_provided()
        {
            var cmd = new GiveRune { PlayerId = owner.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("ItemId is required!");
        }

        [Test]
        public void throw_exception_if_player_not_found()
        {
            var cmd = new GiveRune { PlayerId = 555, ItemSourceId = runeSource.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Player with ID '555' could not be found");
        }

        [Test]
        public void throw_exception_if_runeSource_not_found()
        {
            var cmd = new GiveRune { PlayerId = owner.Id, ItemSourceId = 555 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("ItemSource with ID '555' could not be found");
        }

    }
}

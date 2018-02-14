using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class UnembedRuneTests : TestBase
    {
        private Player player;
        private Item item1;
        private Item rune1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            item1 = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .With(i => i.Owner, player)
                .With(i => i.Runes, new List<Item>())
                .BuildAndSave();

            rune1 = new ItemBuilder()
                .With(i => i.Id, 200)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            item1.AttachRune(rune1);
            rune1.SetEquippedThisTurn(false);

        }

        [Test]
        public void can_unembed_a_rune()
        {

            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = rune1.Id
            };

            var result = Repository.Execute(cmd);

            item1.Runes.Count.Should().Be(0);

            rune1.EmbeddedOnItem.Should().Be(null);

            result.Should().Be("You unembedded your <b>Test Item Source</b>.");
        }

        [Test]
        public void should_throw_exception_if_player_id_not_defined()
        {
            var cmd = new UnembedRune
            {
                ItemId = rune1.Id
            };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("PlayerId is required");
        }

        [Test]
        public void should_throw_exception_if_item_id_not_defined()
        {
            var cmd = new UnembedRune
            {
                PlayerId = player.Id
            };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ItemId is required");
        }

        [Test]
        public void should_throw_exception_if_item_not_found()
        {
            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = 999
            };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Rune with id '999' could not be found!");
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new UnembedRune
            {
                PlayerId = 999,
                ItemId = rune1.Id
            };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You don't own this rune!");
        }

        [Test]
        public void should_throw_exception_if_rune_not_on_item()
        {

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 201)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = rune2.Id
            };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("This rune is not currently embdded on an item.");
        }

        [Test]
        public void should_throw_exception_if_rune_already_equipped_this_turn()
        {

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 201)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .With(i => i.EquippedThisTurn, true)
                .With(i => i.EmbeddedOnItem, item1)
                .BuildAndSave();

            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = rune2.Id
            };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("This rune was equipped this turn.  Wait until next turn to remove it.");
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Forms.Entities;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Form;

namespace TT.Tests.Items.Commands
{
    public class SoulbindChangeFormTests : TestBase
    {
        private Player formerItemPlayer;
        private Player ownerPlayer;
        private Item item;
        private FormSource formSource;
        private ItemSource itemSource;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            formerItemPlayer = new PlayerBuilder()
                .With(p => p.Id, 59)
                .With(p => p.FirstName, "Bob")
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .With(p =>  p.Level, 5)
                .BuildAndSave();

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 68)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 33)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .With(i => i.Level, 5)
                .BuildAndSave();

            formSource = new FormSourceBuilder()
                .With(i => i.Id, 228)
                .BuildAndSave();

            itemSource = new ItemSourceBuilder()
                .With(i => i.Id, 140)
                .BuildAndSave();

        }

        [Test]
        public void can_change_item()
        {

            ownerPlayer = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Items, new List<Item>())
                .With(p => p.Level, 5)
                .With(p => p.Money, 1250)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.Level, 5)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = formSource.Id,
                ItemSource = itemSource.Id,
                ItemId = item.Id
            };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);
        }

        [Test]
        public void should_throw_exception_if_player_null()
        {
            var cmd = new SoulbindChangeForm
            {
                PlayerId = 12345
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID 12345 could not be found"));
        }

        [Test]
        public void should_throw_exception_if_item_null()
        {

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                OwnerId = ownerPlayer.Id,
                ItemId = 12345,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Item with FormerPlayerID 59 could not be found"));
        }

        [Test]
        public void should_throw_exception_if_player_doesnt_own_item()
        {
            var someoneElse = new PlayerBuilder()
                .With(p => p.Id, 1831)
                .With(p => p.FirstName, "Sam")
                .With(p => p.Level, 5)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, someoneElse)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, false)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You don't own that item."));
        }

        [Test]
        public void should_throw_exception_if_item_not_locked()
        {

            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, false)
                .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Only permanent items or pets may be reshaped."));
        }

        [Test]
        public void should_throw_exception_if_not_player_item()
        {
            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.Id, 87)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, true)
                .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Only souled items may be reshaped."));
        }

        [Test]
        public void should_throw_exception_if_item_nonconsenting()
        {
            item = new ItemBuilder()
                .With(i => i.Owner, ownerPlayer)
                .With(i => i.Id, 734)
                .With(i => i.IsPermanent, true)
                .With(i => i.ConsentsToSoulbinding, false)
                .With(i => i.FormerPlayer, formerItemPlayer)
                .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("This item is not currently consenting to soulbinding."));
        }

        [Test]
        public void should_throw_exception_if_formid_formname_invalid()
        {
            item = new ItemBuilder()
               .With(i => i.Owner, ownerPlayer)
               .With(i => i.FormerPlayer, formerItemPlayer)
               .With(i => i.Id, 734)
               .With(i => i.IsPermanent, true)
               .With(i => i.ConsentsToSoulbinding, true)
               .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = -1
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("FormId or FormName are required"));
        }

        [Test]
        public void should_throw_exception_if_formid_invalid()
        {
            item = new ItemBuilder()
               .With(i => i.Owner, ownerPlayer)
               .With(i => i.FormerPlayer, formerItemPlayer)
               .With(i => i.Id, 734)
               .With(i => i.IsPermanent, true)
               .With(i => i.ConsentsToSoulbinding, true)
               .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = -1,
                FormId = -1
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("FormSource with ID -1 could not be found"));
        }

        [Test]
        public void should_throw_exception_if_formsourceid_invalid()
        {
            item = new ItemBuilder()
               .With(i => i.Owner, ownerPlayer)
               .With(i => i.FormerPlayer, formerItemPlayer)
               .With(i => i.Id, 734)
               .With(i => i.IsPermanent, true)
               .With(i => i.ConsentsToSoulbinding, true)
               .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                ItemId = item.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = 123456789,
                FormId = 100
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("FormSource with id '123456789' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_itemid_invalid()
        {
            item = new ItemBuilder()
               .With(i => i.Owner, ownerPlayer)
               .With(i => i.FormerPlayer, formerItemPlayer)
               .With(i => i.Id, 734)
               .With(i => i.IsPermanent, true)
               .With(i => i.ConsentsToSoulbinding, true)
               .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = formSource.Id,
                ItemSource = 123456789,
                ItemId = item.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("ItemSource with id '123456789' could not be found"));

        }

        [Test]
        public void should_throw_exception_if_itemsource_invalid()
        {
            item = new ItemBuilder()
               .With(i => i.Owner, ownerPlayer)
               .With(i => i.FormerPlayer, formerItemPlayer)
               .With(i => i.Id, 734)
               .With(i => i.IsPermanent, true)
               .With(i => i.ConsentsToSoulbinding, true)
               .BuildAndSave();

            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = formSource.Id,
                ItemSource = -1,
                ItemId = item.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("ItemSource with ID 734 could not be found"));
        }

        [Test]
        public void should_throw_exception_if_insufficient_arpeyjis()
        {
            var cmd = new SoulbindChangeForm
            {
                PlayerId = formerItemPlayer.Id,
                OwnerId = ownerPlayer.Id,
                FormSourceId = formSource.Id,
                ItemSource = itemSource.Id,
                ItemId = item.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo($"You cannot afford this.  You need <b>1250</b> Arpeyjis and only have <b>0</b>."));
        }
    }
}

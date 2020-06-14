using NUnit.Framework;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Entities
{
    public class ItemSourceTests : TestBase
    {
        [Test]
        [TestCase(PvPStatics.ItemType_Rune)]
        [TestCase(PvPStatics.ItemType_Consumable)]
        public void items_start_with_initial_permanency_status_true(string itemType)
        {
            var itemSource = new ItemSourceBuilder()
                .With(i => i.ItemType, itemType)
                .BuildAndSave();

            Assert.That(itemSource.IsPermanentFromCreation(), Is.True);
        }

        [Test]
        [TestCase(PvPStatics.ItemType_Shirt)]
        [TestCase(PvPStatics.ItemType_Accessory)]
        [TestCase(PvPStatics.ItemType_Hat)]
        [TestCase(PvPStatics.ItemType_Pants)]
        [TestCase(PvPStatics.ItemType_Pet)]
        [TestCase(PvPStatics.ItemType_Shoes)]
        [TestCase(PvPStatics.ItemType_Undershirt)]
        [TestCase(PvPStatics.ItemType_Underpants)]
        public void items_start_with_initial_permanenct_status_false(string itemType)
        {
            var itemSource = new ItemSourceBuilder()
                .With(i => i.ItemType, itemType)
                .BuildAndSave();

            Assert.That(itemSource.IsPermanentFromCreation(), Is.False);
        }
    }
}

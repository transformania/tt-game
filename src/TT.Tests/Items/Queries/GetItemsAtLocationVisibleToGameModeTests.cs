using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemsAtLocationVisibleToGameModeTests : TestBase
    {
        [Test]
        public void get_all_items_at_location_visible_to_PvP()
        {

            // invisible
            new ItemBuilder().With(i => i.Id, 77)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.Protection)
                .BuildAndSave();

            // visible
            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.PvP)
                .BuildAndSave();

            // visible
            new ItemBuilder().With(i => i.Id, 91)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.Any)
                .BuildAndSave();

            var cmd = new GetItemsAtLocationVisibleToGameMode() {dbLocationName = "swamps", gameMode = (int)GameModeStatics.GameModes.PvP };

            var items = DomainRegistry.Repository.Find(cmd);

            var visibleItemIds = items.Select(i => i.Id);

            Assert.That(visibleItemIds, Has.Member(99));
            Assert.That(visibleItemIds, Has.Member(91));
            Assert.That(visibleItemIds, Has.No.Member(77));

        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Protection)]
        [TestCase((int)GameModeStatics.GameModes.Superprotection)]
        public void get_all_items_at_location_visible_to_Protection(int gameMode)
        {

            // visible
            new ItemBuilder().With(i => i.Id, 77)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.Protection)
                .BuildAndSave();

            // invisible
            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.PvP)
                .BuildAndSave();

            // visible
            new ItemBuilder().With(i => i.Id, 91)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.Any)
                .BuildAndSave();

            var cmd = new GetItemsAtLocationVisibleToGameMode() { dbLocationName = "swamps", gameMode = gameMode };

            var items = DomainRegistry.Repository.Find(cmd);

            var visibleItemIds = items.Select(i => i.Id);

            Assert.That(visibleItemIds, Has.Member(77));
            Assert.That(visibleItemIds, Has.Member(91));
            Assert.That(visibleItemIds, Has.No.Member(99));
        }
    }
}

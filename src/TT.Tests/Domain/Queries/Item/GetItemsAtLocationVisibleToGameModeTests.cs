using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Item;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Item
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
                .With(i => i.PvPEnabled, GameModeStatics.Protection)
                .BuildAndSave();

            // visible
            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, GameModeStatics.PvP)
                .BuildAndSave();

            // visible
            new ItemBuilder().With(i => i.Id, 91)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, GameModeStatics.Any)
                .BuildAndSave();

            var cmd = new GetItemsAtLocationVisibleToGameMode() {dbLocationName = "swamps", gameMode = GameModeStatics.PvP};

            var items = DomainRegistry.Repository.Find(cmd);

            var visibleItemIds = items.Select(i => i.Id);

            visibleItemIds.Should().Contain(99);
            visibleItemIds.Should().Contain(91);
            visibleItemIds.Should().NotContain(77);

        }

        [Test]
        [TestCase(GameModeStatics.Protection)]
        [TestCase(GameModeStatics.SuperProtection)]
        public void get_all_items_at_location_visible_to_Protection(int gameMode)
        {

            // visible
            new ItemBuilder().With(i => i.Id, 77)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, GameModeStatics.Protection)
                .BuildAndSave();

            // invisible
            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, GameModeStatics.PvP)
                .BuildAndSave();

            // visible
            new ItemBuilder().With(i => i.Id, 91)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .With(i => i.PvPEnabled, GameModeStatics.Any)
                .BuildAndSave();

            var cmd = new GetItemsAtLocationVisibleToGameMode() { dbLocationName = "swamps", gameMode = gameMode };

            var items = DomainRegistry.Repository.Find(cmd);

            var visibleItemIds = items.Select(i => i.Id);

            visibleItemIds.Should().Contain(77);
            visibleItemIds.Should().Contain(91);
            visibleItemIds.Should().NotContain(99);

        }

    }
}

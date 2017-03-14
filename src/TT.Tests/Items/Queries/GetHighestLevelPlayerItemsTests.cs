using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetHighestLevelPlayerItemsTests : TestBase
    {

        [Test]
        public void should_get_top_player_items()
        {

            // low level player with high ItemXP (second place)
            var low_level_player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.ItemXP, new InanimateXPBuilder().With(i => i.Amount, 50).BuildAndSave())
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            var low_level_player_item = new ItemBuilder().With(i => i.Id, 21)
                 .With(i => i.FormerPlayer, low_level_player)
                 .With(i => i.Level, 1)
                 .With(i => i.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                 .BuildAndSave();

            // low level player with low ItemXP (third place)
            var low_level_player_low_itemXP = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.ItemXP, new InanimateXPBuilder().With(i => i.Amount, 1).BuildAndSave())
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            var low_level_item_low_xp = new ItemBuilder().With(i => i.Id, 100)
                 .With(i => i.FormerPlayer, low_level_player_low_itemXP)
                 .With(i => i.Level, 1)
                 .With(i => i.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                 .BuildAndSave();

            // high level player item (1st place)
            var high_level_player = new PlayerBuilder()
                .With(p => p.Id, 51)
                .With(p => p.ItemXP, new InanimateXPBuilder().With(i => i.Amount, 50).BuildAndSave())
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            var high_level_player_item = new ItemBuilder().With(i => i.Id, 22)
                 .With(i => i.FormerPlayer, high_level_player)
                 .With(i => i.Level, 2)
                 .With(i => i.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                 .BuildAndSave();

            // bot item (should be excluded entirely)
            var bot_player = new PlayerBuilder()
               .With(p => p.Id, 52)
               .With(p => p.ItemXP, new InanimateXPBuilder().With(i => i.Amount, 50).BuildAndSave())
               .With(p => p.BotId, AIStatics.PsychopathBotId)
               .BuildAndSave();

            var item_from_bot = new ItemBuilder().With(i => i.Id, 23)
                 .With(i => i.FormerPlayer, bot_player)
                 .With(i => i.Level, 54)
                 .With(i => i.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                 .BuildAndSave();

            var cmd = new GetHighestLevelPlayerItems { Limit = 5 };

            var items = DomainRegistry.Repository.Find(cmd).ToArray();

            items.Count().Should().Be(3);
            items[0].Id.Should().Be(high_level_player_item.Id);
            items[1].Id.Should().Be(low_level_player_item.Id);
            items[2].Id.Should().Be(low_level_item_low_xp.Id);

            var allIds = items.Select(i => i.Id);
            allIds.Should().NotContain(item_from_bot.Id);

        }
    }
}

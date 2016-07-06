using System;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Items;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Entities.Items
{
    [TestFixture]
    public class ItemTests : TestBase
    {
        [Test]
        public void bot_items_have_old_last_souled_timestamp()
        {
            var player = new PlayerBuilder()
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            var itemSource = new ItemSourceBuilder().BuildAndSave();

            var createItemCmd = new CreateItem
            {
                
            };

            var item = Item.Create(player, itemSource, createItemCmd);

            var timeDifference = Math.Abs((item.LastSouledTimestamp - DateTime.UtcNow).TotalDays);

            timeDifference.Should().BeGreaterThan(365);
        }

        [Test]
        public void human_items_have_old_last_souled_timestamp()
        {
            var player = new PlayerBuilder()
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            var itemSource = new ItemSourceBuilder().BuildAndSave();

            var createItemCmd = new CreateItem
            {

            };

            var item = Item.Create(player, itemSource, createItemCmd);

            var timeDifference = Math.Abs((item.LastSouledTimestamp - DateTime.UtcNow).TotalDays);

            timeDifference.Should().BeLessThan(1);
        }
    }
}

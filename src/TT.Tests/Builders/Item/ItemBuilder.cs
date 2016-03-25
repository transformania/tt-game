using System;
using TT.Domain.Entities.Item;

namespace TT.Tests.Builders.Item
{
    public class ItemBuilder : Builder<ItemSource, int>
    {
        public ItemBuilder()
        {
            Instance = Create();
            With(u => u.DbName, "test_item");
            With(u => u.FriendlyName, "Test Item Source");
        }
    }
}

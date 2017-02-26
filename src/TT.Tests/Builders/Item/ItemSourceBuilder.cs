
namespace TT.Tests.Builders.Item
{
    public class ItemBuilder : Builder<TT.Domain.Items.Entities.Item, int>
    {
        public ItemBuilder()
        {
            Instance = Create();
            With(u => u.ItemSource, new ItemSourceBuilder().BuildAndSave()).BuildAndSave();
        }
    }
}

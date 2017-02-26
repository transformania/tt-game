using TT.Domain.Assets.Commands;
using TT.Domain.Entities;
using TT.Domain.Items.Entities;

namespace TT.Domain.Assets.Entities
{
    public class Tome : Entity<int>
    {
        public string Text { get; protected set; }
        public ItemSource BaseItem { get; protected set; }

        private Tome() { }

        public static Tome Create(ItemSource baseItem, string text)
        {
            return new Tome
            {
                Text = text,
                BaseItem = baseItem
            };
        }

        public Tome Update(UpdateTome cmd, ItemSource baseItem)
        {
            Id = cmd.TomeId;
            Text = cmd.Text;
            BaseItem = baseItem;
            return this;
        }
    }
}
using System;
using TT.Domain.Commands.Assets;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.Items;

namespace TT.Domain.Entities.Assets
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
            this.Id = cmd.Id;
            this.Text = cmd.Text;
            this.BaseItem = baseItem;
            return this;
        }

        

    }
}
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class EmbedRune : DomainCommand<string>
    {
        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public int RuneId { get; set; }

        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var itemToEmbedOn = ctx.AsQueryable<Item>()
                .Include(i => i.Owner)
                .Include(i => i.Runes)
                .Include(i => i.EmbeddedOnItem)
                .Include(i => i.ItemSource)
                .SingleOrDefault(cr => cr.Id == ItemId);

                var rune = ctx.AsQueryable<Item>()
                    .Include(i => i.Owner)
                    .Include(i => i.ItemSource)
                    .Include(i => i.EmbeddedOnItem)
                    .SingleOrDefault(cr => cr.Id == RuneId);


                if (itemToEmbedOn == null)
                    throw new DomainException($"Item with ID '{ItemId}' could not be found");

                if (rune == null)
                    throw new DomainException($"Rune with ID '{ItemId}' could not be found");

                if (itemToEmbedOn.Owner.Id != PlayerId)
                    throw new DomainException("You do not own the item you are attempting to embed runes on.");

                if (rune.Owner.Id != PlayerId)
                    throw new DomainException("You do not own the rune you are attempting to embed.");

                if (rune.ItemSource.ItemType != PvPStatics.ItemType_Rune)
                    throw new DomainException("Only runes can be embedded on items.");

                if (rune.EmbeddedOnItem != null)
                    throw new DomainException("This rune is already embedded on an item.");

                if (!itemToEmbedOn.CanAttachRunesToThisItemType())
                    throw new DomainException("You cannot embed a rune on this item type.");

                if (!itemToEmbedOn.HasRoomForRunes())
                    throw new DomainException("This item has no more room for additional runes.");

                if (!itemToEmbedOn.IsOfHighEnoughLevelForRune(rune))
                    throw new DomainException($"This item is of too low level to attach this rune.  It is level {itemToEmbedOn.Level} and needs to be at least level {rune.ItemSource.RuneLevel}.");

                itemToEmbedOn.AttachRune(rune);
                ctx.Update(itemToEmbedOn);
                ctx.Update(rune);
                ctx.Commit();

                result = $"You attached your {rune.ItemSource.FriendlyName} onto your {itemToEmbedOn.ItemSource.FriendlyName}.";
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (ItemId <= 0)
                throw new DomainException("ItemId is required");

            if (RuneId <= 0)
                throw new DomainException("RuneId is required");

            if (PlayerId <= 0)
                throw new DomainException("PlayerId is required");
        }

    }
}

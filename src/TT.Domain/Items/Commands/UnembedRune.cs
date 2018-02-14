using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Commands
{
    public class UnembedRune : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        public int ItemId { get; set; }

        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var rune = ctx.AsQueryable<Item>()
                    .Include(i => i.Owner)
                    .Include(i => i.ItemSource)
                    .Include(i => i.EmbeddedOnItem)
                    .Include(i => i.Runes.Select(r => r.EmbeddedOnItem))
                    .FirstOrDefault(i => i.Id == ItemId);

                if (rune == null)
                    throw new DomainException($"Rune with id '{ItemId}' could not be found!");

                if (rune.EmbeddedOnItem == null)
                    throw new DomainException("This rune is not currently embdded on an item.");


                if (rune.Owner == null || PlayerId != rune.Owner.Id)
                    throw new DomainException("You don't own this rune!");

                if (rune.EquippedThisTurn)
                    throw new DomainException("This rune was equipped this turn.  Wait until next turn to remove it.");

                rune.EmbeddedOnItem.RemoveRune(rune);
                ctx.Update(rune);
                ctx.Commit();

                result = $"You unembedded your <b>{rune.ItemSource.FriendlyName}</b>.";
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("PlayerId is required");

            if (ItemId <= 0)
                throw new DomainException("ItemId is required");
        }

    }
}

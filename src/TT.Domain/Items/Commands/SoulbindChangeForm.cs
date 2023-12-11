using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;


namespace TT.Domain.Items.Commands
{
    public class SoulbindChangeForm : DomainCommand
    {

        public int PlayerId { get; set; }
        public int? FormId { get; set; }
        public int FormSourceId { get; set; }
        public int? ItemId { get; set; }
        public int ItemSource { get; set; }
        public int OwnerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var owner = ctx.AsQueryable<Player>()
                    .Include(p => p.Item)
                    .Include(p => p.Item.Owner)
                    .SingleOrDefault(p => p.Id == OwnerId);

                var player = ctx.AsQueryable<Player>()
                    .SingleOrDefault(p => p.Id == PlayerId);

                var item = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.Owner)
                    .SingleOrDefault(i => i.Id == ItemId);


                if (player == null)
                    throw new DomainException($"Player with ID {PlayerId} could not be found");

                if (item == null)
                    throw new DomainException($"Item with FormerPlayerID {PlayerId} could not be found");

                if (item.Owner == null || item.Owner.Id != OwnerId)
                    throw new DomainException("You don't own that item.");

                if (!item.IsPermanent)
                    throw new DomainException("Only permanent items or pets may be reshaped.");

                if (item.FormerPlayer == null)
                    throw new DomainException("Only souled items may be reshaped.");

                if (!item.ConsentsToSoulbinding)
                    throw new DomainException("This item is not currently consenting to soulbinding.");

                FormSource formPlayer;
                if (FormSourceId <= 0)
                {
                    if (FormId == null)
                        throw new DomainException("FormId or FormName are required");

                    formPlayer = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.Id == FormId);
                    if (formPlayer == null)
                        throw new DomainException($"FormSource with ID {FormId} could not be found");
                }
                else
                {
                    formPlayer = ctx.AsQueryable<FormSource>().SingleOrDefault(cr => cr.Id == FormSourceId);
                    if (formPlayer == null)
                        throw new DomainException($"FormSource with id '{FormSourceId}' could not be found");
                }

                ItemSource formItem;
                if (ItemSource <= 0)
                {
                    if (ItemId == null)
                        throw new DomainException("ItemId is required");

                    formItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(cr => cr.Id == ItemId);
                    if (formItem == null)
                        throw new DomainException($"ItemSource with ID {ItemId} could not be found");
                }
                else
                {
                    formItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(cr => cr.Id == ItemSource);
                    if (formItem == null)
                        throw new DomainException($"ItemSource with id '{ItemSource}' could not be found");
                }

                var price = 300;
                if (owner.Money < price)
                    throw new DomainException($"You cannot afford this.  You need <b>{price}</b> Arpeyjis and only have <b>{Math.Floor(owner.Money)}</b>.");


                player.ChangeForm(formPlayer);
                item.ChangeItemForm(formItem);
                owner.ChangeMoney(-price);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}

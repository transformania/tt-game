using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Commands
{
    public class CreateItem : DomainCommand<int>
    {


        public string dbLocationName { get;  set; }
        public bool IsEquipped { get;  set; }
        public int TurnsUntilUse { get;  set; }
        public int Level { get;  set; }
        public DateTime TimeDropped { get;  set; }
        public bool EquippedThisTurn { get;  set; }
        public int PvPEnabled { get;  set; }
        public bool IsPermanent { get;  set; }
        public DateTime LastSouledTimestamp { get;  set; }
        public DateTime LastSold { get;  set; }

        public int ItemSourceId { get; set; }
        public int? OwnerId { get; set; }
        public int? FormerPlayerId { get; set; }

        public CreateItem()
        {
            dbLocationName = "";
            IsEquipped = false;
            TurnsUntilUse = 0;
            Level = 1;
            TimeDropped = DateTime.UtcNow;
            EquippedThisTurn = false;
            PvPEnabled = -1;
            IsPermanent = false;
            LastSouledTimestamp = DateTime.UtcNow;
            LastSold = DateTime.UtcNow;
            FormerPlayerId = null;
        }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var itemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == ItemSourceId);
                if (itemSource == null)
                    throw new DomainException($"Item Source with Id {ItemSourceId} could not be found");

                var owner = ctx.AsQueryable<Player>().SingleOrDefault(t => t.Id == OwnerId);

                if (OwnerId != null && owner == null)
                {
                    throw new DomainException($"Player with Id {OwnerId} could not be found");
                }


                Player formerPlayer = null;
                if (FormerPlayerId != null) { 
                    formerPlayer = ctx.AsQueryable<Player>().SingleOrDefault(t => t.Id == FormerPlayerId);

                    if (formerPlayer == null)
                        throw new DomainException($"Former player with Id {FormerPlayerId} could not be found");
                }


                var item = Item.Create(owner, formerPlayer, itemSource, this);

                ctx.Add(item);
                ctx.Commit();

                result = item.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            
        }

    }
}

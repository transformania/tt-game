using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.Items;
using TT.Domain.Entities.Players;

namespace TT.Domain.Commands.Items
{
    public class CreateItem : DomainCommand<int>
    {

        public string dbName { get;  set; }

        public string dbLocationName { get;  set; }
        public string VictimName { get;  set; }
        public bool IsEquipped { get;  set; }
        public int TurnsUntilUse { get;  set; }
        public int Level { get;  set; }
        public DateTime TimeDropped { get;  set; }
        public bool EquippedThisTurn { get;  set; }
        public int PvPEnabled { get;  set; }
        public bool IsPermanent { get;  set; }
        public string Nickname { get;  set; }
        public DateTime LastSouledTimestamp { get;  set; }
        public DateTime LastSold { get;  set; }

        public int ItemSourceId { get; set; }
        public int? OwnerId { get; set; }

        public CreateItem()
        {
            dbLocationName = "";
            dbName = "";
            VictimName = "";
            IsEquipped = false;
            TurnsUntilUse = 0;
            Level = 1;
            TimeDropped = DateTime.UtcNow;
            EquippedThisTurn = false;
            PvPEnabled = 0;
            IsPermanent = false;
            Nickname = "";
            LastSouledTimestamp = DateTime.UtcNow;
            LastSold = DateTime.UtcNow;
        }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var itemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == ItemSourceId);
                if (itemSource == null)
                    throw new DomainException($"Item Source with Id {ItemSourceId} could not be found");

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == OwnerId);

                if (OwnerId != null && player == null)
                {
                    throw new DomainException($"Player with Id {OwnerId} could not be found");
                }


                var item = Item.Create(player, itemSource, this);

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

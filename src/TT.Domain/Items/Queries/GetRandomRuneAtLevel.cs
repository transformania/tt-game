using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries
{
    public class GetRandomRuneAtLevel : DomainQuerySingle<int>
    {
        public int RuneLevel { get; set; }
        public Random Random { get; set; }

        public override int Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var runes = ctx.AsQueryable<ItemSource>()
                    .Where(r => r.ItemType == PvPStatics.ItemType_Rune && r.RuneLevel == RuneLevel)
                    .Select(r => r.Id).ToList();

                if (Random == null)
                {
                    Random = new Random(Guid.NewGuid().GetHashCode());
                }
                

                return runes.ElementAt((int) Math.Floor(runes.Count * Random.NextDouble()));

            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RuneLevel <= 0)
                throw new DomainException("RuneLevel is required");

            int[] validRuneLevels = {1, 3, 5, 7, 9, 11, 13};

            if (!validRuneLevels.Contains(RuneLevel))
                throw new DomainException($"RuneLevel '{RuneLevel}' is not a valid level.");
        }
    }
}

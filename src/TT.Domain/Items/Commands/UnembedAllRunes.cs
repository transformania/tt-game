using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class UnembedAllRunes : DomainCommand<string>
    {

        public int PlayerId { get; set; }

        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var playerItemsWithRunes = ctx.AsQueryable<Item>()
                    .Include(i => i.Owner)
                    .Include(i => i.ItemSource)
                    .Include(i => i.Runes)
                    .Where(i => i.Owner.Id == PlayerId &&
                        i.Runes.Count > 0 );

                var itemCount = playerItemsWithRunes.Count();

                foreach (var item in playerItemsWithRunes)
                {
                    item.RemoveRunes();
                    ctx.Update(item);
                }

                ctx.Commit();

                result = $"You removed the runes from {itemCount} of your belongings.";
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("PlayerId is required");
        }

    }
}

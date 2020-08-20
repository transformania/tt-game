using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

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

                var itemCount = 0;

                foreach (var item in playerItemsWithRunes)
                {
                    item.RemoveRunes();
                    ctx.Update(item);
                    ++itemCount;
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

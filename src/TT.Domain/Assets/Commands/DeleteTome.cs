using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.Assets.Commands
{
    public class DeleteTome : DomainCommand
    {
        public int TomeId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Tome>().FirstOrDefault(cr => cr.Id == TomeId);

                if (deleteMe == null)
                    throw new DomainException($"Tome with ID {TomeId} was not found");

                ctx.Remove(deleteMe);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (TomeId <= 0)
                throw new DomainException("Tome Id must be greater than 0");
        }
    }
}

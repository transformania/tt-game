using Highway.Data;
using TT.Domain.World.DTOs;

namespace TT.Domain.World.Queries
{
    public class GetWorld : DomainQuerySingle<WorldDetail>
    {

        public override WorldDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.World>()
                            .ProjectToFirstOrDefault<WorldDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}

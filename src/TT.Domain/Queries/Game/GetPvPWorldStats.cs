using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Game;
using TT.Domain.Entities.Game;

namespace TT.Domain.Queries.Game
{
    public class GetWorld : DomainQuerySingle<WorldDetail>
    {

        public override WorldDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<World>()
                            .ProjectToFirstOrDefault<WorldDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}

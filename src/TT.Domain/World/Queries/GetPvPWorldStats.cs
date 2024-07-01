using System.Linq;
using Highway.Data;
using TT.Domain.World.DTOs;

namespace TT.Domain.World.Queries
{
    public class GetWorld : DomainQuerySingle<WorldDetail>
    {

        public override WorldDetail Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Entities.World>().FirstOrDefault()?.MapToDto();

            return ExecuteInternal(context);
        }
    }
}

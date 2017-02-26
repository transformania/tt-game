using System.Linq;
using Highway.Data;
using TT.Domain.AI.DTOs;
using TT.Domain.AI.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.AI.Queries
{
    public class GetNPC : DomainQuerySingle<NPCDetail>
    {
        public int NPCId { get; set; }

        public override NPCDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<NPC>()
                            .Where(cr => cr.Id == NPCId)
                            .ProjectToFirstOrDefault<NPCDetail>();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (NPCId <= 0)
                throw new DomainException("NPC Id must be greater than 0");
        }
    }
}

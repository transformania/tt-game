using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.World.DTOs;

namespace TT.Domain.World.Queries
{
    public class GetLocationLogsAtLocation : DomainQuery<LocationLogDetail>
    {

        public string Location { get; set; }
        public int ConcealmentLevel { get; set; }

        public override IEnumerable<LocationLogDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<LocationLog>()
            .Where(l => l.dbLocationName == Location && l.ConcealmentLevel <= ConcealmentLevel)
            .OrderByDescending(l => l.Timestamp)
            .ThenByDescending(l => l.Id)
            .ProjectToQueryable<LocationLogDetail>();
            return ExecuteInternal(context);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.LocationLog;
using TT.Domain.Entities.LocationLogs;

namespace TT.Domain.Queries.LocationLogs
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
            .ProjectToQueryable<LocationLogDetail>();
            return ExecuteInternal(context);
        }
    }
}

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetAllReports : DomainQuery<ReportDetail>
    {

        public override IEnumerable<ReportDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var reports = ctx.AsQueryable<Report>()
                        .Include(r => r.Reporter)
                        .Include(r => r.Reported)
                        .OrderByDescending(s => s.Timestamp)
                        .ToList();

                    return reports.Select(r => r.MapToDto()).AsQueryable();
                };

            return ExecuteInternal(context);
        }
    }
}

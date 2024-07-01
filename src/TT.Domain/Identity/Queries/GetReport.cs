using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetReport : DomainQuerySingle<ReportDetail>
    {

        public int ReportId { get; set; }

        public override ReportDetail Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var report = ctx
                        .AsQueryable<Report>()
                        .Include(r => r.Reporter)
                        .Include(r => r.Reported)
                        .OrderByDescending(s => s.Timestamp)
                        .SingleOrDefault(r => r.Id == ReportId);

                    return report?.MapToDto();
                };

            return ExecuteInternal(context);
        }
    }
}

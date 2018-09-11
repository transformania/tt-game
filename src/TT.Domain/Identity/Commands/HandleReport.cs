using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{
    public class HandleReport : DomainCommand
    {

        public int ReportId { get; set; }
        public string ModeratorResponse { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {

                var report = ctx.AsQueryable<Report>().SingleOrDefault(r => r.Id == ReportId);

                if (report == null)
                    throw new DomainException($"Report with Id '{ReportId}' could not be found");

                report.SetModeratorResponse(ModeratorResponse);

                ctx.Update(report);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }

        protected override void Validate()
        {
            if (ReportId <= 0)
                throw new DomainException("ReportId must be a positive integer greater than 0");
        }

    }
}

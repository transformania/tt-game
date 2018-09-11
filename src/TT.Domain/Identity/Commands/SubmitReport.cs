using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Identity.Commands
{
    public class SubmitReport : DomainCommand
    {
        public string ReporterId { get; set; }
        public string ReportedId { get; set; }
        public string Reason { get; set; }

        public int Round { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var reporter = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == ReporterId);
                if (reporter == null)
                    throw new DomainException($"Reporting user with Id '{ReporterId}' could not be found");

                var reported = ctx.AsQueryable<Player>()
                    .Include(p => p.User)
                    .SingleOrDefault(t => t.User.Id == ReportedId);

                if (reported == null)
                    throw new DomainException($"Reported user with Id '{ReportedId}' could not be found");

                var entry = Report.Create(reporter, reported.User, Reason, Round);

                ctx.Add(entry);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Reason))
                throw new DomainException("Reason for report is required");

            if (Round <= 0)
                throw new DomainException("Round must be a positive integer greater than 0");
        }
    }
}

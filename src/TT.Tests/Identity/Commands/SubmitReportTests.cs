using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class SubmitReportTests : TestBase
    {
        public Player reportedPlayer;
        public User reporter;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            reporter = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            reportedPlayer = new PlayerBuilder()
                .With(p => p.Id, 123)
                .With(p => p.User, new UserBuilder()
                           .With(p => p.Id, "user2")
                           .With(p => p.UserName, "Bob")
                           .BuildAndSave())
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();
        }

        [Test]
        public void can_submit_report_when_target_is_valid() { 
        
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = reportedPlayer.User.Id, Reason = "Did stuff", Round = 50 };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var report = DataContext.AsQueryable<Report>().FirstOrDefault();
            Assert.That(report, Is.Not.Null);
            Assert.That(report.Reporter.Id, Is.EqualTo(reporter.Id));
            Assert.That(report.Reported.Id, Is.EqualTo(reportedPlayer.User.Id));
            Assert.That(report.Round, Is.EqualTo(50));
            Assert.That(report.Reason, Is.EqualTo("Did stuff"));
        }

        [Test]
        public void should_throw_exception_if_reporter_not_found()
        {
            var cmd = new SubmitReport { ReporterId = "Fake", ReportedId = reportedPlayer.User.Id, Reason = "Did stuff", Round = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Reporting user with Id 'Fake' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_reported_not_found()
        {
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = "Fake", Reason = "Did stuff", Round = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Reported user with Id 'Fake' could not be found"));
        }

        [Test]
        public void should_throw_exception_no_reason()
        {
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = reportedPlayer.User.Id, Reason = null, Round = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Reason for report is required"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void should_throw_exception_if_round_invalid(int round)
        {
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = reportedPlayer.User.Id, Reason = "derP", Round = round };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Round must be a positive integer greater than 0"));
        }
    }
}


using System;
using System.Linq;
using FluentAssertions;
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
            DomainRegistry.Repository.Execute(cmd);

            var report = DataContext.AsQueryable<Report>().First();
            report.Reporter.Id.Should().Be(reporter.Id);
            report.Reported.Id.Should().Be(reportedPlayer.User.Id);
            report.Round.Should().Be(50);
            report.Reason.Should().Be("Did stuff");
        }

        [Test]
        public void should_throw_exception_if_reporter_not_found()
        {
            var cmd = new SubmitReport { ReporterId = "Fake", ReportedId = reportedPlayer.User.Id, Reason = "Did stuff", Round = 50 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Reporting user with Id 'Fake' could not be found");
        }

        [Test]
        public void should_throw_exception_if_reported_not_found()
        {
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = "Fake", Reason = "Did stuff", Round = 50 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Reported user with Id 'Fake' could not be found");
        }

        [Test]
        public void should_throw_exception_no_reason()
        {
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = reportedPlayer.User.Id, Reason = null, Round = 50 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Reason for report is required");
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void should_throw_exception_if_round_invalid(int round)
        {
            var cmd = new SubmitReport { ReporterId = reporter.Id, ReportedId = reportedPlayer.User.Id, Reason = "derP", Round = round };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Round must be a positive integer greater than 0");
        }
    }
}


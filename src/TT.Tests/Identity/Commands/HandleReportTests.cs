using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class HandleReportTests : TestBase
    {
        //public Player reportedPlayer;
        //public User reporter;

        public Report report;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            report = new ReportBuilder()
                .With(r => r.Id, 100)
                .With(r => r.Reason, "things")
                .BuildAndSave();
        }

        [Test]
        public void can_handle_report()
        {
            var cmd = new HandleReport { ReportId = this.report.Id, ModeratorResponse = "handled!"};
            DomainRegistry.Repository.Execute(cmd);

            var report = DataContext.AsQueryable<Report>().First();
            report.ModeratorResponse.Should().Be("handled!");
        }

        [Test]
        public void should_throw_exception_if_report_not_provided()
        {
            var cmd = new HandleReport { ModeratorResponse = "handled!" };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("ReportId must be a positive integer greater than 0");
        }

        [Test]
        public void should_throw_exception_if_report_not_found()
        {
            var cmd = new HandleReport { ModeratorResponse = "handled!", ReportId = 999};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Report with Id '999' could not be found");
        }

    }
}

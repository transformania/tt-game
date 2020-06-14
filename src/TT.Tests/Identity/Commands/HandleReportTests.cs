using System.Linq;
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
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Report>().First().ModeratorResponse, Is.EqualTo("handled!"));
        }

        [Test]
        public void should_throw_exception_if_report_not_provided()
        {
            var cmd = new HandleReport { ModeratorResponse = "handled!" };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ReportId must be a positive integer greater than 0"));
        }

        [Test]
        public void should_throw_exception_if_report_not_found()
        {
            var cmd = new HandleReport { ModeratorResponse = "handled!", ReportId = 999};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Report with Id '999' could not be found"));
        }

    }
}

using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TT.Domain;
using TT.Domain.Players.Entities;

namespace TT.IntegrationTests.BasicTests
{
    [Category("BasicIntegrationTest")]
    public class TestingClass : TransactionalTestBase
    {
        [Test]
        public void TestUploadAndFKAssociation()
        {
            LoadTableFromCSV("Players", "CSVsWithFKs");
            LoadTableFromCSV("AspNetUsers", "CSVsWithFKs");

            Player player = DomainRegistry.Repository.Context.AsQueryable<Player>()
                .Where(p => p.FirstName == "John" && p.LastName == "Smith")
                .Include(p => p.User)
                .First();
            Assert.That(player.User, Is.Not.Null);
        }

        [Test]
        public void TestNullAndEscapingStringValues()
        {
            LoadTableFromCSV("Players", "CSVsWithNulls");

            List<string> playersMembershipIds = DomainRegistry.Repository.Context.AsQueryable<Player>().Select(p => p.MembershipId).ToList();

            Assert.That(playersMembershipIds, Is.Not.Empty);
            Assert.That(playersMembershipIds.First(), Is.Null);
            Assert.That(playersMembershipIds, Has.Exactly(6).Items);
            Assert.That(playersMembershipIds.Skip(1), Has.None.Null);
        }
    }
}

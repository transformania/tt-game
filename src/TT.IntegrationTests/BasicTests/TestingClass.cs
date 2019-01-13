using FluentAssertions;
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
            player.User.Should().NotBeNull();
        }

        [Test]
        public void TestNullAndEscapingStringValues()
        {
            LoadTableFromCSV("Players", "CSVsWithNulls");

            List<string> playersMembershipIds = DomainRegistry.Repository.Context.AsQueryable<Player>().Select(p => p.MembershipId).ToList();

            playersMembershipIds.Should().NotBeEmpty();
            playersMembershipIds.First().Should().BeNull();
            playersMembershipIds.Should().HaveCount(6);
            playersMembershipIds.Skip(1).Should().NotContainNulls();
        }
    }
}

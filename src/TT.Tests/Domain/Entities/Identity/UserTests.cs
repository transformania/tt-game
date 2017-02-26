using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Identity.Entities;
using TT.Domain.Procedures;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Domain.Entities.Identity
{
    [TestFixture]
    public class UserTests : TestBase
    {
        [Test]
        public void can_add_stat_to_existing_stat()
        {

            var stats = new List<Stat>()
            {
                new StatBuilder()
                .With(t => t.Amount, 50)
                .With(t => t.AchievementType, StatsProcedures.Stat__TimesCleansed)
                .BuildAndSave()
            };

            var user = new UserBuilder()
                .With(p => p.Id, "abcde")
                .With(p => p.Stats, stats)
                .BuildAndSave();

            user.AddStat(StatsProcedures.Stat__TimesCleansed, 100);
            user.Stats.First().Amount.Should().Be(150);
        }

        [Test]
        public void can_add_stat_to_no_new_stat()
        {

            var user = new UserBuilder()
                .With(p => p.Id, "abcde")
                .With(p => p.Stats, new List<Stat>())
                .BuildAndSave();

            user.AddStat(StatsProcedures.Stat__TimesCleansed, 100);
            user.Stats.First().Amount.Should().Be(100);
        }

    }
}

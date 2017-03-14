using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.World.Queries;
using TT.Tests.Builders.LocationLogs;

namespace TT.Tests.World.Queries
{
    [TestFixture]
    public class GetLocationLogsAtLocationTest : TestBase
    {
        [Test]
        public void get_location_logs_at_location()
        {

            // valid
            new LocationLogBuilder()
                .With(i => i.Id, 1)
                .With(l => l.dbLocationName,"pluto")
                .With(i => i.ConcealmentLevel, 35)
                .With(l => l.Timestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            // invalid; concealment level too high
            new LocationLogBuilder()
                .With(i => i.Id, 2)
                .With(l => l.dbLocationName, "pluto")
                .With(i => i.ConcealmentLevel, 75)
                .With(l => l.Timestamp, DateTime.UtcNow.AddHours(-1))
                .BuildAndSave();

            // valid
            new LocationLogBuilder()
                .With(i => i.Id, 3)
                .With(l => l.dbLocationName, "pluto")
                .With(i => i.ConcealmentLevel, 45)
                .With(l => l.Timestamp, DateTime.UtcNow.AddHours(-2))
                .BuildAndSave();

            // invalid; different location
            new LocationLogBuilder()
                .With(i => i.Id, 4)
                .With(l => l.dbLocationName, "mars")
                .With(i => i.ConcealmentLevel, 5)
                .With(l => l.Timestamp, DateTime.UtcNow)
                .BuildAndSave();

            var cmd = new GetLocationLogsAtLocation {Location = "pluto", ConcealmentLevel = 50};
            var logs = DomainRegistry.Repository.Find(cmd).ToArray();

            logs.Length.Should().Be(2);
            logs[0].Id.Should().Be(3);
            logs[1].Id.Should().Be(1);

        }
    }
}

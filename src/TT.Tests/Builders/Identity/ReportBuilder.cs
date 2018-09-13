using System;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class ReportBuilder : Builder<Report, int>
    {
        public ReportBuilder()
        {
            Instance = Create();
            With(u => u.Timestamp, DateTime.UtcNow);
        }
    }
}

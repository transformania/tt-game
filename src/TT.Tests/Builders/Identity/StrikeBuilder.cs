using System;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class StrikeBuilder : Builder<Strike, int>
    {
        public StrikeBuilder()
        {
            Instance = Create();
            With(u => u.Timestamp, DateTime.UtcNow);
        }
    }
}
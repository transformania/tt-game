using System;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class CaptchaEntryBuilder : Builder<CaptchaEntry, int>
    {
        public CaptchaEntryBuilder()
        {
            Instance = Create();
            With(u => u.Id, 7);
            With(u => u.ExpirationTimestamp, DateTime.UtcNow);
        }
    }
}

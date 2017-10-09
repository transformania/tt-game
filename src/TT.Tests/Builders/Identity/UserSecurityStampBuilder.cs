using System;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class UserSecurityStampBuilder : Builder<UserSecurityStamp, string>
    {
        public UserSecurityStampBuilder()
        {
            Instance = Create();
            With(s => s.SecurityStamp, Guid.NewGuid().ToString());
        }

        public void AssignToUser(User user)
        {
            With(s => s.User, user);
            With(s => s.Id, user.Id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void AssignToUser(UserBuilder userBuilder)
        {
            var userInstance = userBuilder.Build();

            With(s => s.User, userInstance);
            With(s => s.Id, userInstance.Id);

            var userSecurityStampInstance = Build();

            userBuilder.With(u => u.SecurityStamp, userSecurityStampInstance);
        }
    }
}

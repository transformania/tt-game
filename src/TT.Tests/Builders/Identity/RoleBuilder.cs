using System.Collections.Generic;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class RoleBuilder : Builder<Role, int>
    {
        public RoleBuilder()
        {
            Instance = Create();
            With(r => r.Users, new List<User>());
        }

        public void AddUser(User user)
        {
            Instance.Users.Add(user);
        }
    }
}

using System;
using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class UserBuilder : Builder<User, string>
    {
        public UserBuilder()
        {
            Instance = Create();
            With(u => u.Id, Guid.NewGuid().ToString());
            With(u => u.UserName, "Test User");
            With(u => u.Email, "test@email.com");
        }
    }
}
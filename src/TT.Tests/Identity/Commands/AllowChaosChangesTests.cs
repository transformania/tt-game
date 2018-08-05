using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class AllowChaosChangesTests : TestBase
    {

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_allow_chaos_changes(bool allowChanges)
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.AllowChaosChanges, !allowChanges)
                .BuildAndSave();

            var cmd = new AllowChaosChanges {UserId = "user", ChaosChangesEnabled = allowChanges};
            DomainRegistry.Repository.Execute(cmd);

            user = DataContext.AsQueryable<User>().First();
            user.AllowChaosChanges.Should().Be(allowChanges);

        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            new UserBuilder()
                .With(p => p.Id, "user")
                .BuildAndSave();

            var cmd = new AllowChaosChanges { UserId = "fake", ChaosChangesEnabled = true };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("User with Id 'fake' could not be found");
        }

    }
}

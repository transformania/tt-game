using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Domain.Commands.Identity
{
    [TestFixture]
    public class CreateCaptchaEntryTests : TestBase
    {
        [Test]
        public void can_create_captcha_entry()
        {
            new UserBuilder()
                .With(p => p.Id, "abcde")
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            var cmd = new CreateCaptchaEntry { UserId = "abcde" };
            DomainRegistry.Repository.Execute(cmd);

            DataContext.AsQueryable<CaptchaEntry>().Count(p =>
                p.User.Id == "abcde" &&
                p.User.UserName == "Bob" &&
                p.TimesFailed == 0 &&
                p.TimesPassed == 0)
            .Should().Be(1);

        }

        [Test]
        public void should_throw_error_if_player_not_found()
        {
            var cmd = new CreateCaptchaEntry() { UserId = "abcde" };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("User with Id abcde could not be found");
        }

    }
}

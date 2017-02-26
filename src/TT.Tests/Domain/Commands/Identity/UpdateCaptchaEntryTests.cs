using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Identity
{
    [TestFixture]
    public class UpdateCaptchaEntryTests : TestBase
    {
        [Test]
        public void can_add_captcha_fail()
        {
            new CaptchaEntryBuilder()
                .With(p => p.Id, 5)
                .With(p => p.User, new UserBuilder()
                    .With(p => p.Id, "abcde")
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new UpdateCaptchaEntry {UserId = "abcde", AddPassAttempt = false, AddFailAttempt = true};
            DomainRegistry.Repository.Execute(cmd);

            var entry = DataContext.AsQueryable<CaptchaEntry>().First(e => e.User.Id == "abcde");
            entry.TimesFailed.Should().Be(1);
            entry.TimesPassed.Should().Be(0);
        }

        [Test]
        public void can_add_captcha_pass()
        {
            new CaptchaEntryBuilder()
                .With(p => p.Id, 5)
                .With(p => p.User, new UserBuilder()
                    .With(p => p.Id, "abcde")
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new UpdateCaptchaEntry {UserId = "abcde", AddPassAttempt = true, AddFailAttempt = false};
            DomainRegistry.Repository.Execute(cmd);

            var entry = DataContext.AsQueryable<CaptchaEntry>().First(e => e.User.Id == "abcde");
            entry.TimesFailed.Should().Be(0);
            entry.TimesPassed.Should().Be(1);
            entry.ExpirationTimestamp.Should().BeCloseTo(DateTime.UtcNow.AddHours(24).AddMinutes(30), 250);
        }

        [Test]
        public void should_throw_exception_if_user_has_no_captcha_entry()
        {
            var cmd = new UpdateCaptchaEntry {UserId = "abcde", AddPassAttempt = true, AddFailAttempt = false};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("CaptchaEntry with Id abcde could not be found");
        }
    }
}

using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
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
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var entry = DataContext.AsQueryable<CaptchaEntry>().FirstOrDefault(e => e.User.Id == "abcde");
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.TimesFailed, Is.EqualTo(1));
            Assert.That(entry.TimesPassed, Is.EqualTo(0));
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
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var entry = DataContext.AsQueryable<CaptchaEntry>().FirstOrDefault(e => e.User.Id == "abcde");
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.TimesFailed, Is.EqualTo(0));
            Assert.That(entry.TimesPassed, Is.EqualTo(1));
            Assert.That(entry.ExpirationTimestamp,
                Is.EqualTo(DateTime.UtcNow.AddHours(24).AddMinutes(30)).Within(250).Milliseconds);
        }

        [Test]
        public void should_throw_exception_if_user_has_no_captcha_entry()
        {
            var cmd = new UpdateCaptchaEntry {UserId = "abcde", AddPassAttempt = true, AddFailAttempt = false};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("CaptchaEntry with Id abcde could not be found"));
        }
    }
}

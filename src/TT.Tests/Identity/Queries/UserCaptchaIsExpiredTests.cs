using System;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class UserCaptchaIsExpiredTests : TestBase
    {

        [Test]
        public void return_true_if_user_has_expired_captcha()
        {
            new CaptchaEntryBuilder().With(i => i.Id, 77)
                .With(cr => cr.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .BuildAndSave())
                    .With(u => u.ExpirationTimestamp, DateTime.UtcNow.AddMinutes(-1))
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired {UserId = "abcde"}), Is.True);
        }

        [Test]
        public void return_false_if_user_captcha_not_expired()
        {
            new CaptchaEntryBuilder().With(i => i.Id, 77)
                .With(cr => cr.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .BuildAndSave())
                    .With(u => u.ExpirationTimestamp, DateTime.UtcNow.AddMinutes(1))
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired {UserId = "abcde"}), Is.False);
        }

        [Test]
        public void throw_exception_if_captcha_doesnt_exist()
        {
            var cmd = new UserCaptchaIsExpired {UserId = "abcde"};
            Assert.That(() => Repository.FindSingle(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id abcde has no CaptchaEntry"));
        }

    }
}

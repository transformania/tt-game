using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class GetCaptchaEntryTests : TestBase
    {
        [Test]
        public void should_get_captcha_entry()
        {
            new CaptchaEntryBuilder().With(i => i.Id, 77)
                .With(cr => cr.User, new UserBuilder()
                    .With(u => u.Id, "abcde")
                    .With(u => u.UserName, "Bob")
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new GetCaptchaEntry { UserId = "abcde" };

            var entry = DomainRegistry.Repository.FindSingle(cmd);

            Assert.That(entry.Id, Is.EqualTo(77));
            Assert.That(entry.User.Id, Is.EqualTo("abcde"));
            Assert.That(entry.User.UserName, Is.EqualTo("Bob"));
        }
    }
}

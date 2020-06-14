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
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<CaptchaEntry>().Where(p =>
                p.User.Id == "abcde" &&
                p.User.UserName == "Bob" &&
                p.TimesFailed == 0 &&
                p.TimesPassed == 0), Has.Exactly(1).Items);
        }

        [Test]
        public void should_throw_error_if_player_not_found()
        {
            var cmd = new CreateCaptchaEntry() { UserId = "abcde" };
            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id abcde could not be found"));
        }

    }
}

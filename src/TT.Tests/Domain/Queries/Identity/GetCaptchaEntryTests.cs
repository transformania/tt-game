using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Domain.Queries.Identity
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

            entry.Id.Should().Be(77);
            entry.User.Id.Should().Be("abcde");
            entry.User.UserName.Should().Be("Bob");
        }
    }
}

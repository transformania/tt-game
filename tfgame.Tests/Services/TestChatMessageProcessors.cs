using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using FluentAssertions;
using NUnit.Framework;
using tfgame.Services;
using tfgame.Statics;

namespace tfgame.Tests.Services
{
    [TestFixture]
    public class TestRegularTextProcessor
    {
        [Test]
        public void Should_append_time_stamp_to_regular_message()
        {
            var data = new MessageData("Tester", "Testing");
            new RegularTextProcessor().Process(data);

            data.Output.Should().Be(string.Format("{0}   [.[{1}].]", "Testing", DateTime.UtcNow.ToShortTimeString()));
        }

        [TestCase("")]
        [TestCase(" ")]
        public void Should_not_process_blank_message(string message)
        {
            var data = new MessageData("Tester", message);
            new RegularTextProcessor().Process(data);

            data.Output.Should().BeNull();
        }

        [TestCase("")]
        [TestCase(" ")]
        public void Should_not_process_if_sender_name_is_blank(string name)
        {
            var data = new MessageData(name, "Testing");
            new RegularTextProcessor().Process(data);

            data.Output.Should().BeNull();
        }
    }

    [TestFixture]
    public class TestReservedTextProcessor
    {
        [SetUp]
        public void SetUp()
        {
            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://tempuri.org", null),new HttpResponse(null));
        }

        [TearDown]
        public void TearDown()
        {
            HttpContext.Current = null;
        }
        
        [TestCase(PvPStatics.Permissions_Admin)]
        [TestCase(PvPStatics.Permissions_Moderator)]
        public void Should_format_reserved_text_used_by_privileged_users(string permission)
        {
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("Tester"), new[] {permission});

            foreach (var data in ChatStatics.ReservedText.Select(reservedText => new MessageData("Tester", reservedText)))
            {
                new ReservedTextProcessor().Process(data);
                data.Output.Should().Be(string.Format("{0}   [.[{1}].]", data.Message, DateTime.UtcNow.ToShortTimeString()));
            }
        }

        [Test]
        public void Should_strip_reserved_text_from_message_by_non_priviledged_users()
        {
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("Tester"), new string[] {});

            foreach (var data in ChatStatics.ReservedText.Select(reservedText => new MessageData("Tester", reservedText)))
            {
                new ReservedTextProcessor().Process(data);
                data.Output.Should().Be(string.Format("    [.[{0}].]", DateTime.UtcNow.ToShortTimeString()));
            }
        }
    }

    [TestFixture]
    public class TestDmTextProcessor
    {
        [Test]
        public void Should_format_dm_message_text()
        {
            var data = new MessageData("Tester", "/dm message something happens");
            new DmMessageTextProcessor().Process(data);

            data.Output.Should().Be(string.Format("[=[{0} [DM]:   {1}]=]", "Tester", "something happens"));
        }

        [Test]
        public void Should_not_process_message_without_trigger()
        {
            var data = new MessageData("Tester", "/not dm message something happens");

            new DmMessageTextProcessor().Process(data);
            data.Processed.Should().BeFalse();
        }
    }
}
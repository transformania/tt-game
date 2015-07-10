using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using tfgame.dbModels;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Queries.DMRoll;
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

        [Test]
        public void Should_not_process_message_without_trigger()
        {
            var data = new MessageData("Tester", "[notluxa]");
            new ReservedTextProcessor().Process(data);

            data.Processed.Should().BeFalse();
        }
    }

    [TestFixture]
    public class TestDmMessageTextProcessor
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

    [TestFixture]
    public class TestDmActionTextProcessor
    {
        [TestCase("creature", "forest")]
        [TestCase("item", "highschool")]
        [TestCase("event", "bimbocalypse")]
        [TestCase("trap", "latexfactory")]
        [TestCase("tf.animate", "highschool")]
        [TestCase("tf.inanimate", "forest")]
        [TestCase("tf.animal", "bimbocalypse")]
        [TestCase("tf.partial", "latexfactory")]
        public void Should_format_dm_rp_command(string actionType, string tag)
        {
            var rollOutput = string.Format("DM[{0}:{1}]:  {2}", actionType, tag, "Wibble happens");

            DomainRegistry.Root = Substitute.For<IRoot>();
            DomainRegistry.Root.Find(Arg.Any<GetRollText>()).Returns(rollOutput);

            var data = new MessageData("Tester", string.Format("/dm {0}:{1}", actionType, tag));
            
            new DmActionTextProcessor().Process(data);

            data.Output.Should().Be(string.Format("[=[{0}]=]", rollOutput));
            DomainRegistry.Root.Received().Find(Arg.Is<GetRollText>(cmd => cmd.ActionType == actionType && cmd.Tag == tag));
        }
        
        [Test]
        public void Should_not_process_message_without_trigger()
        {
            var data = new MessageData("Tester", "/not dm creature:forest");

            new DmActionTextProcessor().Process(data);
            data.Processed.Should().BeFalse();
        }

        [Test]
        public void Should_not_process_message_with_invalid_action_type()
        {
            var data = new MessageData("Tester", "/dm wibble:forest");

            new DmActionTextProcessor().Process(data);
            data.Processed.Should().BeFalse();
        }

        [Test]
        public void Should_not_process_message_with_invalid_tag()
        {
            var data = new MessageData("Tester", "/dm creature:wibble");

            new DmActionTextProcessor().Process(data);
            data.Processed.Should().BeFalse();
        }
    }

    [TestFixture]
    public class TestRollTextProcessor
    {
        [Test]
        public void Should_format_die_roll()
        {
            var data = new MessageData("Tester", "/roll d1");
            new RollTextProcessor().Process(data);

            data.Output.Should().Be(string.Format("[-[{0} rolled a 1 (d1).]-]", data.Name));    
        }

        [Test]
        public void Should_not_process_message_without_trigger()
        {
            var data = new MessageData("Tester", "/not roll d1");

            new RollTextProcessor().Process(data);
            data.Processed.Should().BeFalse();
        }

        [TestCase("/roll d")]
        [TestCase("/roll 20")]
        public void Should_not_process_message_without_die_value(string message)
        {
            var data = new MessageData("Tester", message);

            new RollTextProcessor().Process(data);
            data.Processed.Should().BeFalse();
        }
    }

    [TestFixture]
    public class TestPlayerActionTextProcessor
    {
        [Test]
        public void Should_format_action_text()
        {
            var data = new MessageData("Tester", "/me does something");

            new PlayerActionTextProcessor().Process(data);

            data.Output.Should().Be(string.Format("[+[{0} {1}]+]", data.Name, "does something"));
        }

        [Test]
        public void Should_not_process_message_without_trigger()
        {
            var data = new MessageData("Tester", "/not me does something");
            new PlayerActionTextProcessor().Process(data);

            data.Processed.Should().BeFalse();
        }
    }
}
using System.Linq;
using System.Security.Principal;
using System.Web;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Queries.DMRoll;
using TT.Domain.Statics;
using TT.Web.Services;

namespace TT.Tests.Services
{
    [TestFixture]
    public class TestRegularTextProcessor
    {
        [Test]
        public void Should_process_regular_message()
        {
            var data = new MessageData("Tester", "Testing");
            new RegularTextProcessor().Process(data);

            data.Output.Text.Should().Be("Testing");
            data.Output.SendPlayerChatColor.Should().BeTrue();
            data.Output.MessageType.Should().Be(MessageType.RegularText);
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
                data.Output.Text.Should().Be(data.Message);
                data.Output.SendPlayerChatColor.Should().BeTrue();
                data.Output.MessageType.Should().Be(MessageType.ReservedText);
            }
        }

        [Test]
        public void Should_strip_reserved_text_from_message_by_non_priviledged_users()
        {
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("Tester"), new string[] {});

            foreach (var data in ChatStatics.ReservedText.Select(reservedText => new MessageData("Tester", reservedText)))
            {
                new ReservedTextProcessor().Process(data);
                data.Output.Text.Should().Be(" ");
                data.Output.SendPlayerChatColor.Should().BeTrue();
                data.Output.MessageType.Should().Be(MessageType.RegularText);
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

            data.Output.Text.Should().Be(" something happens");
            data.Output.SendPlayerChatColor.Should().BeTrue();
            data.Output.MessageType.Should().Be(MessageType.DmMessage);
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

            data.Output.Text.Should().Be(rollOutput);
            data.Output.SendPlayerChatColor.Should().BeTrue();
            data.Output.MessageType.Should().Be(MessageType.DmAction);

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

            data.Output.Text.Should().Be(" rolled 1d1: 1 (1)");
            data.Output.SendPlayerChatColor.Should().BeFalse();
            data.Output.MessageType.Should().Be(MessageType.DieRoll);
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

        [Test]
        public void Should_handle_multiple_dice()
        {
            var data = new MessageData("Tester", "/roll 2d1");
            new RollTextProcessor().Process(data);

            data.Output.Text.Should().Be(" rolled 2d1: 1+1 (2)");
        }

        [Test]
        public void Should_not_allow_more_than_ten_dice_rolls_in_a_single_message()
        {
            var data = new MessageData("Tester", "/roll 11d1");
            new RollTextProcessor().Process(data);

            data.Processed.Should().BeFalse();
        }

        [Test]
        public void Should_allow_adding_additional_numbers_to_sum()
        {
            var data = new MessageData("Tester", "/roll 2d1+1000");
            new RollTextProcessor().Process(data);

            data.Output.Text.Should().Be(" rolled 2d1+1000: 1+1 (1002)");
        }

        [Test]
        public void Should_allow_adding_subtracting_numbers_from_sum()
        {
            var data = new MessageData("Tester", "/roll 2d1-1000");
            new RollTextProcessor().Process(data);

            data.Output.Text.Should().Be(" rolled 2d1-1000: 1+1 (-998)");
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

            data.Output.Text.Should().Be(" does something");
            data.Output.SendPlayerChatColor.Should().BeTrue();
            data.Output.MessageType.Should().Be(MessageType.Action);
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
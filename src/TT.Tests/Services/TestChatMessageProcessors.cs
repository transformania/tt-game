﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using NSubstitute;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Chat;
using TT.Domain.Chat.Queries;
using TT.Domain.Statics;

namespace TT.Tests.Services
{
    [TestFixture]
    public class TestChatMessageProcessors
    {
        private static IPrincipal regularUser = new ClaimsPrincipal(new ClaimsIdentity());
        private static IPrincipal adminUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Role, PvPStatics.Permissions_Admin),
            new Claim(ClaimTypes.Role, PvPStatics.Permissions_Moderator)
        }));

        [TestFixture]
        public class TestRegularTextProcessor
        {
            [Test]
            public void Should_process_regular_message()
            {
                var data = new MessageData(regularUser, "Tester", "Testing");
                new RegularTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo("Testing"));
                Assert.That(data.Output.SendPlayerChatColor, Is.True);
                Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.RegularText));
            }

            [TestCase("")]
            [TestCase(" ")]
            public void Should_not_process_blank_message(string message)
            {
                var data = new MessageData(regularUser, "Tester", message);
                new RegularTextProcessor().Process(data);

                Assert.That(data.Output, Is.Null);
            }

            [TestCase("")]
            [TestCase(" ")]
            public void Should_not_process_if_sender_name_is_blank(string name)
            {
                var data = new MessageData(regularUser, name, "Testing");
                new RegularTextProcessor().Process(data);

                Assert.That(data.Output, Is.Null);
            }
        }

        [TestFixture]
        public class TestReservedTextProcessor
        {
            [TestCase(PvPStatics.Permissions_Admin)]
            [TestCase(PvPStatics.Permissions_Moderator)]
            public void Should_format_reserved_text_used_by_privileged_users(string permission)
            {
                foreach (var data in ChatStatics.ReservedText.Select(reservedText =>
                    new MessageData(adminUser, "Tester", reservedText)))
                {
                    new ReservedTextProcessor().Process(data);
                    Assert.That(data.Output.Text, Is.EqualTo(data.Message));
                    Assert.That(data.Output.SendPlayerChatColor, Is.True);
                    Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.ReservedText));
                }
            }

            [Test]
            public void Should_strip_reserved_text_from_message_by_non_priviledged_users()
            {
                foreach (var data in ChatStatics.ReservedText.Select(reservedText =>
                    new MessageData(regularUser, "Tester", reservedText)))
                {
                    new ReservedTextProcessor().Process(data);
                    Assert.That(data.Output.Text, Is.EqualTo(" "));
                    Assert.That(data.Output.SendPlayerChatColor, Is.True);
                    Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.RegularText));
                }
            }

            [Test]
            public void Should_not_process_message_without_trigger()
            {
                var data = new MessageData(adminUser, "Tester", "[notluxa]");
                new ReservedTextProcessor().Process(data);

                Assert.That(data.Processed, Is.False);
            }
        }

        [TestFixture]
        public class TestDmMessageTextProcessor
        {
            [Test]
            public void Should_format_dm_message_text()
            {
                var data = new MessageData(regularUser, "Tester", "/dm message something happens");
                new DmMessageTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(" something happens"));
                Assert.That(data.Output.SendPlayerChatColor, Is.True);
                Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.DmMessage));
            }

            [Test]
            public void Should_not_process_message_without_trigger()
            {
                var data = new MessageData(regularUser, "Tester", "/not dm message something happens");

                new DmMessageTextProcessor().Process(data);
                Assert.That(data.Processed, Is.False);
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
                var rollOutput = $"DM[{actionType}:{tag}]:  {"Wibble happens"}";

                DomainRegistry.Root = Substitute.For<IRoot>();
                DomainRegistry.Root.Find(Arg.Any<GetRollText>()).Returns(rollOutput);

                var data = new MessageData(regularUser, "Tester", $"/dm {actionType}:{tag}");

                new DmActionTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(rollOutput));
                Assert.That(data.Output.SendPlayerChatColor, Is.True);
                Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.DmAction));

                DomainRegistry.Root.Received()
                    .Find(Arg.Is<GetRollText>(cmd => cmd.ActionType == actionType && cmd.Tag == tag));
            }

            [Test]
            public void Should_not_process_message_without_trigger()
            {
                var data = new MessageData(regularUser, "Tester", "/not dm creature:forest");

                new DmActionTextProcessor().Process(data);
                Assert.That(data.Processed, Is.False);
            }

            [Test]
            public void Should_not_process_message_with_invalid_action_type()
            {
                var data = new MessageData(regularUser, "Tester", "/dm wibble:forest");

                new DmActionTextProcessor().Process(data);
                Assert.That(data.Processed, Is.False);
            }

            [Test]
            public void Should_not_process_message_with_invalid_tag()
            {
                var data = new MessageData(regularUser, "Tester", "/dm creature:wibble");

                new DmActionTextProcessor().Process(data);
                Assert.That(data.Processed, Is.False);
            }
        }

        [TestFixture]
        public class TestRollTextProcessor
        {
            [Test]
            public void Should_format_die_roll()
            {
                var data = new MessageData(regularUser, "Tester", "/roll d1");
                new RollTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(" rolled 1d1: 1 (1)"));
                Assert.That(data.Output.SendPlayerChatColor, Is.False);
                Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.DieRoll));
            }

            [Test]
            public void Should_not_process_message_without_trigger()
            {
                var data = new MessageData(regularUser, "Tester", "/not roll d1");

                new RollTextProcessor().Process(data);
                Assert.That(data.Processed, Is.False);
            }

            [TestCase("/roll d")]
            [TestCase("/roll 20")]
            public void Should_not_process_message_without_die_value(string message)
            {
                var data = new MessageData(regularUser, "Tester", message);

                new RollTextProcessor().Process(data);
                Assert.That(data.Processed, Is.False);
            }

            [Test]
            public void Should_handle_multiple_dice()
            {
                var data = new MessageData(regularUser, "Tester", "/roll 2d1");
                new RollTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(" rolled 2d1: 1+1 (2)"));
            }

            [Test]
            public void Should_not_allow_more_than_ten_dice_rolls_in_a_single_message()
            {
                var data = new MessageData(regularUser, "Tester", "/roll 11d1");
                new RollTextProcessor().Process(data);

                Assert.That(data.Processed, Is.False);
            }

            [Test]
            public void Should_allow_adding_additional_numbers_to_sum()
            {
                var data = new MessageData(regularUser, "Tester", "/roll 2d1+1000");
                new RollTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(" rolled 2d1+1000: 1+1 (1002)"));
            }

            [Test]
            public void Should_allow_adding_subtracting_numbers_from_sum()
            {
                var data = new MessageData(regularUser, "Tester", "/roll 2d1-1000");
                new RollTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(" rolled 2d1-1000: 1+1 (-998)"));
            }
        }

        [TestFixture]
        public class TestPlayerActionTextProcessor
        {
            [Test]
            public void Should_format_action_text()
            {
                var data = new MessageData(regularUser, "Tester", "/me does something");

                new PlayerActionTextProcessor().Process(data);

                Assert.That(data.Output.Text, Is.EqualTo(" does something"));
                Assert.That(data.Output.SendPlayerChatColor, Is.True);
                Assert.That(data.Output.MessageType, Is.EqualTo(MessageType.Action));
            }

            [Test]
            public void Should_not_process_message_without_trigger()
            {
                var data = new MessageData(regularUser, "Tester", "/not me does something");
                new PlayerActionTextProcessor().Process(data);

                Assert.That(data.Processed, Is.False);
            }
        }
    }
}
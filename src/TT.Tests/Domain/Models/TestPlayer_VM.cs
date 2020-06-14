using System;
using NSubstitute;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Models;
using TT.Tests.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TT.Domain.Players.Commands;

namespace TT.Tests.Domain.Models
{
    using TagEnum = PlayerDescriptorStatics.TagBehavior;

    [TestFixture]
    public class TestPlayer_VM
    {
        private static IDictionary<string, PlayerDescriptorDTO> staff;
        private Player_VM player;

        [SetUp]
        public void SetUp()
        {
            DomainRegistry.Root = Substitute.For<IRoot>();

            player = TestData.CreateRegularPlayer<MockedPlayer_VMWrapper>(membershipId: Guid.NewGuid().ToString());
            staff = new Dictionary<string, PlayerDescriptorDTO>();
        }

        [Test]
        public void Should_get_blank_descriptor_for_negative_membership_id()
        {
            var player = new Player_VM { MembershipId = "-1" };

            var descriptor = player.GetDescriptor();

            Assert.That(descriptor.Item1, Is.Empty);
            Assert.That(descriptor.Item2, Is.Empty);
        }

        [Test]
        public void Should_get_descriptor_with_full_player_name_if_not_staff()
        {
            var descriptor = player.GetDescriptor();
            Assert.That(descriptor.Item1, Is.EqualTo("Test 'Wibble' User"));
            Assert.That(descriptor.Item2, Is.Empty);
        }

        [Test]
        public void Should_get_descriptor_with_correct_picture_URL_if_staff()
        {
            var url = "TestURL";

            staff.Add(player.MembershipId, new PlayerDescriptorDTO { PictureURL = url });

            Assert.That(player.GetDescriptor().Item2, Is.EqualTo(url));
        }

        [Test]
        public void Should_get_descriptor_with_empty_picture_URL_if_staff_and_PictureURL_is_not_set()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { });

            Assert.That(player.GetDescriptor().Item2, Is.Empty);
        }

        [Test]
        public void Should_get_descriptor_with_full_name_if_staff_and_TagBehaviorEnum_is_set_to_the_default_Append()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { });

            Assert.That(player.GetDescriptor().Item1,
                Is.EqualTo($"{player.FirstName} '{player.Nickname}' {player.LastName}"));
        }

        [Test]
        public void Should_get_descriptor_with_name_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceFullName()
        {
            var replaceName = "TestName";

            staff.Add(player.MembershipId, new PlayerDescriptorDTO { Name = replaceName, TagBehaviorEnum = TagEnum.ReplaceFullName });

            Assert.That(player.GetDescriptor().Item1, Is.EqualTo($"{replaceName}"));
        }

        [Test]
        public void Should_get_descriptor_with_last_name_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceLastName()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { TagBehaviorEnum = TagEnum.ReplaceLastName });

            Assert.That(player.GetDescriptor().Item1, Is.EqualTo($"{player.FirstName} '{player.Nickname}'"));
        }

        [Test]
        public void Should_get_descriptor_with_last_name_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceLastName_should_not_display_nick_if_not_set()
        {
            var player = TestData.CreateRegularPlayer<MockedPlayer_VMWrapper>(membershipId: Guid.NewGuid().ToString(), donator: false);

            staff.Add(player.MembershipId, new PlayerDescriptorDTO { TagBehaviorEnum = TagEnum.ReplaceLastName });

            Assert.That(player.GetDescriptor().Item1, Is.EqualTo(player.FirstName));
        }

        [Test]
        public void Should_get_descriptor_with_last_name_and_nick_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceLastNameAndNick()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { TagBehaviorEnum = TagEnum.ReplaceLastNameAndNick });

            Assert.That(player.GetDescriptor().Item1, Is.EqualTo(player.FirstName));
        }

        [Test]
        public void Should_update_player_online_activity_timestamp_if_overdue()
        {
            var player = TestData.CreateRegularPlayer<Player_VM>();
            player.OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-3);

            player.UpdateOnlineActivityTimestamp();

            DomainRegistry.Root.Received().Execute(Arg.Is<UpdateOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }

        [Test]
        public void Should_not_update_player_online_activity_timestamp_if_not_overdue()
        {
            var player = TestData.CreateRegularPlayer<Player_VM>();
            player.OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-1);

            player.UpdateOnlineActivityTimestamp();

            DomainRegistry.Root.DidNotReceive().Execute(Arg.Is<UpdateOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }

        private class MockedPlayer_VMWrapper : Player_VM
        {
            protected override IReadOnlyDictionary<string, PlayerDescriptorDTO> staffDictionary
            {
                get
                {
                    return new ReadOnlyDictionary<string, PlayerDescriptorDTO>(staff);
                }
            }
        }
    }
}
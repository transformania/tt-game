using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Commands.Player;
using TT.Domain.Models;
using TT.Tests.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TT.Tests.Domain.Models
{
    using TagEnum = PlayerDescriptorStatics.TagBehavior;
    using RoleEnum = PlayerDescriptorStatics.Role;

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

            descriptor.Item1.Should().BeEmpty();
            descriptor.Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_get_descriptor_with_full_player_name_if_not_staff()
        {
            var descriptor = player.GetDescriptor();
            descriptor.Item1.Should().Be("Test 'Wibble' User");
            descriptor.Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_get_descriptor_with_correct_picture_URL_if_staff()
        {
            var url = "TestURL";

            staff.Add(player.MembershipId, new PlayerDescriptorDTO { PictureURL = url });

            player.GetDescriptor().Item2.Should().Be(url);
        }

        [Test]
        public void Should_get_descriptor_with_empty_picture_URL_if_staff_and_PictureURL_is_not_set()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { });

            player.GetDescriptor().Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_get_descriptor_with_full_name_if_staff_and_TagBehaviorEnum_is_set_to_the_default_Append()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { });

            player.GetDescriptor().Item1.Should().Be($"{player.FirstName} '{player.Nickname}' {player.LastName} (Admin)");
        }

        [Test]
        public void Should_get_descriptor_with_name_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceFullName()
        {
            var replaceName = "TestName";

            staff.Add(player.MembershipId, new PlayerDescriptorDTO { Name = replaceName, TagBehaviorEnum = TagEnum.ReplaceFullName });

            player.GetDescriptor().Item1.Should().Be($"{replaceName} (Admin)");
        }

        [Test]
        public void Should_get_descriptor_with_last_name_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceLastName()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { TagBehaviorEnum = TagEnum.ReplaceLastName });

            player.GetDescriptor().Item1.Should().Be($"{player.FirstName} '{player.Nickname}' (Admin)");
        }

        [Test]
        public void Should_get_descriptor_with_last_name_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceLastName_should_not_display_nick_if_not_set()
        {
            var player = TestData.CreateRegularPlayer<MockedPlayer_VMWrapper>(membershipId: Guid.NewGuid().ToString(), donator: false);

            staff.Add(player.MembershipId, new PlayerDescriptorDTO { TagBehaviorEnum = TagEnum.ReplaceLastName });

            player.GetDescriptor().Item1.Should().Be($"{player.FirstName} (Admin)");
        }

        [Test]
        public void Should_get_descriptor_with_last_name_and_nick_replaced_if_staff_and_TagBehaviorEnum_is_set_to_ReplaceLastNameAndNick()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { TagBehaviorEnum = TagEnum.ReplaceLastNameAndNick });

            player.GetDescriptor().Item1.Should().Be($"{player.FirstName} (Admin)");
        }

        [Test]
        public void Should_get_descriptor_with_full_name_and_dev_if_staff_and_TagBehaviorEnum_is_set_to_the_default_Append()
        {
            staff.Add(player.MembershipId, new PlayerDescriptorDTO { RoleEnum = RoleEnum.Developer });

            player.GetDescriptor().Item1.Should().Be($"{player.FirstName} '{player.Nickname}' {player.LastName} (Dev)");
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
using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using tfgame.dbModels;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Commands.Player;
using tfgame.dbModels.Models;
using tfgame.Services;
using tfgame.Statics;
using tfgame.Tests.Services;

namespace tfgame.Tests.DBModels.Models
{
    [TestFixture]
    public class TestPlayer_VM
    {
        [SetUp]
        public void SetUp()
        {
            DomainRegistry.Root = Substitute.For<IRoot>();
        }

        [Test]
        public void Should_get_correct_descriptor_for_staff()
        {
            foreach (var staffMember in ChatStatics.Staff)
            {
                var player = new Player_VM { MembershipId = staffMember.Key };
                var descriptor = player.GetDescriptor();

                descriptor.Item1.Should().Be(staffMember.Value.Item1);
                descriptor.Item2.Should().Be(staffMember.Value.Item2);
            }
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
            var player = TestData.CreateRegularPlayer();

            var descriptor = player.GetDescriptor();
            descriptor.Item1.Should().Be("Test 'Wibble' User");
            descriptor.Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_update_player_online_activity_timestamp_if_overdue()
        {
            var player = TestData.CreateRegularPlayer();
            player.OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-3);

            player.UpdateOnlineActivityTimestamp();

            DomainRegistry.Root.Received().Execute(Arg.Is<UpdateOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }

        [Test]
        public void Should_not_update_player_online_activity_timestamp_if_not_overdue()
        {
            var player = TestData.CreateRegularPlayer();
            player.OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-1);

            player.UpdateOnlineActivityTimestamp();

            DomainRegistry.Root.DidNotReceive().Execute(Arg.Is<UpdateOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }
    }
}
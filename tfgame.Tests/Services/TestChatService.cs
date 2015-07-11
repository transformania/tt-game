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

namespace tfgame.Tests.Services
{
    [TestFixture]
    public class TestChatService
    {
        [SetUp]
        public void SetUp()
        {
            DomainRegistry.Root = Substitute.For<IRoot>();
        }
        
        [Test]
        public void Should_update_player_online_activity_timestamp_if_overdue()
        {
            var player = new Player_VM 
            { 
                Id = 12345, 
                OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-3)
            };

            var chatService = new ChatService();
            chatService.MarkOnlineActivityTimestamp(player);

            DomainRegistry.Root.Received().Execute(Arg.Is<MarkOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }

        [Test]
        public void Should_not_update_player_online_activity_timestamp_if_not_overdue()
        {
            var player = new Player_VM
            {
                Id = 12345,
                OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-1)
            };

            var chatService = new ChatService();
            chatService.MarkOnlineActivityTimestamp(player);

            DomainRegistry.Root.DidNotReceive().Execute(Arg.Is<MarkOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }

        [Test]
        public void Should_get_correct_descriptor_for_staff()
        {
            var chatService = new ChatService();

            foreach (var staffMember in ChatStatics.Staff)
            {
                var player = new Player_VM { MembershipId = staffMember.Key };
                var descriptor = chatService.GetPlayerDescriptorFor(player);

                descriptor.Item1.Should().Be(staffMember.Value.Item1);
                descriptor.Item2.Should().Be(staffMember.Value.Item2);
            }
        }

        [Test]
        public void Should_get_blank_descriptor_for_negative_membership_id()
        {
            var chatService = new ChatService();
            var player = new Player_VM { MembershipId = -1 };

            var descriptor = chatService.GetPlayerDescriptorFor(player);

            descriptor.Item1.Should().BeEmpty();
            descriptor.Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_get_descriptor_with_full_player_name_if_not_staff()
        {
            var chatService = new ChatService();
            var player = new Player_VM { MembershipId = 100, FirstName = "Test", LastName = "User", Nickname = "Wibble", DonatorLevel = 2};

            var descriptor = chatService.GetPlayerDescriptorFor(player);
            descriptor.Item1.Should().Be("Test 'Wibble' User");
            descriptor.Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_add_user_to_chat_user_list_on_connection()
        {
            var chatService = new ChatService();
            var player = new Player_VM { MembershipId = 100, FirstName = "Test", LastName = "User", Nickname = "Wibble", DonatorLevel = 2};
            
            chatService.OnUserConnected(player, Guid.NewGuid().ToString());

            ChatService.ChatPersistance.Should().ContainKey(player.MembershipId);
            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be(player.GetFullName());
            ChatService.ChatPersistance[player.MembershipId].Connections.Should().HaveCount(1);
        }

        [Test]
        public void Should_add_staff_to_chat_user_list_with_correct_name_on_connection()
        {
            var chatService = new ChatService();
            var player = new Player_VM { MembershipId = 69, FirstName = "Test", LastName = "User"};

            chatService.OnUserConnected(player, Guid.NewGuid().ToString());

            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be(ChatStatics.Staff[player.MembershipId].Item1);
        }

        [Test]
        public void Should_keep_track_of_multiple_connections_for_same_user()
        {
            var chatService = new ChatService();
            var player1 = new Player_VM { MembershipId = 100, FirstName = "Test1", LastName = "User1", Nickname = "Wibble", DonatorLevel = 2 };
            var player2 = new Player_VM { MembershipId = 200, FirstName = "Test2", LastName = "User2" };

            chatService.OnUserConnected(player1, Guid.NewGuid().ToString());
            chatService.OnUserConnected(player1, Guid.NewGuid().ToString());

            chatService.OnUserConnected(player2, Guid.NewGuid().ToString());
            chatService.OnUserConnected(player2, Guid.NewGuid().ToString());
            chatService.OnUserConnected(player2, Guid.NewGuid().ToString());

            ChatService.ChatPersistance.Should().ContainKey(player1.MembershipId);
            ChatService.ChatPersistance.Should().ContainKey(player2.MembershipId);

            ChatService.ChatPersistance[player1.MembershipId].Connections.Should().HaveCount(2);
            ChatService.ChatPersistance[player2.MembershipId].Connections.Should().HaveCount(3);
        }
    }
}
using System;
using System.Linq;
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
            ChatService.ChatPersistance.Clear();
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
            var player = CreateRegularPlayer();

            var descriptor = chatService.GetPlayerDescriptorFor(player);
            descriptor.Item1.Should().Be("Test 'Wibble' User");
            descriptor.Item2.Should().BeEmpty();
        }

        [Test]
        public void Should_add_user_to_chat_user_list_on_connection()
        {
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            
            chatService.OnUserConnected(player, Guid.NewGuid().ToString());

            ChatService.ChatPersistance.Should().ContainKey(player.MembershipId);
            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be(player.GetFullName());
            ChatService.ChatPersistance[player.MembershipId].Connections.Should().HaveCount(1);
            ChatService.ChatPersistance[player.MembershipId].IsDonator.Should().BeTrue();
        }

        [Test]
        public void Should_add_staff_to_chat_user_list_with_correct_name_on_connection()
        {
            var chatService = new ChatService();
            var player = CreateStaffPlayer();

            chatService.OnUserConnected(player, Guid.NewGuid().ToString());

            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be(ChatStatics.Staff[player.MembershipId].Item1);
        }

        [Test]
        public void Should_keep_track_of_multiple_connections_for_same_user()
        {
            var chatService = new ChatService();
            var player1 = CreateRegularPlayer(100, "Test1", "User1");
            var player2 = CreateRegularPlayer(200, "Test2", "User2", false);

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

        [Test]
        public void Should_remove_connection_on_disconnect()
        {
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();

            chatService.OnUserConnected(player, connectionId1);
            chatService.OnUserConnected(player, connectionId2);

            ChatService.ChatPersistance[player.MembershipId].Connections.Should().HaveCount(2);

            chatService.OnUserDisconnected(player, connectionId1);

            ChatService.ChatPersistance[player.MembershipId].Connections.Should().HaveCount(1);
            ChatService.ChatPersistance[player.MembershipId].Connections.Should().Contain(x => x.ConnectionId == connectionId2);
        }

        [Test]
        public void Should_remove_player_from_persistance_when_they_have_no_connections()
        {
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            chatService.OnUserConnected(player, connectionId);

            ChatService.ChatPersistance[player.MembershipId].Connections.Should().HaveCount(1);

            chatService.OnUserDisconnected(player, connectionId);

            ChatService.ChatPersistance.ContainsKey(player.MembershipId).Should().BeFalse();
        }

        [Test]
        public void Should_track_when_user_joins_a_room()
        {
            const string room = "global";
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();
            
            chatService.OnUserConnected(player, connectionId);
            chatService.OnUserJoinRoom(player, connectionId, room);

            var connection = ChatService.ChatPersistance[player.MembershipId].Connections.Single();
            connection.ConnectionId.Should().Be(connectionId);
            connection.Room.Should().Be(room);
            connection.LastActivity.ToShortTimeString().Should().Be(DateTime.UtcNow.ToShortTimeString());
        }

        [Test]
        public void Should_track_when_user_joins_multiple_rooms()
        {
            const string room1 = "global";
            const string room2 = "test";
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();

            chatService.OnUserConnected(player, connectionId1);
            chatService.OnUserJoinRoom(player, connectionId1, room1);

            chatService.OnUserConnected(player, connectionId2);
            chatService.OnUserJoinRoom(player, connectionId2, room2);

            ChatService.ChatPersistance[player.MembershipId].Connections.Should().Contain(x => x.ConnectionId == connectionId1 && x.Room == room1);
            ChatService.ChatPersistance[player.MembershipId].Connections.Should().Contain(x => x.ConnectionId == connectionId2 && x.Room == room2);
        }

        [Test]
        public void Should_track_last_time_user_sent_a_message()
        {
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            chatService.OnUserConnected(player, connectionId);
            chatService.OnUserSentMessage(player, connectionId);

            var connection = ChatService.ChatPersistance[player.MembershipId].Connections.Single(x => x.ConnectionId == connectionId);
            connection.LastActivity.ToShortTimeString().Should().Be(DateTime.UtcNow.ToShortTimeString());
        }

        [Test]
        public void Should_update_name_of_user_after_reroll_on_joining_room()
        {
            const string room = "global";
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            chatService.OnUserConnected(player, connectionId);
            chatService.OnUserJoinRoom(player, connectionId, room);

            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be("Test 'Wibble' User");

            player.FirstName = "Re";
            player.LastName = "Roll";

            chatService.MonitorEvents();
            chatService.OnUserJoinRoom(player, connectionId, room);

            chatService.ShouldRaise("NameChanged");
            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be("Re 'Wibble' Roll");
        }

        [Test]
        public void Should_update_name_of_user_after_reroll_on_message_sent()
        {
            var chatService = new ChatService();
            var player = CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            chatService.OnUserConnected(player, connectionId);

            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be("Test 'Wibble' User");

            player.FirstName = "Re";
            player.LastName = "Roll";
            
            chatService.MonitorEvents();
            chatService.OnUserSentMessage(player, connectionId);

            chatService.ShouldRaise("NameChanged");
            ChatService.ChatPersistance[player.MembershipId].Name.Should().Be("Re 'Wibble' Roll");
        }
        
        private Player_VM CreateRegularPlayer(int membershipId = 100, string firstName = "Test", string lastName = "User", bool donator = true)
        {
            return new Player_VM
            {
                MembershipId = membershipId, 
                FirstName = firstName, 
                LastName = lastName, 
                Nickname = donator ? "Wibble" : null, 
                DonatorLevel = donator ? 2 : 0
            };
        }

        private Player_VM CreateStaffPlayer()
        {
            return new Player_VM { MembershipId = 69, FirstName = "Test", LastName = "User" };
        }
    }
}
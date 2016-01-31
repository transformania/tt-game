using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Models;
using tfgame.Services;
using TT.Domain.Statics;

namespace TT.Tests.Services
{
    [TestFixture]
    public class TestChatPersistenceService
    {
        [SetUp]
        public void SetUp()
        {
            ChatPersistenceServiceWrapper.Reset();
        }

        [Test]
        public void Should_register_connection()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();

            persistanceService.TrackConnection(player, Guid.NewGuid().ToString());

            persistanceService.InternalPersistence.Should().ContainKey(player.MembershipId);
            persistanceService.InternalPersistence[player.MembershipId].Name.Should().Be(player.GetFullName());
            persistanceService.InternalPersistence[player.MembershipId].Connections.Should().HaveCount(1);
            persistanceService.InternalPersistence[player.MembershipId].IsDonator.Should().BeTrue();
        }

        [Test]
        public void Should_register_staff_connection_with_correct_name()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateStaffPlayer();

            persistanceService.TrackConnection(player, Guid.NewGuid().ToString());

            persistanceService.InternalPersistence[player.MembershipId].Name.Should().Be(ChatStatics.Staff[player.MembershipId].Item1);
        }

        [Test]
        public void Should_keep_track_of_multiple_connections_for_same_user()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player1 = TestData.CreateRegularPlayer("100", "Test1", "User1");
            var player2 = TestData.CreateRegularPlayer("200", "Test2", "User2", false);

            persistanceService.TrackConnection(player1, Guid.NewGuid().ToString());
            persistanceService.TrackConnection(player1, Guid.NewGuid().ToString());

            persistanceService.TrackConnection(player2, Guid.NewGuid().ToString());
            persistanceService.TrackConnection(player2, Guid.NewGuid().ToString());
            persistanceService.TrackConnection(player2, Guid.NewGuid().ToString());

            persistanceService.InternalPersistence.Should().ContainKey(player1.MembershipId);
            persistanceService.InternalPersistence.Should().ContainKey(player2.MembershipId);

            persistanceService.InternalPersistence.Should().HaveCount(2);

            persistanceService.InternalPersistence[player1.MembershipId].Connections.Should().HaveCount(2);
            persistanceService.InternalPersistence[player2.MembershipId].Connections.Should().HaveCount(3);
        }

        [Test]
        public void Should_remove_connection_on_disconnect()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();

            persistanceService.TrackConnection(player, connectionId1);
            persistanceService.TrackConnection(player, connectionId2);

            persistanceService.InternalPersistence[player.MembershipId].Connections.Should().HaveCount(2);

            persistanceService.TrackDisconnect(player.MembershipId, connectionId1);

            persistanceService.InternalPersistence[player.MembershipId].Connections.Should().HaveCount(1);
            persistanceService.InternalPersistence[player.MembershipId].Connections.Should().Contain(x => x.ConnectionId == connectionId2);
        }

        [Test]
        public void Should_remove_player_when_they_have_no_more_active_connections()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistanceService.TrackConnection(player, connectionId);

            persistanceService.InternalPersistence[player.MembershipId].Connections.Should().HaveCount(1);

            persistanceService.TrackDisconnect(player.MembershipId, connectionId);

            persistanceService.InternalPersistence.ContainsKey(player.MembershipId).Should().BeFalse();
        }

        [Test]
        public void Should_track_when_user_joins_a_room()
        {
            const string room = "global";
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistanceService.TrackConnection(player, connectionId);
            persistanceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            var connection = persistanceService.InternalPersistence[player.MembershipId].Connections.Single();
            connection.ConnectionId.Should().Be(connectionId);
            connection.Room.Should().Be(room);
            connection.LastActivity.ToShortTimeString().Should().Be(DateTime.UtcNow.ToShortTimeString());
        }

        [Test]
        public void Should_track_when_user_joins_multiple_rooms()
        {
            const string room1 = "global";
            const string room2 = "test";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId1);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId1, room1);

            persistenceService.TrackConnection(player, connectionId2);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId2, room2);

            persistenceService.InternalPersistence[player.MembershipId].Connections.Should().Contain(x => x.ConnectionId == connectionId1 && x.Room == room1);
            persistenceService.InternalPersistence[player.MembershipId].Connections.Should().Contain(x => x.ConnectionId == connectionId2 && x.Room == room2);
        }

        [Test]
        public void Should_track_last_time_user_sent_a_message()
        {
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackMessageSend(player.MembershipId, connectionId);

            var connection = persistenceService.InternalPersistence[player.MembershipId].Connections.Single(x => x.ConnectionId == connectionId);
            connection.LastActivity.ToShortTimeString().Should().Be(DateTime.UtcNow.ToShortTimeString());
        }

        [Test]
        public void Should_be_able_to_know_if_a_player_has_changed_their_name()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            persistenceService.HasNameChanged(player.MembershipId, "New Player Name").Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_know_if_a_player_has_not_changed_their_name()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            persistenceService.HasNameChanged(player.MembershipId, player.GetDescriptor().Item1).Should().BeFalse();
        }

        [Test]
        public void Should_get_list_of_all_rooms_a_player_is_in()
        {
            const string room1 = "room1";
            const string room2 = "room2";
            const string room3 = "room3";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player1 = TestData.CreateRegularPlayer();
            var player2 = TestData.CreateRegularPlayer("200");
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();
            var connectionId3 = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player1, connectionId1);
            persistenceService.TrackConnection(player1, connectionId2);
            persistenceService.TrackConnection(player2, connectionId3);

            persistenceService.TrackRoomJoin(player1.MembershipId, connectionId1, room1);
            persistenceService.TrackRoomJoin(player1.MembershipId, connectionId2, room2);
            persistenceService.TrackRoomJoin(player2.MembershipId, connectionId3, room3);

            var rooms = persistenceService.GetRoomsPlayerIsIn(player1.MembershipId);

            rooms.Should().Contain(new[] {room1, room2}).And.NotContain(room3);
        }

        [Test]
        public void Should_get_empty_list_of_rooms_if_player_is_in_any()
        {
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);

            var rooms = persistenceService.GetRoomsPlayerIsIn(player.MembershipId);

            rooms.Should().BeEmpty();
        }

        [Test]
        public void Should_get_room_for_player_and_connection_id()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            var result = persistenceService.GetRoom(player.MembershipId, connectionId);
            result.Should().Be(room);
        }

        [Test]
        public void Should_get_empty_room_name_if_player_isnt_found()
        {
            var persistenceService = new ChatPersistenceServiceWrapper();
            var playerId = "doesn't exist";
            var connectionId = Guid.NewGuid().ToString();

            var result = persistenceService.GetRoom(playerId, connectionId);
            result.Should().Be(string.Empty);
        }

        [Test]
        public void Should_get_empty_room_name_for_player_if_connection_id_isnt_found()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, Guid.NewGuid().ToString(), room);

            var result = persistenceService.GetRoom(player.MembershipId, connectionId);
            result.Should().Be(string.Empty);
        }

        [Test]
        public void Should_get_all_users_in_room()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player1 = TestData.CreateRegularPlayer("101");
            var player2 = TestData.CreateRegularPlayer("102");
            var player3 = TestData.CreateRegularPlayer("103");
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();
            var connectionId3 = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player1, connectionId1);
            persistenceService.TrackConnection(player2, connectionId2);
            persistenceService.TrackConnection(player3, connectionId3);

            persistenceService.TrackRoomJoin(player1.MembershipId, connectionId1, room);
            persistenceService.TrackRoomJoin(player2.MembershipId, connectionId2, "another_room");
            persistenceService.TrackRoomJoin(player3.MembershipId, connectionId3, room);

            var users = persistenceService.GetUsersInRoom(room).ToList();
            users.Should().Contain(x => x.MembershipId == player1.MembershipId);
            users.Should().Contain(x => x.MembershipId == player3.MembershipId);
            users.Should().NotContain(x => x.MembershipId == player2.MembershipId);
        }

        [Test]
        public void Should_update_player_name()
        {
            const string room = "global";
            const string newName = "New Player Name";

            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            persistenceService.TrackPlayerNameChange(player.MembershipId, newName);

            persistenceService.GetUsersInRoom(room).Should().Contain(x => x.Name == newName);
        }

        public class ChatPersistenceServiceWrapper : ChatPersistenceService
        {
            public IDictionary<string, ChatUser> InternalPersistence { get { return Persistence; } }

            public static void Reset()
            {
                Persistence.Clear();
            }
        }
    }
}
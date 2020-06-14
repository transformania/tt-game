using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Models;
using TT.Web.Services;

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
            var player = TestData.CreateRegularPlayer<Player_VM>();

            persistanceService.TrackConnection(player, Guid.NewGuid().ToString());

            Assert.That(persistanceService.InternalPersistence, Contains.Key(player.MembershipId));
            Assert.That(persistanceService.InternalPersistence[player.MembershipId].Name,
                Is.EqualTo(player.GetFullName()));
            Assert.That(persistanceService.InternalPersistence[player.MembershipId].Connections, Has.Exactly(1).Items);
            Assert.That(persistanceService.InternalPersistence[player.MembershipId].IsDonator, Is.True);
        }

        [Test]
        public void Should_keep_track_of_multiple_connections_for_same_user()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player1 = TestData.CreateRegularPlayer<Player_VM>("100", "Test1", "User1");
            var player2 = TestData.CreateRegularPlayer<Player_VM>("200", "Test2", "User2", false);

            persistanceService.TrackConnection(player1, Guid.NewGuid().ToString());
            persistanceService.TrackConnection(player1, Guid.NewGuid().ToString());

            persistanceService.TrackConnection(player2, Guid.NewGuid().ToString());
            persistanceService.TrackConnection(player2, Guid.NewGuid().ToString());
            persistanceService.TrackConnection(player2, Guid.NewGuid().ToString());

            Assert.That(persistanceService.InternalPersistence, Contains.Key(player1.MembershipId));
            Assert.That(persistanceService.InternalPersistence, Contains.Key(player2.MembershipId));

            Assert.That(persistanceService.InternalPersistence, Has.Exactly(2).Items);

            Assert.That(persistanceService.InternalPersistence[player1.MembershipId].Connections, Has.Exactly(2).Items);
            Assert.That(persistanceService.InternalPersistence[player2.MembershipId].Connections, Has.Exactly(3).Items);
        }

        [Test]
        public void Should_remove_connection_on_disconnect()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();

            persistanceService.TrackConnection(player, connectionId1);
            persistanceService.TrackConnection(player, connectionId2);

            Assert.That(persistanceService.InternalPersistence[player.MembershipId].Connections, Has.Exactly(2).Items);

            persistanceService.TrackDisconnect(player.MembershipId, connectionId1);

            Assert.That(persistanceService.InternalPersistence[player.MembershipId].Connections, Has.Exactly(1).Items);
            Assert.That(persistanceService.InternalPersistence[player.MembershipId].Connections.First().ConnectionId, Is.EqualTo(connectionId2));
        }

        [Test]
        public void Should_remove_player_when_they_have_no_more_active_connections()
        {
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistanceService.TrackConnection(player, connectionId);

            Assert.That(persistanceService.InternalPersistence[player.MembershipId].Connections, Has.Exactly(1).Items);

            persistanceService.TrackDisconnect(player.MembershipId, connectionId);

            Assert.That(persistanceService.InternalPersistence, Does.Not.ContainKey(player.MembershipId));
        }

        [Test]
        public void Should_track_when_user_joins_a_room()
        {
            const string room = "global";
            var persistanceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistanceService.TrackConnection(player, connectionId);
            persistanceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            var connection = persistanceService.InternalPersistence[player.MembershipId].Connections.Single();
            Assert.That(connection.ConnectionId, Is.EqualTo(connectionId));
            Assert.That(connection.Room, Is.EqualTo(room));
            Assert.That(connection.LastActivity.ToShortTimeString(), Is.EqualTo(DateTime.UtcNow.ToShortTimeString()));
        }

        [Test]
        public void Should_track_when_user_joins_multiple_rooms()
        {
            const string room1 = "global";
            const string room2 = "test";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId1);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId1, room1);

            persistenceService.TrackConnection(player, connectionId2);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId2, room2);

            Assert.That(persistenceService.InternalPersistence[player.MembershipId].Connections, Has.Exactly(2).Items);
            Assert.That(
                persistenceService.InternalPersistence[player.MembershipId].Connections
                    .Where(x => x.ConnectionId == connectionId1 && x.Room == room1), Has.Exactly(1).Items);
            Assert.That(
                persistenceService.InternalPersistence[player.MembershipId].Connections
                    .Where(x => x.ConnectionId == connectionId2 && x.Room == room2), Has.Exactly(1).Items);
        }

        [Test]
        public void Should_track_last_time_user_sent_a_message()
        {
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackMessageSend(player.MembershipId, connectionId);

            var connection = persistenceService.InternalPersistence[player.MembershipId].Connections.Single(x => x.ConnectionId == connectionId);
            Assert.That(connection.LastActivity.ToShortTimeString(), Is.EqualTo(DateTime.UtcNow.ToShortTimeString()));
        }

        [Test]
        public void Should_be_able_to_know_if_a_player_has_changed_their_name()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            Assert.That(persistenceService.HasNameChanged(player.MembershipId, "New Player Name"), Is.True);
        }

        [Test]
        public void Should_be_able_to_know_if_a_player_has_not_changed_their_name()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            Assert.That(persistenceService.HasNameChanged(player.MembershipId, player.GetDescriptor().Item1), Is.False);
        }

        [Test]
        public void Should_get_list_of_all_rooms_a_player_is_in()
        {
            const string room1 = "room1";
            const string room2 = "room2";
            const string room3 = "room3";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player1 = TestData.CreateRegularPlayer<Player_VM>();
            var player2 = TestData.CreateRegularPlayer<Player_VM>("200");
            var connectionId1 = Guid.NewGuid().ToString();
            var connectionId2 = Guid.NewGuid().ToString();
            var connectionId3 = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player1, connectionId1);
            persistenceService.TrackConnection(player1, connectionId2);
            persistenceService.TrackConnection(player2, connectionId3);

            persistenceService.TrackRoomJoin(player1.MembershipId, connectionId1, room1);
            persistenceService.TrackRoomJoin(player1.MembershipId, connectionId2, room2);
            persistenceService.TrackRoomJoin(player2.MembershipId, connectionId3, room3);

            Assert.That(persistenceService.GetRoomsPlayerIsIn(player1.MembershipId),
                Is.SupersetOf(new[] {room1, room2}).And.No.Member(room3));
        }

        [Test]
        public void Should_get_empty_list_of_rooms_if_player_is_in_any()
        {
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);

            Assert.That(persistenceService.GetRoomsPlayerIsIn(player.MembershipId), Is.Empty);
        }

        [Test]
        public void Should_get_room_for_player_and_connection_id()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            Assert.That(persistenceService.GetRoom(player.MembershipId, connectionId), Is.EqualTo(room));
        }

        [Test]
        public void Should_get_empty_room_name_if_player_isnt_found()
        {
            var persistenceService = new ChatPersistenceServiceWrapper();
            var playerId = "doesn't exist";
            var connectionId = Guid.NewGuid().ToString();

            Assert.That(persistenceService.GetRoom(playerId, connectionId), Is.Empty);
        }

        [Test]
        public void Should_get_empty_room_name_for_player_if_connection_id_isnt_found()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, Guid.NewGuid().ToString(), room);

            Assert.That(persistenceService.GetRoom(player.MembershipId, connectionId), Is.Empty);
        }

        [Test]
        public void Should_get_all_users_in_room()
        {
            const string room = "global";
            var persistenceService = new ChatPersistenceServiceWrapper();
            var player1 = TestData.CreateRegularPlayer<Player_VM>("101");
            var player2 = TestData.CreateRegularPlayer<Player_VM>("102");
            var player3 = TestData.CreateRegularPlayer<Player_VM>("103");
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
            Assert.That(users.Where(x => x.MembershipId == player1.MembershipId), Has.Exactly(1).Items);
            Assert.That(users.Where(x => x.MembershipId == player3.MembershipId), Has.Exactly(1).Items);
            Assert.That(users.Where(x => x.MembershipId == player2.MembershipId), Is.Empty);
        }

        [Test]
        public void Should_update_player_name()
        {
            const string room = "global";
            const string newName = "New Player Name";

            var persistenceService = new ChatPersistenceServiceWrapper();
            var player = TestData.CreateRegularPlayer<Player_VM>();
            var connectionId = Guid.NewGuid().ToString();

            persistenceService.TrackConnection(player, connectionId);
            persistenceService.TrackRoomJoin(player.MembershipId, connectionId, room);

            persistenceService.TrackPlayerNameChange(player.MembershipId, newName);

            Assert.That(persistenceService.GetUsersInRoom(room).Where(x => x.Name == newName), Has.Exactly(1).Items);
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
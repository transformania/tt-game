﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using tfgame.CustomHtmlHelpers;
using tfgame.dbModels.Models;
using tfgame.Extensions;
using tfgame.Procedures;
using tfgame.Services;
using tfgame.Statics;

namespace tfgame.Chat
{
    public class ChatHub : Hub
    {
        private readonly IChatPersistanceService _chatPersistenceService;
        
        public ChatHub()
        {
            _chatPersistenceService = new ChatPersistenceService();
        }

        public override Task OnConnected()
        {
            var me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(Context.User.Identity.GetUserId()).Player;
            _chatPersistenceService.TrackConnection(me, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            var connectionId = Context.ConnectionId;
            var me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(Context.User.Identity.GetUserId()).Player;
            var room = _chatPersistenceService.GetRoom(me.MembershipId, connectionId);

            _chatPersistenceService.TrackDisconnect(me.MembershipId, connectionId);
            
            if (string.IsNullOrWhiteSpace(room) || _chatPersistenceService.GetRoomsPlayerIsIn(me.MembershipId).Contains(room))
                return base.OnDisconnected();

            SendNoticeToRoom(room, me, " has left the room.");
            UpdateUserList(room, false);

            return base.OnDisconnected();
        }

        public void Send(string message)
        {
            string room = Clients.Caller.toRoom;
            var me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(Context.User.Identity.GetUserId());
            
            me.Player.UpdateOnlineActivityTimestamp();
            
            // Assert player is not banned
            if (me.Player.IsBannedFromGlobalChat && room == "global")
                return;

            // Get player picture and name
            var pic = CharactersHere.GetImageURL(me, true).ToString();
            var descriptor = me.Player.GetDescriptor();

            var name = descriptor.Item1;
            pic = string.IsNullOrWhiteSpace(descriptor.Item2) ? pic : descriptor.Item2;

            // Performs message processing to correctly format any special text
            var output = ChatMessageProcessor.ProcessMessage(new MessageData(name, message));

            if (!string.IsNullOrWhiteSpace(output.Text))
            {
                var colorOut = output.SendPlayerChatColor ? me.Player.ChatColor : "";

                _chatPersistenceService.TrackMessageSend(me.Player.MembershipId, Context.ConnectionId);

                var model = new
                {
                    User = name,
                    IsStaff = ChatStatics.Staff.ContainsKey(me.Player.MembershipId),
                    Color = colorOut,
                    Pic = pic,
                    Message = WebUtility.HtmlEncode(output.Text),
                    MessageType = Enum.GetName(output.MessageType.GetType(), output.MessageType),
                    Timestamp = DateTime.UtcNow.ToUnixTime(),
                };

                if (_chatPersistenceService.HasNameChanged(me.Player.MembershipId, name))
                {
                    _chatPersistenceService.TrackPlayerNameChange(me.Player.MembershipId, name);
                    Clients.Caller.nameChanged(name);
                }

                Clients.Group(room).addNewMessageToPage(model);
                ChatLogProcedures.WriteLogToDatabase(room, name, output.Text);
            }

            UpdateUserList(room);
        }

        public Task JoinRoom(string roomName)
        {
            var me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(Context.User.Identity.GetUserId());

            if (!_chatPersistenceService.GetRoomsPlayerIsIn(me.Player.MembershipId).Contains(roomName))
                SendNoticeToRoom(roomName, me.Player, " has joined the room.");

            _chatPersistenceService.TrackRoomJoin(me.Player.MembershipId, Context.ConnectionId, roomName);
            UpdateUserList(roomName);

            return Groups.Add(Context.ConnectionId, roomName);
        }

        private void SendNoticeToRoom(string roomName, Player_VM me, string text)
        {
            if (me.BotId < AIStatics.ActivePlayerBotId)
                return;

            var model = new
            {
                User = me.GetDescriptor().Item1,
                IsStaff = ChatStatics.Staff.ContainsKey(me.MembershipId),
                Message = WebUtility.HtmlEncode(text),
                MessageType = Enum.GetName(typeof(MessageType), MessageType.Notification),
                Timestamp = DateTime.UtcNow.ToUnixTime(),
            };

            if (!ChatStatics.HideOnJoinChat.Contains(me.MembershipId))
                Clients.Group(roomName).addNewMessageToPage(model);
        }

        private void UpdateUserList(string room, bool includeCaller = true)
        {
            var usersInChat = _chatPersistenceService.GetUsersInRoom(room)
                .Select(x => new
                {
                    User = x.Name,
                    LastActive = x.Connections
                        .Where(con => con.Room == room)
                        .OrderByDescending(con => con.LastActivity)
                        .First().LastActivity.ToUnixTime(),
                    x.IsDonator,
                    IsStaff = ChatStatics.Staff.ContainsKey(x.MembershipId),
                })
                .ToList();

            var userList = new
            {
                Staff = usersInChat.Where(x => x.IsStaff)
                .OrderBy(x => x.User)
                .Select(uic => new { uic.User, LastActivity = uic.LastActive }),

                Donators = usersInChat.Where(x => !x.IsStaff && x.IsDonator)
                .OrderBy(x => x.User)
                .Select(uic => new { uic.User, LastActivity = uic.LastActive }),

                Users = usersInChat.Where(x => !x.IsStaff && !x.IsDonator)
                .OrderBy(x => x.User)
                .Select(uic => new { uic.User, LastActivity = uic.LastActive }),
            };

            Clients.Group(room).updateUserList(userList);

            if (includeCaller)
                Clients.Caller.updateUserList(userList);
        }
    }
}
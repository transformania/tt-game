using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Chat.Commands;
using TT.Domain.Concrete;
using TT.Domain.Legacy.Procedures;
using TT.Domain.Models;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Web.CustomHtmlHelpers;
using TT.Web.Extensions;
using TT.Web.Services;

namespace TT.Web.Hubs
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
            SendAs(message, PlayerProcedures.GetPlayerFormViewModel_FromMembership(Context.User.Identity.GetUserId()));
        }

        public void SendAs(string message, PlayerFormViewModel me)
        {
            string room = Clients.Caller.toRoom;
            
            me.Player.UpdateOnlineActivityTimestamp();

            if (me.Player.BotId == AIStatics.ActivePlayerBotId && DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.Player.MembershipId}))
            {
                return;
            }
            
            // Assert player is not banned
            if (me.Player.IsBannedFromGlobalChat && room == "global")
                return;

            if (!message.StartsWith("/me") && JokeShopProcedures.HUSHED_EFFECT.HasValue && EffectProcedures.PlayerHasActiveEffect(me.Player.ToDbPlayer(), JokeShopProcedures.HUSHED_EFFECT.Value))
            {
                String[] denied = {"/me tries to speak but cannot!",
                                   "/me puffs profusely but doesn't make a sound!",
                                   "/me is unable to utter a single word!",
                                   "/me gesticulates wildly in the hope of communicating!",
                                   "/me is afflicted by a magical field leaving them completely mute!",
                                   "/me has been hushed!",
                                   "/me tries to mouth some words in the hope you can understand!",
                                   "/me has lost their voice!",
                                   "/me has their lips sealed!",
                                   "/me will be unable to speak until the enchantment cast on them has lapsed!",
                                   "/me is counting down the minutes until they are able to speak again!",
                                   "/me tries to communicate through telepathy!"};
                message = denied[new Random().Next(denied.Count())];
            }

            // Get player picture and name
            var pic = HtmlHelpers.GetImageURL(me, true).ToString();
            var descriptor = me.Player.GetDescriptor();

            var name = descriptor.Item1;
            pic = string.IsNullOrWhiteSpace(descriptor.Item2) ? pic : descriptor.Item2;

            if (me.Player.BotId == AIStatics.ActivePlayerBotId)
            {
                if (Context.User.IsInRole(PvPStatics.Permissions_Developer))
                {
                    name = name + " (Dev)";
                }
                else if (Context.User.IsInRole(PvPStatics.Permissions_Admin))
                {
                    name = name + " (Admin)";
                }
                else if (Context.User.IsInRole(PvPStatics.Permissions_Moderator))
                {  
                    switch (me.Player.MembershipId)
                    {
                        case "d465db1c-ba4f-4347-b666-4dfd1c9a5e33": //Because Martha wants to be a filthy casual.
                            break;
                        case "08b476c3-d262-45b6-9e6a-7d94b472fefe":
                            break; //So is this one.
                        default:
                            name = name + " (Mod)";
                            break;
                    }
                }
            }

            // Performs message processing to correctly format any special text
            var output = ChatMessageProcessor.ProcessMessage(new MessageData(name, message));

            if (!string.IsNullOrWhiteSpace(output.Text))
            {
                var colorOut = output.SendPlayerChatColor ? me.Player.ChatColor : "";

                if (me.Player.BotId == AIStatics.ActivePlayerBotId)
                {
                    _chatPersistenceService.TrackMessageSend(me.Player.MembershipId, Context.ConnectionId);
                }

                var model = new
                {
                    User = name,
                    IsStaff = me.Player.BotId == AIStatics.ActivePlayerBotId ? ChatStatics.Staff.ContainsKey(me.Player.MembershipId) : false,
                    Color = colorOut,
                    Pic = pic,
                    Message = WebUtility.HtmlEncode(output.Text),
                    MessageType = Enum.GetName(output.MessageType.GetType(), output.MessageType),
                    Timestamp = DateTime.UtcNow.ToUnixTime(),
                };

                if (me.Player.BotId == AIStatics.ActivePlayerBotId && _chatPersistenceService.HasNameChanged(me.Player.MembershipId, name))
                {
                    _chatPersistenceService.TrackPlayerNameChange(me.Player.MembershipId, name);
                    Clients.Caller.nameChanged(name);
                }

                Clients.Group(room).addNewMessageToPage(model);

                DomainRegistry.Repository.Execute(new CreateChatLog
                {
                    Message = output.Text,
                    Room = room,
                    Name = name,
                    UserId = me.Player.MembershipId,
                    Color = me.Player.ChatColor,
                    PortraitUrl = pic
                });
            }

            UpdateUserList(room);

            // NPC dice prank
            if (room == "global" && me.Player.BotId == AIStatics.ActivePlayerBotId &&
                message.StartsWith("/roll") && message.Contains("4d20") &&
                JokeShopProcedures.IsJokeShopActive())
            {
                var rand = new Random();

                if (rand.Next(10) == 0)
                {
                    IPlayerRepository playerRepo = new EFPlayerRepository();
                    var npcIds = playerRepo.Players.Where(p => p.BotId <= AIStatics.PsychopathBotId &&
                                                               p.Mobility == PvPStatics.MobilityFull)
                                                   .Select(p => p.Id).ToArray();

                    if (npcIds.Any())
                    {
                        var npcId = npcIds[rand.Next(npcIds.Count())];
                        SendAs(message, PlayerProcedures.GetPlayerFormViewModel(npcId));
                    }
                }
            }

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
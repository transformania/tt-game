using System.Linq;
using Microsoft.AspNet.SignalR;
using tfgame.dbModels.Models;
using tfgame.dbModels.Queries.Player;
using tfgame.Extensions;
using tfgame.Procedures;
using tfgame.Services;
using tfgame.Statics;
using WebMatrix.WebData;
using System.Threading.Tasks;
using tfgame.CustomHtmlHelpers;

namespace tfgame.Chat
{
    public class ChatHub : Hub
    {
        private readonly ChatService chatService;
        
        public ChatHub()
        {
            chatService = new ChatService();
            chatService.NameChanged += OnNameChanged;
        }

        public override Task OnConnected()
        {
            var me = new GetPlayerFromUserName { UserName = Context.User.Identity.Name }.Find();
            chatService.OnUserConnected(me, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            var connectionId = Context.ConnectionId;
            var room = string.Empty;
            var me = new GetPlayerFromUserName { UserName = Context.User.Identity.Name }.Find();
            
            if (ChatService.ChatPersistance.ContainsKey(me.MembershipId))
            {
                var connection = ChatService.ChatPersistance[me.MembershipId].Connections.SingleOrDefault(x => x.ConnectionId == connectionId);
                if (connection != null && connection.Room != null)
                    room = connection.Room;
            }
            
            chatService.OnUserDisconnected(me, connectionId);

            var chatUser = (ChatService.ChatPersistance.ContainsKey(me.MembershipId) ? ChatService.ChatPersistance[me.MembershipId] : null);

            if (string.IsNullOrWhiteSpace(room) || (chatUser != null && chatUser.InRooms.Contains(room)))
                return base.OnDisconnected();

            SendNoticeToRoom(room, me, "has left the room.");
            UpdateUserList(room, false);

            return base.OnDisconnected();
        }

        public void OnNameChanged(object sender, ChatService.NameChangedEventArgs e)
        {
            foreach (var room in ChatService.ChatPersistance[e.MembershipId].InRooms)
                UpdateUserList(room);
        }

        public void Send(string name, string message)
        {
            string room = Clients.Caller.toRoom;
            var me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(WebSecurity.CurrentUserId);
            
            chatService.MarkOnlineActivityTimestamp(me.Player);

            // Assert player is not banned
            if (me.Player.IsBannedFromGlobalChat && room == "global")
                return;

            // Get player picture and name
            var pic = CharactersHere.GetImageURL(me, true).ToString();
            var descriptor = chatService.GetPlayerDescriptorFor(me.Player);

            name = descriptor.Item1;
            pic = string.IsNullOrWhiteSpace(descriptor.Item2) ? pic : descriptor.Item2;

            // Performs message processing to correctly format any special text
            var output = ChatMessageProcessor.ProcessMessage(new MessageData(name, message));

            Clients.Group(room).addNewMessageToPage(pic, output.SendNameToClient ? name : "", output.Text, output.SendPlayerChatColor ? me.Player.ChatColor : "");

            ChatLogProcedures.WriteLogToDatabase(room, name, output.Text);
            chatService.OnUserSentMessage(me.Player, Context.ConnectionId);

            UpdateUserList(room);
        }

        public Task JoinRoom(string roomName)
        {
            var me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(WebSecurity.CurrentUserId);

            if (!ChatService.ChatPersistance[me.Player.MembershipId].InRooms.Contains(roomName))
                SendNoticeToRoom(roomName, me.Player, "has joined the room.");

            chatService.OnUserJoinRoom(me.Player, Context.ConnectionId, roomName);
            UpdateUserList(roomName);

            return Groups.Add(Context.ConnectionId, roomName);
        }

        private void SendNoticeToRoom(string roomName, Player_VM me, string text)
        {
            try
            {
                if (me.MembershipId <= 0) 
                    return;

                var message = string.Format("[-[{0} {1}]-]", me.GetFullName(), text);

                if (!ChatStatics.HideOnJoinChat.Contains(me.MembershipId))
                    Clients.Group(roomName).addNewMessageToPage("", "", message, me.ChatColor);
            }
            catch
            {
            }
        }

        private void UpdateUserList(string room, bool includeCaller = true)
        {
            var usersInChat = ChatService.ChatPersistance
                .Where(x => x.Value.InRooms.Contains(room))
                .Select(x => new
                {
                    User = x.Value.Name,
                    LastActive = x.Value.Connections
                        .Where(con => con.Room == room)
                        .OrderByDescending(con => con.LastActivity)
                        .First().LastActivity.ToUnixTime(),
                    IsDonator = x.Value.IsDonator,
                    IsStaff = ChatStatics.Staff.ContainsKey(x.Key),
                })
                .ToList();

            var userList = new
            {
                Staff = usersInChat.Where(x => x.IsStaff)
                .OrderBy(x => x.User)
                .Select(uic => new { User = uic.User, LastActivity = uic.LastActive }),

                Donators = usersInChat.Where(x => !x.IsStaff && x.IsDonator)
                .OrderBy(x => x.User)
                .Select(uic => new { User = uic.User, LastActivity = uic.LastActive }),

                Users = usersInChat.Where(x => !x.IsStaff && !x.IsDonator)
                .OrderBy(x => x.User)
                .Select(uic => new { User = uic.User, LastActivity = uic.LastActive }),
            };

            Clients.Group(room).updateUserList(userList);
            
            if (includeCaller)
                Clients.Caller.updateUserList(userList);
        }
    }
}
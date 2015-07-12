using Microsoft.AspNet.SignalR;
using tfgame.dbModels.Queries.Player;
using tfgame.Procedures;
using tfgame.Services;
using WebMatrix.WebData;
using System.Threading.Tasks;
using tfgame.ViewModels;
using tfgame.CustomHtmlHelpers;

namespace tfgame.Chat
{
    public class ChatHub : Hub
    {
        private readonly ChatService chatService;
        
        public ChatHub()
        {
            chatService = new ChatService();
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
        }

        public override Task OnConnected()
        {
            var me = new GetPlayerFromUserName {UserName = Context.User.Identity.Name}.Find();
            chatService.OnUserConnected(me, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            var me = new GetPlayerFromUserName { UserName = Context.User.Identity.Name }.Find();
            chatService.OnUserDisconnected(me, Context.ConnectionId);

            return base.OnDisconnected();
        }

        public Task JoinRoom(string roomName)
        {
            PlayerFormViewModel me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(WebSecurity.CurrentUserId);

            try
            {
                if (me.Player.MembershipId > 0)
                {
                    string message = "[-[" + me.Player.GetFullName() + " has joined the room.]-]";

                    if (me.Player.MembershipId != 69 && me.Player.MembershipId != 3490)
                    {
                        Clients.Group(roomName).addNewMessageToPage("", "", message, me.Player.ChatColor);
                    }
                }
            }
            catch
            {

            }

            chatService.OnUserJoinRoom(me.Player, Context.ConnectionId, roomName);
            return Groups.Add(Context.ConnectionId, roomName);
        }
    }
}
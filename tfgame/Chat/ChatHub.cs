using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using tfgame.Procedures;
using tfgame.Statics;
using WebMatrix.WebData;
using tfgame.dbModels.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using System.Collections.Generic;




namespace tfgame.Chat
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {

            string room = Clients.Caller.toRoom;
            //string test = Context.

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is not banned
            if (me.IsBannedFromGlobalChat == true && room == "global")
            {
                return;
            }

            if (me.MembershipId == 69)
            {
                name = "Judoo (admin)";
            }
            else if (me.MembershipId == 3490)
            {
                name = "Mizuho (dev)";
            }
            else if (me.MembershipId == 251)
            {
                name = "Arrhae (dev)";
            }
            else if (me.MembershipId == -1)
            {

            }
            else
            {
                name = me.GetFullName();
            }



            if (name != " " && name != "" && message != "")
            {
                if (message.Contains("[luxa]") || message.Contains("[blanca]")  || message.Contains("[poll]")  || message.Contains("[fp]"))
                {
                    if (HttpContext.Current.User.IsInRole(PvPStatics.Permissions_Moderator) || HttpContext.Current.User.IsInRole(PvPStatics.Permissions_Admin))
                    {
                        message += "   [.[" + DateTime.UtcNow.ToShortTimeString() + "].]";
                        Clients.Group(room).addNewMessageToPage(name, message, me.ChatColor);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }
                    else
                    {
                        message = message.Replace("[luxa]", " ");
                        message = message.Replace("[blanca]", " ");
                        message = message.Replace("[poll]", " ");
                        message = message.Replace("[fp]", " ");
                        message += "   [.[" + DateTime.UtcNow.ToShortTimeString() + "].]";
                        Clients.Group(room).addNewMessageToPage(name, message, me.ChatColor);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }
                }
                else if (message.StartsWith("/dm message"))
                {
                    message = message.Replace("/dm message", "");
                    string output = "[=[" + name + " [DM]:  " + message + "]=]";
                    Clients.Group(room).addNewMessageToPage("", output, me.ChatColor);
                    ChatLogProcedures.WriteLogToDatabase(room, name, output);
                }

                else if (message.StartsWith("/dm"))
                {
                    message = RPCommand(message);
                    if (message == "")
                    {
                        // do nothing, bad command
                    }
                    else
                    {
                        message = "[=[" + message + "]=]";
                        Clients.Group(room).addNewMessageToPage(name, message, me.ChatColor);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }

                }
                else if (message.StartsWith("/me"))
                {
                    message = message.Replace("/me", "");
                    string output = "[+[" + name + message + "]+]";
                    Clients.Group(room).addNewMessageToPage("", output, me.ChatColor);
                    ChatLogProcedures.WriteLogToDatabase(room, name, output);
                }

               

                else if (message.StartsWith("/roll"))
                {
                    Match m = Regex.Match(message, @"/roll d([0-9]*)");
                    if (m.Success)
                    {
                        Match x = Regex.Match(message, @"\d+");
                        int value = Convert.ToInt32(x.Value);
                        message = "[-[" + name + " rolled a " + PlayerProcedures.RollDie(value) + " (d" + value + ").]-]";
                        Clients.Group(room).addNewMessageToPage("", message);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }
                }
                else
                {
                    message += "   [.[" + DateTime.UtcNow.ToShortTimeString() + "].]";
                    Clients.Group(room).addNewMessageToPage(name, message, me.ChatColor);
                    ChatLogProcedures.WriteLogToDatabase(room, name, message);
                }
            }
            else
            {

            }
        }

        public Task JoinRoom(string roomName)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            try
            {
                if (me.MembershipId > 0)
                {
                    string message = "[-[" + me.GetFullName() + " has joined the room.]-]";

                    if (me.MembershipId != 69 && me.MembershipId != 3490)
                    {
                        Clients.Group(roomName).addNewMessageToPage("", message, me.ChatColor);
                    }
                }
            }
            catch
            {

            }
            return Groups.Add(Context.ConnectionId, roomName);
        }

        public string RPCommand(string input)
        {
            string output = "";
            if (input.StartsWith("/dm"))
            {

                int tagIndex = input.IndexOf(':') + 1;
                string tag = "";

                if (tagIndex > 0)
                {
                    tag = input.Substring(tagIndex);
                }


                if (input.StartsWith("/dm creature"))
                {
                    output = DMRollProcedures.GetRoll("creature", tag);
                }
                else if (input.StartsWith("/dm item"))
                {
                    output = DMRollProcedures.GetRoll("item", tag);
                }
                else if (input.StartsWith("/dm event"))
                {
                    output = DMRollProcedures.GetRoll("event", tag);
                }
                else if (input.StartsWith("/dm trap"))
                {
                    output = DMRollProcedures.GetRoll("trap", tag);
                }
                else if (input.StartsWith("/dm tf.animate"))
                {
                    output = DMRollProcedures.GetRoll("tf.animate", tag);
                }
                else if (input.StartsWith("/dm tf.inanimate"))
                {
                    output = DMRollProcedures.GetRoll("tf.inanimate", tag);
                }
                else if (input.StartsWith("/dm tf.animal"))
                {
                    output = DMRollProcedures.GetRoll("tf.animal", tag);
                }
                else if (input.StartsWith("/dm tf.partial"))
                {
                    output = DMRollProcedures.GetRoll("tf.partial", tag);
                }

                //    output = "DM (" + ;

                else
                {
                    return "";
                }

                return output;
            }
            else
            {
                return output;
            }
        }

        // public static void addEnterMessageToPage(Message)
    }
}
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
using tfgame.ViewModels;
using tfgame.CustomHtmlHelpers;




namespace tfgame.Chat
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {

            string room = Clients.Caller.toRoom;
            PlayerFormViewModel me = PlayerProcedures.GetPlayerFormViewModel_FromMembership(WebSecurity.CurrentUserId);

            string pic = CharactersHere.GetImageURL(me, true).ToString();

            // assert player is not banned
            if (me.Player.IsBannedFromGlobalChat == true && room == "global")
            {
                return;
            }

            if (me.Player.MembershipId == 69)
            {
                name = "Judoo (admin)";
                pic = "/Images/PvP/portraits/Thumbnails/100/Judoo.jpg";
            }
            else if (me.Player.MembershipId == 3490)
            {
                name = "Mizuho (dev)";
                pic = "/Images/PvP/portraits/Thumbnails/100/Mizuho.jpg";
            }
            else if (me.Player.MembershipId == 251)
            {
                name = "Arrhae (dev)";
                // Arrhae wants to keep regular portrait for now, not admin/dev custom one
            }
            else if (me.Player.MembershipId == -1)
            {

            }
            else
            {
                name = me.Player.GetFullName();
            }



            if (name != " " && name != "" && message != "")
            {
                if (message.Contains("[luxa]") || message.Contains("[blanca]")  || message.Contains("[poll]")  || message.Contains("[fp]"))
                {
                    if (HttpContext.Current.User.IsInRole(PvPStatics.Permissions_Moderator) || HttpContext.Current.User.IsInRole(PvPStatics.Permissions_Admin))
                    {
                        message += "   [.[" + DateTime.UtcNow.ToShortTimeString() + "].]";
                        Clients.Group(room).addNewMessageToPage(pic, name, message, me.Player.ChatColor);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }
                    else
                    {
                        message = message.Replace("[luxa]", " ");
                        message = message.Replace("[blanca]", " ");
                        message = message.Replace("[poll]", " ");
                        message = message.Replace("[fp]", " ");
                        message += "   [.[" + DateTime.UtcNow.ToShortTimeString() + "].]";
                        Clients.Group(room).addNewMessageToPage(pic, name, message, me.Player.ChatColor);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }
                }
                else if (message.StartsWith("/dm message"))
                {
                    message = message.Replace("/dm message", "");
                    string output = "[=[" + name + " [DM]:  " + message + "]=]";
                    Clients.Group(room).addNewMessageToPage(pic, "", output, me.Player.ChatColor);
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
                        Clients.Group(room).addNewMessageToPage(pic, name, message, me.Player.ChatColor);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }

                }
                else if (message.StartsWith("/me"))
                {
                    message = message.Replace("/me", "");
                    string output = "[+[" + name + message + "]+]";
                    Clients.Group(room).addNewMessageToPage(pic, "", output, me.Player.ChatColor);
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
                        Clients.Group(room).addNewMessageToPage(pic, "", message);
                        ChatLogProcedures.WriteLogToDatabase(room, name, message);
                    }
                }
                else
                {
                    message += "   [.[" + DateTime.UtcNow.ToShortTimeString() + "].]";
                    Clients.Group(room).addNewMessageToPage(pic, name, message, me.Player.ChatColor);
                    ChatLogProcedures.WriteLogToDatabase(room, name, message);
                }
            }
            else
            {

            }
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
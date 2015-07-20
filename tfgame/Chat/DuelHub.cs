using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using tfgame.Procedures;
using tfgame.dbModels.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using System.Collections.Generic;
using tfgame.ViewModels;
using System.Timers;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;



namespace tfgame.Chat
{
    public class DuelHub : Hub
    {

        public void Send(string input)
        {
            string room = Clients.Caller.toRoom;
            Player me = PlayerProcedures.GetPlayerFromMembership(Convert.ToInt32(Context.User.Identity.GetUserId()));
           // Clients.Group(room).addNewMessageToPage(input);
        }

        public Task JoinRoom(string roomName)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(Convert.ToInt32(Context.User.Identity.GetUserId()));
            string message = "[-[" + me.FirstName + " " + me.LastName + " has joined the room.]-]";
            Clients.Group(roomName).addNewMessageToPage(message);
            return Groups.Add(Context.ConnectionId, roomName);
        }

    }


    public static class Duel
    {
        public static PlayerFormViewModel P1_PlayerForm { get; set; }
        public static PlayerFormViewModel P2_PlayerForm { get; set; }
        public static DuelPlayerForm P1 { get; set; }
        public static DuelPlayerForm P2 { get; set; }
        public static DbStaticSkill P1_Spell { get; set; }
        public static DbStaticSkill P2_Spell { get; set; }


        public static Timer Timer { get; set; }

        public static void Begin()
        {
            P1_PlayerForm = PlayerProcedures.GetPlayerFormViewModel(35649);
            P1_PlayerForm = PlayerProcedures.GetPlayerFormViewModel(35649);
            P1 = new DuelPlayerForm
            {

            };
        }

        public static void RunTick(object source, ElapsedEventArgs e)
        {

            ClientData output = new ClientData();
            output.p1 = P1;
            output.p2 = P2;

            var context = GlobalHost.ConnectionManager.GetHubContext<DuelHub>();
            context.Clients.Group("duel").addNewMessageToPage(output);
        }

        public static void MovePlayer()
        {

        }
    }

    public class ClientData
    {
        [JsonProperty("p1")]
        public DuelPlayerForm p1 { get; set; }
        [JsonProperty("p2")]
        public DuelPlayerForm p2 { get; set; }

        //[JsonProperty("pentities")]
        //public List<EntityLocation> entities { get; set; }
    }

    //public class EntityLocation {
    //    [JsonProperty("entity")]
    //    public string entity { get; set;}
    //    [JsonProperty("x")]
    //    public decimal X { get; set;}
    //    [JsonProperty("y")]
    //    public decimal Y { get; set;}
    //}

    public class DuelPlayerForm {
        [JsonProperty("wp")]
        public decimal WP { get; set; }
        [JsonProperty("mn")]
        public decimal Mana { get; set; }
        [JsonProperty("x")]
        public decimal X { get; set; }
        [JsonProperty("y")]
        public decimal Y { get; set; }
        [JsonProperty("ax")]
        public decimal Attack_X { get; set; }
        [JsonProperty("ay")]
        public decimal Attack_Y { get; set; }
    }

}
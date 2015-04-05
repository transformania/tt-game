using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using tfgame.Procedures;
using WebMatrix.WebData;
using tfgame.dbModels.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using System.Collections.Generic;
using tfgame.ViewModels;
using System.Timers;
using Newtonsoft.Json;

namespace tfgame.Chat
{
    public class NoticeHub : Hub
    {

        public Task Connect()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            string room = "_" + me.Id;
            return Groups.Add(Context.ConnectionId, room);
        }

        public void ReceiveNotice(int playerId, string message)
        {
            string room = "_" + playerId;
            Clients.Group(room).receiveNotice(message);
        }

        

    }
}
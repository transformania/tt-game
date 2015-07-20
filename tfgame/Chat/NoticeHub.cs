using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using tfgame.Procedures;
using tfgame.dbModels.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.Extensions;
using System.Collections.Generic;
using tfgame.ViewModels;
using System.Timers;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;

namespace tfgame.Chat
{
    public class NoticeHub : Hub
    {

        public Task Connect()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(Context.User.Identity.GetCurrentUserId());

            int minimumDonatorLevelForNotifications = 0;

            if (me.DonatorLevel >= minimumDonatorLevelForNotifications)
            {
                string room = "_" + me.Id;
                return Groups.Add(Context.ConnectionId, room);
            }

            // do nothing...
            return Task.FromResult(0);
           
        }

        public void ReceiveNotice(int playerId, string message)
        {
            string room = "_" + playerId;
            Clients.Group(room).receiveNotice(message);
        }

        

    }
}
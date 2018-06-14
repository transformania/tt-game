using System;
using System.Web.Mvc;
using FeatureSwitch;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Chat.Queries;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class ChatController : Controller
    {
        public virtual ActionResult Index(string room)
        {
            return FeatureContext.IsEnabled<ChatV2>() ? ChatV2() : ChatV1(room);
        }

        private ActionResult ChatV1(string room)
        {
            var userId = User.Identity.GetUserId();
            var me = new GetPlayerFormFromMembership { MembershipId = userId }.Find();

            if (!me.CanAccessChat())
                return View(MVC.PvP.Views.MakeNewCharacter);

            if (string.IsNullOrWhiteSpace(room) || room.StartsWith("_"))
            {
                TempData["Result"] = "A chat room must have a name and not begin with an underscore";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (me.Player.IsBannedFromGlobalChat && room == "global")
            {
                TempData["Error"] = "A moderator has temporarily banned you from global chat.";
                TempData["SubError"] = "To restore your chat priveliges please make an appeal on the forums.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var model = new ChatViewModel
            {
                RoomName = room,
                ChatUser = me.Player.GetDescriptor().Item1,
                ChatColor = me.Player.ChatColor,
            };

            return View(MVC.Chat.Views.ChatIndex, model);
        }

        private ActionResult ChatV2()
        {
            return View(MVC.Chat.Views.Chat);
        }

        public virtual ActionResult Chat(string room)
        {
            return RedirectToAction(MVC.Chat.Index(room));
        }

        public virtual ActionResult PrivateChat()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            if (me == null || me.BotId == AIStatics.RerolledPlayerBotId || me.FirstName.IsNullOrEmpty() || me.LastName.IsNullOrEmpty())
            {
                return View(MVC.PvP.Views.MakeNewCharacter);
            }
            return View(MVC.Chat.Views.PrivateBegin);
        }

        public virtual ActionResult ChatLog(string room, string filter)
        {
            var model = new ChatLogViewModel
            {
                Room = room,
                Filter = filter,
                ChatLog = DomainRegistry.Repository.Find(new GetChatLogs { Room = room, Filter = filter })
            };
            return View(MVC.Chat.Views.ChatLog, model);
        }

        public virtual ActionResult ChatCommands()
        {
            return View(MVC.Chat.Views.ChatCommands);
        }
    }
}
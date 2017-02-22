using System.Web.Mvc;
using FeatureSwitch;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Queries.Players;
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
    }
}
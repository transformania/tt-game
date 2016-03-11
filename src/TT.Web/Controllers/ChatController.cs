using System.Web.Mvc;
using FeatureSwitch;
using Microsoft.AspNet.Identity;
using TT.Domain.Queries.Player;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        public ActionResult Index(string room)
        {
            return FeatureContext.IsEnabled<ChatV2>() ? ChatV2() : ChatV1(room);
        }

        private ActionResult ChatV1(string room)
        {
            var userId = User.Identity.GetUserId();
            var me = new GetPlayerFormFromMembership { MembershipId = userId }.Find();

            if (!me.CanAccessChat())
                return View("~/Views/PvP/MakeNewCharacter.cshtml");

            if (string.IsNullOrWhiteSpace(room) || room.StartsWith("_"))
            {
                TempData["Result"] = "A chat room must have a name and not begin with an underscore";
                return RedirectToAction("Play", "PvP");
            }

            if (me.Player.IsBannedFromGlobalChat && room == "global")
            {
                TempData["Error"] = "A moderator has temporarily banned you from global chat.";
                TempData["SubError"] = "To restore your chat priveliges please make an appeal on the forums.";
                return RedirectToAction("Play", "PvP");
            }

            var model = new ChatViewModel
            {
                RoomName = room,
                ChatUser = me.Player.GetDescriptor().Item1,
                ChatColor = me.Player.ChatColor,
            };

            return View("ChatIndex", model);
        }

        private ActionResult ChatV2()
        {
            return View("Chat");
        }
    }
}
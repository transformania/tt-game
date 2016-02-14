using System.Web.Mvc;
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
            var userId = User.Identity.GetUserId();
            var me = new GetPlayerFormFromMembership { MembershipId = userId }.Find();

            if (!me.CanAccessChat())
                return View("~/Views/PvP/MakeNewCharacter.cshtml");

            var roomName = room;

            if (string.IsNullOrWhiteSpace(roomName) || roomName.StartsWith("_"))
            {
                TempData["Result"] = "A chat room must have a name and not begin with an underscore";
                return RedirectToAction("Play", "PvP");
            }

            if (me.Player.IsBannedFromGlobalChat && roomName == "global")
            {
                TempData["Error"] = "A moderator has temporarily banned you from global chat.";
                TempData["SubError"] = "To restore your chat priveliges please make an appeal on the forums.";
                return RedirectToAction("Play", "PvP");
            }

            var model = new ChatViewModel
            {
                RoomName = roomName,
                ChatUser = me.Player.GetDescriptor().Item1,
                ChatColor = me.Player.ChatColor,
            };

            return View("ChatIndex", model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("Chat");
        }
    }
}
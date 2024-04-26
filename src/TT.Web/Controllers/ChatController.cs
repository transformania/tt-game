using System;
using System.Web.Mvc;
using FeatureSwitch;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Chat.Queries;
using TT.Domain.Items.Queries;
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

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = userId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            if (!me.CanAccessChat())
                return View(MVC.PvP.Views.MakeNewCharacter);

            if (string.IsNullOrWhiteSpace(room) || room.StartsWith("_"))
            {
                TempData["Result"] = "A chat room must have a name and not begin with an underscore";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (me.Player.IsBannedFromGlobalChat && room == "global")
            {
                TempData["Error"] = "A moderator has temporarily banned you from global chat for the following reason: " + me.Player.ChatLockoutMessage;
                TempData["SubError"] = "To restore your chat privileges please make an appeal on Discord.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var roomName = room;

            if (!UserCanSeeRoom(ref roomName))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var model = new ChatViewModel
            {
                Room = room,
                RoomName = roomName,
                ChatUser = me.Player.GetDescriptor().Item1,
                ChatColor = me.Player.ChatColor,
            };

            ViewBag.Result = TempData["Result"];

            return View(MVC.Chat.Views.ChatIndex, model);
        }

        private bool UserCanSeeRoom(ref string roomName)
        {
            if (roomName.StartsWith("coven_"))
            {
                var userId = User.Identity.GetUserId();
                var me = PlayerProcedures.GetPlayerFromMembership(userId);

                if (User.IsInRole(PvPStatics.Permissions_Moderator) || User.IsInRole(PvPStatics.Permissions_Admin))
                {
                    // Always grant access for moderators
                    if (me.Covenant == null || me.Covenant <= 0 || roomName != $"coven_{me.Covenant}")
                    {
                        // User is not a member of this coven
                        TempData["Result"] = "You are in this covenant chat room for moderation purposes.";
                    }
                }
                else if (me.Covenant == null || me.Covenant <= 0)
                {
                    TempData["Error"] = "You are not in a covenant and cannot access this covenant chat room.";
                    return false;
                }
                else if (roomName != $"coven_{me.Covenant}")
                {
                    TempData["Error"] = "You are not in this covenant and cannot access its chat room.";
                    return false;
                }

                // Player is in coven or a moderator
                var covenId = roomName.Substring(6).Parse<int>();
                var coven = CovenantProcedures.GetDbCovenant(covenId);
                if (coven != null)
                {
                    roomName = $"{coven.Name} (covenant)";
                }
            }
            else if (roomName.StartsWith("owner_"))
            {
                var userId = User.Identity.GetUserId();
                var me = PlayerProcedures.GetPlayerFromMembership(userId);

                if (roomName != $"owner_{me.Id}")
                {
                    // Not the owner
                    var item = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });

                    if (item == null || item.Owner == null || roomName != $"owner_{item.Owner.Id}")
                    {
                        // Not owned by the player
                        if (User.IsInRole(PvPStatics.Permissions_Moderator) || User.IsInRole(PvPStatics.Permissions_Admin))
                        {
                            TempData["Result"] = "You are in this owner chat room for moderation purposes.";
                        }
                        else
                        {
                            TempData["Error"] = "You do not have access to this owner chat room.";
                            return false;
                        }
                    }
                }

                var ownerId = roomName.Substring(6).Parse<int>();
                var owner = PlayerProcedures.GetPlayerFormViewModel(ownerId);
                if (owner != null && owner.Player != null)
                {
                    roomName = $"{owner.Player.GetFullName()} (owner chat)";
                }
            }

            return true;
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
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var roomName = room;

            if (!UserCanSeeRoom(ref roomName))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var model = new ChatLogViewModel
            {
                Room = room,
                RoomName = roomName,
                Filter = filter,
                ChatLog = DomainRegistry.Repository.Find(new GetChatLogs { Room = room, Filter = filter })
            };

            ViewBag.Result = TempData["Result"];

            return View(MVC.Chat.Views.ChatLog, model);
        }

        public virtual ActionResult ChatCommands()
        {
            return View(MVC.Chat.Views.ChatCommands);
        }
    }
}
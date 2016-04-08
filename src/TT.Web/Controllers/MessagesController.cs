using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.ViewModels;
using TT.Web.Services;

namespace TT.Web.Controllers
{
    public class MessagesController : Controller
    {
        // GET: /Messages
        [HttpGet]
        [Authorize]
        public ActionResult Index(int offset = 0)
        {
            // this might fix some odd log-off message interception oddities... maybe?
            string myMembershipId = User.Identity.GetUserId();

            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            MessageBag output = MessageProcedures.GetPlayerMessages(me, offset);
            output.InboxSize = 150;

            // if you are inanimate and are being worn, grab the data on who is wearing you

            if (me.Mobility == "inanimate")
            {
                PlayerFormViewModel personWearingMe = ItemProcedures.BeingWornBy(me);
                if (personWearingMe != null)
                {
                    output.WearerId = personWearingMe.Player.Id;
                    output.WearerName = personWearingMe.Player.GetFullName();
                }
            }

            bool isDonator = DonatorProcedures.DonatorGetsMessagesRewards(me);

            ViewBag.IsDonator = isDonator;

            if (isDonator)
            {
                output.InboxSize = 500;
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View("Messages", output);
        }

        // POST: /Message/DeleteMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteMessage(bool deleteAll, int messageId)
        {
            string myMembershipId = User.Identity.GetUserId();

            if (deleteAll)
            {
                MessageProcedures.DeleteAllMessages(myMembershipId);
                return RedirectToAction("Index");
            }

            // assert player owns message
            if (!MessageProcedures.PlayerOwnsMessage(messageId, myMembershipId))
            {
                TempData["Error"] = "You can't delete this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction("Index");
            }

            MessageProcedures.DeleteMessage(messageId);
            return RedirectToAction("Index");
        }

        // GET: /Message/ReadMessage/{messageId}
        [HttpGet]
        [Authorize]
        public ActionResult ReadMessage(int messageId)
        {
            string myMembershipId = User.Identity.GetUserId();
            // assert player owns message
            if (!MessageProcedures.PlayerOwnsMessage(messageId, myMembershipId))
            {
                TempData["Error"] = "You can't read this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction("Index");
            }
            MessageViewModel output = MessageProcedures.GetMessageAndMarkAsRead(messageId);
            // ViewBag.IsReplyable = PlayerProcedures.GetPlayer(output)

            return View(output);
        }

        // POST: /Messages/MarkAsUnread
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult MarkAsUnread(int messageId)
        {
            string myMembershipId = User.Identity.GetUserId();
            // assert player owns message
            if (!MessageProcedures.PlayerOwnsMessage(messageId, myMembershipId))
            {
                TempData["Error"] = "You can't mark this message as unread.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction("Index");
            }
            MessageProcedures.MarkMessageAsUnread(messageId);

            TempData["Result"] = "Message marked as unread.";
            return RedirectToAction("Index");
        }

        // POST: /Messages/MarkAsUnread
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult MarkAsRead(int messageId)
        {
            string myMembershipId = User.Identity.GetUserId();
            // assert player owns message
            if (!MessageProcedures.PlayerOwnsMessage(messageId, myMembershipId))
            {
                TempData["Error"] = "You can't mark this message as read.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction("Index");
            }

            MessageProcedures.MarkMessageAsRead(messageId);

            TempData["Result"] = "Message marked as read.";
            return RedirectToAction("Index");
        }

        // GET: /Messages/Write
        [HttpGet]
        [Authorize]
        public ActionResult Write(int playerId, int responseTo = -1)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player sendingTo = PlayerProcedures.GetPlayer(playerId);
            MessageSubmitViewModel output = new MessageSubmitViewModel();

            output.SenderId = me.Id;
            output.ReceiverId = playerId;
            output.responseToId = responseTo;
            output.SendingToName = sendingTo.GetFullName();

            if (TempData["MessageText"] != null && TempData["ErrorMessage"] != null)
            {
                // preserves what the user typed if coming from SendMessage
                output.MessageText = TempData["MessageText"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (responseTo != -1)
            {
                MessageViewModel msgRepliedTo = MessageProcedures.GetMessage(responseTo);

                // assert the letter being replied to is yours
                if (msgRepliedTo.dbMessage.ReceiverId != me.Id)
                {
                    TempData["Result"] = "You can't reply to this message since the original was not sent to you.";
                    return RedirectToAction("Index");
                }
                else
                {
                    output.RespondingToMsg = msgRepliedTo.dbMessage.MessageText;
                }
            }

            return View("Write", output);
        }

        // POST: /Messages/SendMessage
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(MessageSubmitViewModel input)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player receiver = PlayerProcedures.GetPlayer(input.ReceiverId);

            // assert player is not banned from chat
            if (me.IsBannedFromGlobalChat)
            {
                TempData["Error"] = "You have been banned and cannot send any messages.";
                TempData["SubError"] = "If you feel this is in error or wish to make an appeal you may do so on the forums.";
                return RedirectToAction("Play", "PvP");
            }

            // assert no blacklist exists
            if (BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, receiver, "message"))
            {
                TempData["Error"] = "This player has blacklisted you or is on your own blacklist.";
                TempData["SubError"] = "You cannot send messages to players who have blacklisted you.  Remove them from your blacklist or ask them to remove you from theirs.";
                return RedirectToAction("Play", "PvP");
            }

            if (input.MessageText == null || input.MessageText == "")
            {
                TempData["ErrorMessage"] = "You need to write something to send to this person.";
                TempData["MessageText"] = input.MessageText;
                return RedirectToAction("Write", new { playerId = input.ReceiverId, responseTo = input.responseToId });

            }

            if (input.MessageText.Length > 1000)
            {
                TempData["ErrorMessage"] = "Your message is too long.";
                TempData["MessageText"] = input.MessageText;
                return RedirectToAction("Write", new { playerId = input.ReceiverId, responseTo = input.responseToId });
            }

            MessageProcedures.AddMessage(input, myMembershipId);
            NoticeService.PushNotice(receiver, "<b>" + me.GetFullName() + " has sent you a new message.</b>", NoticeService.PushType__PlayerMessage);
            TempData["Result"] = "Your message has been sent.";

            if (me.Mobility != "full")
            {
                ItemProcedures.UpdateSouledItem(me.FirstName, me.LastName);
            }

            return RedirectToAction("Index");
        }

        // GET: /Messages/CovenantWideMessage
        [HttpGet]
        [Authorize]
        public ActionResult CovenantWideMessage()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot send out mass messages to your members.";
                return RedirectToAction("MyCovenant", "Covenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can send out mass messages.";
                return RedirectToAction("MyCovenant", "Covenant");
            }

            return View();
        }

        // POST: /Messages/SendCovenantWideMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult SendCovenantWideMessage(string Message)
        {

            if (Message.Length > 1000)
            {
                TempData["Error"] = "Your message is too long.  There is a 1000 character limit.";
                return RedirectToAction("MyCovenant", "Covenant");
            }
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot send out mass messages to your members.";
                return RedirectToAction("MyCovenant", "Covenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can send out mass messages.";
                return RedirectToAction("MyCovenant", "Covenant");
            }

            MessageProcedures.SendCovenantWideMessage(me, Message);

            TempData["Result"] = "Message sent out!";
            return RedirectToAction("MyCovenant", "Covenant");

        }
    }
}
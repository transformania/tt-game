using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Queries;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Messages.Commands;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Queries;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Web.Services;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class MessagesController : Controller
    {
        // GET: /Messages
        [HttpGet]
        public virtual ActionResult Index(int offset = 0)
        {

            var myMembershipId = User.Identity.GetUserId();

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            DomainRegistry.Repository.Execute(new DeletePlayerExpiredMessages { OwnerId = me.Id });

            var output = MessageProcedures.GetPlayerMessages(me, offset);

            output.InboxSize = 150;

            // if you are inanimate and are being worn, grab the data on who is wearing you

            if (me.Mobility == PvPStatics.MobilityInanimate)
            {
                var personWearingMe = ItemProcedures.BeingWornBy(me);
                if (personWearingMe != null)
                {
                    output.WearerId = personWearingMe.Player.Id;
                    output.WearerBotId = personWearingMe.Player.BotId;
                    output.WearerName = personWearingMe.Player.GetFullName();
                }
            }

            output.FriendOnlyMessages = me.FriendOnlyMessages;

            var isDonator = me.DonatorGetsMessagesRewards();

            ViewBag.IsDonator = isDonator;

            if (isDonator)
            {
                output.InboxSize = 500;
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Messages.Views.Messages, output);
        }

        // POST: /Message/DeleteMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteMessage(bool deleteAll, int messageId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            if (deleteAll)
            {
                DomainRegistry.Repository.Execute(new DeleteAllMessagesOwnedByPlayer() { OwnerId = me.Id });
                TempData["Result"] = "Your messages have been deleted.";
                return RedirectToAction(MVC.Messages.Index());
            }

            try
            {
                DomainRegistry.Repository.Execute(new DeleteMessage { MessageId = messageId, OwnerId = me.Id });
            }
            catch (DomainException)
            {
                TempData["Error"] = "You can't delete this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction(MVC.Messages.Index());
            }

            return RedirectToAction(MVC.Messages.Index());
        }

        // GET: /Message/ReadMessage/{messageId}
        [HttpGet]
        public virtual ActionResult ReadMessage(int messageId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            MessageDetail message;

            try
            {
                message = DomainRegistry.Repository.FindSingle(new GetMessage { MessageId = messageId, OwnerId = me.Id });
            }
            catch (DomainException)
            {
                TempData["Error"] = "You can't read this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction(MVC.Messages.Index());
            }

            DomainRegistry.Repository.Execute(new MarkAsRead { MessageId = message.MessageId, ReadStatus = MessageStatics.Read, OwnerId = me.Id });

            return View(MVC.Messages.Views.ReadMessage, message);
        }

        public virtual ActionResult ReadConversation(int messageId, bool reversed = false)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (!DomainRegistry.Repository.FindSingle(new PlayerOwnsMessage { MessageId = messageId, OwnerId = me.Id }))
            {
                TempData["Error"] = "You can't read this conversation.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction(MVC.Messages.Index());
            }

            var message = DomainRegistry.Repository.FindSingle(new GetMessage { MessageId = messageId, OwnerId = me.Id });

            IEnumerable<MessageDetail> messages;

            try
            {
                messages =
                    DomainRegistry.Repository.Find(new GetMessagesInConversation
                    {
                        conversationId = message.ConversationId
                    });

                if (reversed)
                {
                    messages = messages.Reverse();
                }

                return PartialView(MVC.Messages.Views.partial.Conversation, messages);
            }
            catch
            {
                TempData["Error"] = "No conversation found";
                return RedirectToAction(MVC.Messages.Index());
            }


        }

        // POST: /Messages/MarkAsUnread
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult MarkReadStatus(int messageId, int readStatus)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            try
            {
                DomainRegistry.Repository.Execute(new MarkAsRead { MessageId = messageId, ReadStatus = readStatus, OwnerId = me.Id });
            }
            catch (DomainException)
            {
                TempData["Error"] = "You can't mark this message as unread or unread.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction(MVC.Messages.Index());
            }

            if (readStatus == MessageStatics.ReadAndMarkedAsUnread)
            {
                TempData["Result"] = "Message marked as unread.";
            }
            else
            {
                TempData["Result"] = "Message marked as read.";
            }

            return RedirectToAction(MVC.Messages.Index());
        }

        // GET: /Messages/Write
        [HttpGet]
        public virtual ActionResult Write(int playerId, int responseTo = -1)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var sendingTo = PlayerProcedures.GetPlayer(playerId);
            var output = new MessageSubmitViewModel();

            output.SenderId = me.Id;
            output.ReceiverId = playerId;
            output.responseToId = responseTo;
            output.SendingToName = sendingTo.GetFullName();

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            if (TempData["MessageText"] != null && TempData["ErrorMessage"] != null)
            {
                // preserves what the user typed if coming from SendMessage
                output.MessageText = TempData["MessageText"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            if (responseTo != -1)
            {
                try
                {
                    var msgRepliedTo = DomainRegistry.Repository.FindSingle(new GetMessage { MessageId = responseTo, OwnerId = me.Id });
                    output.RespondingToMsg = msgRepliedTo.MessageText;

                }
                catch (DomainException)
                {
                    TempData["Result"] = "You can't reply to this message since the original was not sent to you.";
                    return RedirectToAction(MVC.Messages.Index());
                }

            }

            return View(MVC.Messages.Views.Write, output);
        }

        // POST: /Messages/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SendMessage(MessageSubmitViewModel input)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var receiver = PlayerProcedures.GetPlayer(input.ReceiverId);

            // assert player is not banned from chat
            if (me.IsBannedFromGlobalChat)
            {
                TempData["Error"] = "You have been banned and cannot send any messages for the following reason: " + me.ChatLockoutMessage;
                TempData["SubError"] = "If you feel this is in error or wish to make an appeal you may do so via Discord.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert no blacklist exists
            if (BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, receiver, "message"))
            {
                TempData["Error"] = "This player has blacklisted you or is on your own blacklist.";
                TempData["SubError"] = "You cannot send messages to players who have blacklisted you.  Remove them from your blacklist or ask them to remove you from theirs.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if either user has friend only messaging on, assert that the players are friends
            if ((me.FriendOnlyMessages || receiver.FriendOnlyMessages) && !MessageProcedures.PlayersAreFriends(me, receiver))
            {
                TempData["Error"] = "You and/or the recipient has friend only messaging turn ON and you are not friends with this user";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (EffectProcedures.PlayerHasActiveEffect(me, CharacterPrankProcedures.HUSHED_EFFECT))
            {
                TempData["ErrorMessage"] = "You have been hushed and cannot currently send a message.  Try again once the effect has worn off.";
                TempData["MessageText"] = input.MessageText;
                return RedirectToAction(MVC.Messages.Write(input.ReceiverId, input.responseToId));
            }

            if (input.MessageText.IsNullOrEmpty())
            {
                TempData["ErrorMessage"] = "You need to write something to send to this person.";
                TempData["MessageText"] = input.MessageText;
                return RedirectToAction(MVC.Messages.Write(input.ReceiverId, input.responseToId));

            }

            if (input.MessageText.Length > 1000)
            {
                TempData["ErrorMessage"] = "Your message is too long.";
                TempData["MessageText"] = input.MessageText;
                return RedirectToAction(MVC.Messages.Write(input.ReceiverId, input.responseToId));
            }

            MessageDetail repliedMsg = null;
            if (input.responseToId > 0)
            {
                repliedMsg = DomainRegistry.Repository.FindSingle(new GetMessage { MessageId = input.responseToId, OwnerId = me.Id });
            }


            DomainRegistry.Repository.Execute(new CreateMessage
            {
                ReceiverId = receiver.Id,
                SenderId = me.Id,
                Text = input.MessageText,
                ReplyingToThisMessage = repliedMsg
            });

            NoticeService.PushNotice(receiver, "<b>" + me.GetFullName() + " has sent you a new message.</b>", NoticeService.PushType__PlayerMessage);

            TempData["Result"] = "Your message has been sent.";

            if (me.Mobility != PvPStatics.MobilityFull)
            {
                ItemProcedures.UpdateSouledItem(me);
            }

            return RedirectToAction(MVC.Messages.Index());
        }

        // GET: /Messages/CovenantWideMessage
        [HttpGet]
        public virtual ActionResult CovenantWideMessage()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot send out mass messages to your members.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can send out mass messages.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            return View(MVC.Messages.Views.CovenantWideMessage);
        }

        // POST: /Messages/SendCovenantWideMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SendCovenantWideMessage(string Message)
        {

            if (Message.Length > 1000)
            {
                TempData["Error"] = "Your message is too long.  There is a 1000 character limit.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot send out mass messages to your members.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can send out mass messages.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            MessageProcedures.SendCovenantWideMessage(me, Message);

            TempData["Result"] = "Message sent out!";
            return RedirectToAction(MVC.Covenant.MyCovenant());

        }

        public virtual ActionResult MarkAsAbusive(int id, Guid? conversationId)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.Execute(new MarkAsAbusive { MessageId = id, OwnerId = me.Id, ConversationId = conversationId });
                TempData["Result"] = "This message has been marked as abusive and a moderator will soon review it.";
                return RedirectToAction(MVC.Messages.Index());
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.Messages.Index());
            }

        }

        public virtual ActionResult MarkAsAbusivePreview(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            MessageDetail message;

            try
            {
                message = DomainRegistry.Repository.FindSingle(new GetMessage { MessageId = id, OwnerId = me.Id });
            }
            catch (DomainException)
            {
                TempData["Error"] = "You can't read this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction(MVC.Messages.Index());
            }

            return View(MVC.Messages.Views.MarkAsAbusivePreview, message);
        }
    }
}
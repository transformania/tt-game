using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class MessageProcedures
    {
        public static MessageCountDataViewModel GetMessageCountData(Player player)
        {
            IMessageRepository messageRepo = new EFMessageRepository();

            IEnumerable<Message> myMessages = messageRepo.Messages.Where(m => m.ReceiverId == player.Id);
            MessageCountDataViewModel output = new MessageCountDataViewModel
            {
                NewMessagesCount = myMessages.Where(m => m.IsRead == false).Count(),
                TotalMessagesCount = myMessages.Count()
            };

            return output;

        }

        public static int GetPlayerUnreadMessageCount(Player player)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            return messageRepo.Messages.Where(m => m.ReceiverId == player.Id && m.IsRead == false).Count();
        }

        public static MessageViewModel GetMessage(int messageId)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Message dbMessage = messageRepo.Messages.FirstOrDefault(m => m.Id == messageId);
            Player sender = playerRepo.Players.FirstOrDefault(p => p.Id == dbMessage.SenderId);
            Player receiver = playerRepo.Players.FirstOrDefault(p => p.Id == dbMessage.ReceiverId);

            MessageViewModel output = new MessageViewModel();
            output.dbMessage = dbMessage;

            if (sender == null)
            {
                output.SenderName = "(inanimate)";
            }
            else
            {
                output.SenderName = sender.GetFullName();
            }

            if (receiver == null)
            {
                output.SentToName = "(inanimate)";
            }
            else
            {
                output.SentToName = receiver.FirstName + " " + receiver.LastName;
            }

            return output;
        }

        public static MessageViewModel GetMessageAndMarkAsRead(int messageId)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Message dbMessage = messageRepo.Messages.FirstOrDefault(m => m.Id == messageId);

            dbMessage.IsRead = true;
            dbMessage.ReadStatus = 1;
            messageRepo.SaveMessage(dbMessage);


            Player sender = playerRepo.Players.FirstOrDefault(p => p.Id == dbMessage.SenderId);

            MessageViewModel output = new MessageViewModel();
            output.dbMessage = dbMessage;

            if (sender == null)
            {
                output.SenderName = "(inanimate)";
            }
            else
            {
                output.SenderName = sender.FirstName + " " + sender.LastName;
            }

            return output;
        }

        public static void MarkMessageAsUnread(int messageId)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            Message dbMessage = messageRepo.Messages.FirstOrDefault(m => m.Id == messageId);
            dbMessage.ReadStatus = 2;
            messageRepo.SaveMessage(dbMessage);
        }

        public static MessageBag GetPlayerMessages(Player player, int offset)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            int inboxLimit = 150;

            if (DonatorProcedures.DonatorGetsMessagesRewards(player) == true)
            {
                inboxLimit = 500;
            }

            List<Message> mydbMessagesALL = messageRepo.Messages.Where(m => m.ReceiverId == player.Id).OrderByDescending(m => m.Timestamp).ToList();
            List<Message> mydbMessagesTODELETE = mydbMessagesALL.Skip(inboxLimit).ToList();

            // paginate down to first N results
            Paginator paginator = new Paginator(mydbMessagesALL.Count(), 25);
            paginator.CurrentPage = offset + 1;
            mydbMessagesALL = mydbMessagesALL.Skip(paginator.GetSkipCount()).Take(paginator.PageSize).ToList();

            List < Message> mydbMessages = mydbMessagesALL.Take(inboxLimit).ToList();

            List<Message> mydbSentMessages = messageRepo.Messages.Where(m => m.SenderId == player.Id).OrderByDescending(m => m.Timestamp).Take(20).ToList();

            List<MessageViewModel> messageViewModelList = new List<MessageViewModel>();
            List<MessageViewModel> messageSentViewModelList = new List<MessageViewModel>();

            foreach (Message message in mydbMessages)
            {
                messageViewModelList.Add(GetMessage(message.Id));
            }

            foreach (Message message in mydbSentMessages)
            {
                messageSentViewModelList.Add(GetMessage(message.Id));
            }

            MessageBag output = new MessageBag
            {
                Messages = messageViewModelList,
                SentMessages = messageSentViewModelList,
            };

            foreach (Message message in mydbMessagesTODELETE)
            {
                messageRepo.DeleteMessage(message.Id);
            }

            output.Paginator = paginator;

            return output;


        }

        public static void DeleteMessage(int messageId)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            messageRepo.DeleteMessage(messageId);
        }

        public static bool PlayerOwnsMessage(int messageId, string membershipId)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(membershipId);
            IMessageRepository messageRepo = new EFMessageRepository();
            Message message = messageRepo.Messages.FirstOrDefault(m => m.Id == messageId && m.ReceiverId == me.Id);
            if (message != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DeleteAllMessages(string membershipId)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(membershipId);
            IMessageRepository messageRepo = new EFMessageRepository();

            List<Message> myMessagesToDelete = messageRepo.Messages.Where(m => m.ReceiverId == me.Id).ToList();

            foreach (Message message in myMessagesToDelete)
            {
                messageRepo.DeleteMessage(message.Id);
            }


        }

        public static void AddMessage(MessageSubmitViewModel messageVM, string membershipId)
        {
            Message message = new Message();

            message.MessageText = messageVM.MessageText;
            message.SenderId = messageVM.SenderId;
            message.ReceiverId = messageVM.ReceiverId;

            AddMessage(message, membershipId);
        }

        public static void AddMessage(Message message, string membershipId)
        {
            Player sender = PlayerProcedures.GetPlayerFromMembership(membershipId);
            Player receiver = PlayerProcedures.GetPlayer(message.ReceiverId);
            IMessageRepository messageRepo = new EFMessageRepository();

            message.Timestamp = DateTime.UtcNow;
            message.SenderId = sender.Id;
            message.IsRead = false;
            message.ReadStatus = 0;

            if (DonatorProcedures.EitherPlayersAreDonatorsOfTier(sender, receiver, 3) == true)
            {
                message.DoNotRecycleMe = true;
            }
            else
            {
                message.DoNotRecycleMe = false;
            }

            messageRepo.SaveMessage(message);

        }

        public static void SendCovenantWideMessage(Player covLeader, string input)
        {
            // get the players in the leader's covenant
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var query = from p in playerRepo.Players
                        where p.Covenant == covLeader.Covenant && p.Id != covLeader.Id
                        select p;

            foreach (var p in query)
            {
                AddMessage(new Message
                {
                    ReceiverId = p.Id,
                    SenderId = covLeader.Id,
                    IsRead = false,
                    ReadStatus = 0,
                    MessageText = input,
                    Timestamp = DateTime.UtcNow,
                }, covLeader.MembershipId);
            }
        }


    }
}
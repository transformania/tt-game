using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Messages;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Queries.Messages;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class MessageProcedures
    {

        public static int GetPlayerUnreadMessageCount(Player player)
        {
            IMessageRepository messageRepo = new EFMessageRepository();
            return messageRepo.Messages.Where(m => m.ReceiverId == player.Id && !m.IsRead).Count();
        }

        public static MessageBag GetPlayerMessages(Player player, int offset)
        {
           
            int inboxLimit = 150;

            if (DonatorProcedures.DonatorGetsMessagesRewards(player))
            {
                inboxLimit = 500;
            }

            var receivedMessages = DomainRegistry.Repository.Find(new GetPlayerReceivedMessages {ReceiverId = player.Id});
            var sentMessages = DomainRegistry.Repository.Find(new GetPlayerSentMessages {SenderId = player.Id, Take = 20});

            var receivedMessagesToDelete = receivedMessages.Skip(inboxLimit);

            // paginate down to first N results
            Paginator paginator = new Paginator(receivedMessages.Count(), 25);
            paginator.CurrentPage = offset + 1;
            receivedMessages = receivedMessages.Skip(paginator.GetSkipCount()).Take(paginator.PageSize).ToList();

            MessageBag output = new MessageBag
            {
                Messages = receivedMessages,
                SentMessages = sentMessages
            };

            foreach (var message in receivedMessagesToDelete)
            {
                DomainRegistry.Repository.Execute(new DeleteMessage {MessageId = message.Id});
            }

            output.Paginator = paginator;

            return output;
        }

        public static void AddMessage(Message message, string membershipId)
        {
            Player sender = PlayerProcedures.GetPlayerFromMembership(membershipId);
            Player receiver = PlayerProcedures.GetPlayer(message.ReceiverId);
            IMessageRepository messageRepo = new EFMessageRepository();

            message.Timestamp = DateTime.UtcNow;
            message.SenderId = sender.Id;
            message.IsRead = false;
            message.ReadStatus = MessageStatics.Unread;

            if (DonatorProcedures.EitherPlayersAreDonatorsOfTier(sender, receiver, 3))
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
                    ReadStatus = MessageStatics.Unread,
                    MessageText = input,
                    Timestamp = DateTime.UtcNow,
                }, covLeader.MembershipId);
            }
        }


    }
}
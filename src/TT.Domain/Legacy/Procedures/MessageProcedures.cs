using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Messages.Commands;
using TT.Domain.Messages.Queries;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class MessageProcedures
    {

        public static MessageBag GetPlayerMessages(Player player, int offset)
        {

            var inboxLimit = player.DonatorGetsMessagesRewards() ? 500 : 150;

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
                DomainRegistry.Repository.Execute(new DeleteMessage {MessageId = message.Id, OwnerId = player.Id});
            }

            output.Paginator = paginator;

            return output;
        }

        public static void SendCovenantWideMessage(Player covLeader, string input)
        {
            // get the players in the leader's covenant
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var query = from p in playerRepo.Players
                        where p.Covenant == covLeader.Covenant && p.Id != covLeader.Id && p.BotId == AIStatics.ActivePlayerBotId
                        select p;

            foreach (var p in query)
            {
                DomainRegistry.Repository.Execute(new CreateMessage
                {
                    ReceiverId = p.Id,
                    SenderId = covLeader.Id,
                    Text = input,
                    ReplyingToThisMessage = null
                });
            }
        }


    }
}
using System;
using NSubstitute;
using NUnit.Framework;
using tfgame.dbModels;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Commands.Player;
using tfgame.dbModels.Models;
using tfgame.Services;

namespace tfgame.Tests.Services
{
    [TestFixture]
    public class TestChatService
    {
        [SetUp]
        public void SetUp()
        {
            DomainRegistry.Root = Substitute.For<IRoot>();
        }
        
        [Test]
        public void Should_update_player_online_activity_timestamp_if_overdue()
        {
            var player = new Player_VM 
            { 
                Id = 12345, 
                OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-3)
            };

            var chatService = new ChatService();
            chatService.MarkOnlineActivityTimestamp(player);

            DomainRegistry.Root.Received().Execute(Arg.Is<MarkOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }

        [Test]
        public void Should_not_update_player_online_activity_timestamp_if_not_overdue()
        {
            var player = new Player_VM
            {
                Id = 12345,
                OnlineActivityTimestamp = DateTime.UtcNow.AddMinutes(-1)
            };

            var chatService = new ChatService();
            chatService.MarkOnlineActivityTimestamp(player);

            DomainRegistry.Root.DidNotReceive().Execute(Arg.Is<MarkOnlineActivityTimestamp>(cmd => cmd.Player.Id == player.Id));
        }
    }
}
using System;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Services;

namespace TT.Tests.Domain.Services
{
    [TestFixture]
    public class AttackNotificationBrokerTests
    {
        [Test]
        public void Should_raise_event_on_notify()
        {
            const int playerId = 1;
            const string message = "You've been attacked";

            var service = new AttackNotificationBroker();
            using (var monitoredService = service.Monitor())
            {
                service.Notify(playerId, message);

                monitoredService
                    .Should()
                    .Raise("NotificationRaised")
                    .WithSender(service)
                    .WithArgs<NotificationRaisedEventArgs>(args =>
                        args.PlayerId == playerId && args.Message == message);
            }
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_not_allow_invalid_playerId(int id)
        {
            const string message = "You've been attacked";

            var service = new AttackNotificationBroker();

            Action action = () => service.Notify(id, message);

            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("Cannot raise attack notification. Parameter name: playerId");
        }

        [TestCase("")]
        [TestCase("")]
        [TestCase(null)]
        public void Should_not_allow_invalid_playerId(string message)
        {
            const int playerId = 1;

            var service = new AttackNotificationBroker();

            Action action = () => service.Notify(playerId, message);

            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("Cannot raise attack notification. Parameter name: message");
        }
    }
}
using System;
using NUnit.Framework;
using TT.Domain.Services;

namespace TT.Tests.Domain.Services
{
    [TestFixture]
    public class AttackNotificationBrokerTests
    {
        [Test]
        [Ignore("TODO: Rewrite for use without FluentAssertions")]
        public void Should_raise_event_on_notify()
        {
            /*const int playerId = 1;
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
            }*/
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_not_allow_invalid_playerId(int id)
        {
            const string message = "You've been attacked";

            var service = new AttackNotificationBroker();

            Assert.That(() => service.Notify(id, message),
                Throws.TypeOf<ArgumentException>().With.Message
                    .Matches("Cannot raise attack notification\\. [\r\n]+Parameter name: playerId"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Should_not_allow_invalid_message(string message)
        {
            const int playerId = 1;

            var service = new AttackNotificationBroker();

            Assert.That(() => service.Notify(playerId, message),
                Throws.TypeOf<ArgumentException>().With.Message
                    .Matches("Cannot raise attack notification\\. [\r\n]+Parameter name: message"));
        }
    }
}
using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Messages;
using TT.Domain.Entities.Messages;
using TT.Domain.Entities.Players;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Commands.Messages
{

    public class CreateMessagesTests : TestBase
    {
        private Player playerBob;
        private Player playerSam;

        [SetUp]
        public void Init()
        {
            playerBob =  new PlayerBuilder()
                .With(p => p.User, new UserBuilder().With(u => u.Id, "guid").BuildAndSave())
                .With(p => p.Id, 13)
                .With(p => p.FirstName, "Bob")
                .With(p => p.LastName, "Bobbyson")
                .BuildAndSave();

            playerSam = new PlayerBuilder()
                .With(p => p.User, new UserBuilder().With(u => u.Id, "guid").BuildAndSave())
                .With(p => p.Id, 17)
                .With(p => p.FirstName, "Sam")
                .With(p => p.LastName, "Samrade")
                .BuildAndSave();
        }


        [Test]
        public void Should_create_new_message()
        {

            var cmd = new CreateMessage();
            cmd.SenderId = playerBob.Id;
            cmd.ReceiverId = playerSam.Id;
            cmd.Text = "Message!";

            Repository.Execute(cmd);

            DataContext.AsQueryable<Message>().Count(p =>
                p.Sender.Id == 13 &&
                p.Receiver.Id == 17 && 
                p.MessageText == "Message!")
            .Should().Be(1);
        }

        [Test]
        public void Should_throw_exception_if_sender_is_not_found()
        {

            var cmd = new CreateMessage();
            cmd.SenderId = 34745;
            cmd.ReceiverId = playerSam.Id;
            cmd.Text = "Message!";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Sending player with Id 34745 could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_receiver_is_not_found()
        {

            var cmd = new CreateMessage();
            cmd.SenderId = playerBob.Id;
            cmd.ReceiverId = 34745;
            cmd.Text = "Message!";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Receiving player with Id 34745 could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_text_is_blank()
        {

            var cmd = new CreateMessage();
            cmd.SenderId = playerBob.Id;
            cmd.ReceiverId = 34745;
            cmd.Text = "";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Text must not be empty or null"));
        }

    }
}

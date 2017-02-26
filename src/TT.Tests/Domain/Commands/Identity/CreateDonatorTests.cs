using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Domain.Commands.Identity
{
    [TestFixture]
    public class CreateDonatorTests : TestBase
    {
        [Test]
        public void can_create_donator_entry()
        {

            var user = new UserBuilder()
                .BuildAndSave();

            var cmd = new CreateDonator
            {
                UserId = user.Id,
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3,
                SpecialNotes = "good boy!"
            };
            DomainRegistry.Repository.Execute(cmd);

            DataContext.AsQueryable<Donator>().Count(d =>
                d.PatreonName == "Bob" &&
                d.ActualDonationAmount == 9 &&
                d.Tier == 3 &&
                d.SpecialNotes == "good boy!")
            .Should().Be(1);

        }

        [Test]
        public void should_require_user()
        {
            var cmd = new CreateDonator
            {
                UserId = null,
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3
            };
          
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("userId is required");
        }

        [Test]
        public void should_throw_error_if_user_not_found()
        {
            var cmd = new CreateDonator
            {
                UserId = "fakeuser",
                ActualDonationAmount = 9,
                PatreonName = "Bob",
                Tier = 3
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("User 'fakeuser' could not be found");
        }
    }
}

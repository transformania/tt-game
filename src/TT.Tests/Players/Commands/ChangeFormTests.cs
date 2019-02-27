using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{

    [TestFixture]
    public class ChangeFormTests : TestBase
    {

        private FormSource newFormSource;
        private FormSource oldFormSource;

        [SetUp]
        public override void SetUp()
        {

            base.SetUp();

            newFormSource = new FormSourceBuilder()
                .With(n => n.Id, 3)
                .With(p => p.FriendlyName, "werewolf")
                .With(p => p.Gender, PvPStatics.GenderFemale)
                .BuildAndSave();

            oldFormSource = new FormSourceBuilder()
                .With(n => n.Id, 1)
                .BuildAndSave();
        }

        [Test]
        public void Should_change_player_form()
        {

            new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.FormSource, oldFormSource)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var cmd = new ChangeForm { PlayerId = 23, FormId = newFormSource.Id };

            DomainRegistry.Repository.Execute(cmd);

            DataContext.AsQueryable<Player>().Count(p =>
               p.Id == 23 &&
               p.Gender == PvPStatics.GenderFemale &&
               p.FormSource.Id == 3 &&
               p.FormSource.FriendlyName == "werewolf")
           .Should().Be(1);

        }

        [Test]
        public void should_set_location_to_owner_location_if_owned_item_or_pet()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 249)
                .With(p => p.FormSource, oldFormSource)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .With(i => i.Item, new ItemBuilder()
                    .With(i => i.Id, 123456)
                    .With(i => i.Owner, new PlayerBuilder()
                        .With(p => p.Location, "rimworld")
                        .BuildAndSave())
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new ChangeForm { PlayerId = player.Id, FormId = newFormSource.Id };

            DomainRegistry.Repository.Execute(cmd);

            var playerLoaded = DataContext.AsQueryable<Player>().SingleOrDefault(p => p.Id == player.Id);
            playerLoaded.Location.Should().Be("rimworld");
        }

        [Test]
        public void should_set_location_to_item_location_if_unowned_item_or_pet()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 249)
                .With(p => p.FormSource, oldFormSource)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .With(i => i.Item, new ItemBuilder()
                    .With(i => i.dbLocationName, "mibbitworld")
                    .With(i => i.Id, 123456)
                    .With(i => i.Owner, null)
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new ChangeForm { PlayerId = player.Id, FormId = newFormSource.Id };

            DomainRegistry.Repository.Execute(cmd);

            var playerLoaded = DataContext.AsQueryable<Player>().SingleOrDefault(p => p.Id == player.Id);
            playerLoaded.Location.Should().Be("mibbitworld");
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeForm { PlayerId = 23, FormId = oldFormSource.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Player with ID 23 could not be found");
        }

        [Test]
        public void Should_throw_exception_if_form_source_not_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 23)
                .BuildAndSave();

            var cmd = new ChangeForm { PlayerId = 23, FormId = -123 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("FormSource with ID -123 could not be found");
        }



    }
}

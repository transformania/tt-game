﻿using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Players;
using TT.Domain.Entities.Players;
using TT.Domain.Statics;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Players
{
    public class ChangeFormTests : TestBase
    {
        [Test]
        public void Should_change_player_form()
        {
            var form = new FormSourceBuilder()
                .With(n => n.Id, 1)
                .BuildAndSave();

             new FormSourceBuilder()
                .With(n => n.Id, 3)
                .With(p => p.FriendlyName, "werewolf")
                .With(p => p.Gender, PvPStatics.GenderFemale)
                .With(p => p.dbName, "dbWerewolf")
                .BuildAndSave();

            new PlayerBuilder()
                .With(p => p.Id, 23)
                .With(p => p.FormSource, form)
                .With(p => p.Form, "catgirl")
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var cmd = new ChangeForm { PlayerId = 23, FormId = 3 };

            DomainRegistry.Repository.Execute(cmd);

            DataContext.AsQueryable<Player>().Count(p =>
               p.Id == 23 &&
               p.Gender == PvPStatics.GenderFemale &&
               p.FormSource.Id == 3 &&
               p.FormSource.FriendlyName == "werewolf" &&
               p.Form == "dbWerewolf")
           .Should().Be(1);

        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeForm { PlayerId = 23, FormId = 3 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Player with ID 23 could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_form_source_not_found()
        {

            new PlayerBuilder()
                .With(p => p.Id, 23)
                .BuildAndSave();

            var cmd = new ChangeForm { PlayerId = 23, FormId = 3 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("FormSource with ID 3 could not be found"));
        }



    }
}
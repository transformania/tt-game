using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Effects.Entities;
using TT.Domain.Entities.Skills;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Effects;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.Skills;

namespace TT.Tests.Players.Commands
{
    [TestFixture]
    public class DeletePlayerTests : TestBase
    {

        [Test]
        public void should_delete_player_and_their_stuff()
        {

            var owner = new PlayerBuilder()
                 .With(p => p.Id, 23)
                 .BuildAndSave();

            new SkillBuilder()
                .With(s => s.Id, 1)
                .With(s => s.Owner, owner)
                .BuildAndSave();

            new EffectBuilder()
                .With(s => s.Id, 2)
                .With(s => s.Owner, owner)
                .BuildAndSave();

            new PlayerLogBuilder()
                .With(s => s.Id, 3)
                .With(s => s.Owner, owner)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new DeletePlayer {PlayerId = 23});

            DataContext.AsQueryable<Player>().Count(p =>
               p.Id == 23)
             .Should().Be(0);

            DataContext.AsQueryable<Skill>().Count(p =>
               p.Id == 1)
             .Should().Be(0);

            DataContext.AsQueryable<Effect>().Count(p =>
              p.Id == 2)
            .Should().Be(0);

            DataContext.AsQueryable<PlayerLog>().Count(p =>
              p.Id == 3)
            .Should().Be(0);

        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new DeletePlayer {PlayerId = 23};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with ID 23 was not found");
        }
    }
}

using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Effects.Commands;
using TT.Domain.Effects.Entities;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Effects;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Effects
{
    [TestFixture]
    public class CreateEffectTests : TestBase
    {
        public void can_create_effect()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

             new EffectSourceBuilder()
                .With(e => e.Id, 77)
                .BuildAndSave();

            var cmd = new CreateEffect { OwnerId = 100, EffectSourceId = 100 };
            DomainRegistry.Repository.Execute(cmd);

            DataContext.AsQueryable<Effect>().Count(p =>
                p.Owner.Id == 100 &&
                p.EffectSource.Id == 77)
            .Should().Be(1);

        }

        [Test]
        public void should_throw_error_if_effect_source_not_found()
        {
            var cmd = new CreateEffect() { OwnerId = 100, EffectSourceId = 55 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Effect Source with Id 55 could not be found");
        }

        [Test]
        public void should_throw_error_if_player_not_found()
        {
            new EffectSourceBuilder()
                .With(ss => ss.Id, 55)
                .BuildAndSave();

            var cmd = new CreateEffect() { OwnerId = 100, EffectSourceId = 55 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with Id 100 could not be found");
        }

    }
}

using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Effects.Commands;
using TT.Domain.Effects.Entities;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Effects;
using TT.Tests.Builders.Players;

namespace TT.Tests.Effects.Commands
{
    [TestFixture]
    public class CreateEffectTests : TestBase
    {
        [Test]
        public void can_create_effect()
        {
            new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

             new EffectSourceBuilder()
                .With(e => e.Id, 77)
                .BuildAndSave();

            var cmd = new CreateEffect { OwnerId = 100, EffectSourceId = 77 };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Effect>().Where(p =>
                p.Owner.Id == 100 &&
                p.EffectSource.Id == 77), Has.Exactly(1).Items);
        }

        [Test]
        public void should_throw_error_if_effect_source_not_found()
        {
            var cmd = new CreateEffect { OwnerId = 100, EffectSourceId = 55 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Effect Source with Id 55 could not be found"));
        }

        [Test]
        public void should_throw_error_if_player_not_found()
        {
            new EffectSourceBuilder()
                .With(ss => ss.Id, 55)
                .BuildAndSave();

            var cmd = new CreateEffect { OwnerId = 100, EffectSourceId = 55 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with Id 100 could not be found"));
        }

    }
}

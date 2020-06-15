using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.TFEnergies.Commands;
using TT.Domain.TFEnergies.Entities;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Players;

namespace TT.Tests.Combat.Commands
{

    [TestFixture]
    public class CreateTFEnergyTests : TestBase
    {

        [Test]
        public void can_create_tf_energy()
        {
            var player = new PlayerBuilder()
                .With(i => i.Id, 50)
                .BuildAndSave();

            var caster = new PlayerBuilder()
                .With(i => i.Id, 987)
                .BuildAndSave();

            var form = new FormSourceBuilder()
                .With(i => i.Id, 13)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new CreateTFEnergy
            {
                PlayerId = player.Id,
                Amount = 100,
                FormSourceId = form.Id,
                CasterId = caster.Id
            });

            Assert.That(DataContext.AsQueryable<TFEnergy>().Where(t =>
                t.Owner.Id == 50 &&
                t.Caster.Id == 987 &&
                t.Amount == 100 &&
                t.FormSource.Id == 13), Has.Exactly(1).Items);
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {

            var cmd = new CreateTFEnergy
            {
                PlayerId = 13
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID 13 could not be found"));
        }

    }
}

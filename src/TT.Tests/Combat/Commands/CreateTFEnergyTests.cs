using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Combat.Commands;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Exceptions;
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
                .With(f => f.dbName, "kittygirl")
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new CreateTFEnergy
            {
                PlayerId = player.Id,
                Amount = 100,
                FormSourceId = form.Id,
                FormName = form.dbName,
                CasterId = caster.Id
            });

            DataContext.AsQueryable<TFEnergy>().Count(t =>
                t.Owner.Id == 50 &&
                t.Caster.Id == 987 &&
                t.Amount == 100 &&
                t.FormSource.Id == 13)
            .Should().Be(1);

        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {

            var cmd = new CreateTFEnergy
            {
                PlayerId = 13
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Player with ID 13 could not be found");
        }

    }
}

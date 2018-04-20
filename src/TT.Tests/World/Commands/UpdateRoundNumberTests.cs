using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.World.Commands;
using TT.Tests.Builders.Game;

namespace TT.Tests.World.Commands
{
    public class UpdateRoundNumberTests : TestBase
    {
        [Test]
        public void can_update_round_number()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "old round")
                .With(i => i.TurnNumber, 0)
                .With(i => i.ChaosMode, true)
                .BuildAndSave();


            DomainRegistry.Repository.Execute(new UpdateRoundNumber { RoundNumber = "new round"});

            var world = DataContext.AsQueryable<TT.Domain.World.Entities.World>().First();

            world.RoundNumber.Should().Be("new round");
        }

        [Test]
        public void should_throw_exception_if_round_number_incorrect()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "old round")
                .With(i => i.TurnNumber, 3)
                .With(i => i.ChaosMode, true)
                .BuildAndSave();

            Action action = () => Repository.Execute(new UpdateRoundNumber { RoundNumber = "new round" });
            action.Should().ThrowExactly<DomainException>().WithMessage("Round renaming can only be done at turn 0 or the maximum round turn.");
        }

        [Test]
        public void should_throw_exception_if_not_in_chaos()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "old round")
                .With(i => i.TurnNumber, 0)
                .With(i => i.ChaosMode, false)
                .BuildAndSave();

            Action action = () => Repository.Execute(new UpdateRoundNumber { RoundNumber = "new round" });
            action.Should().ThrowExactly<DomainException>().WithMessage("Round renaming can only be done in chaos mode.");
        }

        [Test]
        public void should_throw_exception_if_round_number_null()
        {
            new WorldBuilder()
                .With(i => i.RoundNumber, "old round")
                .With(i => i.TurnNumber, 0)
                .With(i => i.ChaosMode, true)
                .BuildAndSave();

            Action action = () => Repository.Execute(new UpdateRoundNumber { });
            action.Should().ThrowExactly<DomainException>().WithMessage("Round Number must be set!");
        }

    }
}

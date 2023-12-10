using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;

namespace TT.Tests.Identity.Commands
{
    [TestFixture]
    public class UpdateLastHolidaySpiritInteractionTests : TestBase
    {

        [Test]
        public void can_update_last_interaction()
        {
            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "100").BuildAndSave())
                .With(p => p.LastHolidaySpiritInteraction, 5)
                .BuildAndSave();

            var cmd = new UpdateLastHolidaySpiritInteraction { UserId = "100", LastHolidaySpiritInteraction = 50 };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "100").LastHolidaySpiritInteraction, Is.EqualTo(50));
        }


        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "100").BuildAndSave())
                .With(p => p.LastHolidaySpiritInteraction, 5)
                .BuildAndSave();

            var cmd = new UpdateLastHolidaySpiritInteraction { UserId = "200", LastHolidaySpiritInteraction = 50 };
            Assert.That(()=> Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id '200' could not be found"));
        }

    }
}

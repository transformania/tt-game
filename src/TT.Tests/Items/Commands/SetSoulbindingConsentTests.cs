using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    public class SetSoulbindingConsentTests : TestBase
    {
        private Player player;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.Location, "underworld")
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Item, new ItemBuilder()
                    .With(i => i.Id, 100)
                    .With(i => i.ConsentsToSoulbinding, false)
                    .BuildAndSave()
                ).BuildAndSave();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void can_set_consent_to(bool isConsenting)
        {
            player.Item.SetSoulbindingConsent(!isConsenting);
            Assert.That(
                Repository.Execute(new SetSoulbindingConsent {PlayerId = player.Id, IsConsenting = isConsenting}),
                Is.EqualTo($"You have set your soulbinding consent to <b>{isConsenting}</b>."));

            Assert.That(DataContext.AsQueryable<Item>().First(i => i.Id == player.Item.Id).ConsentsToSoulbinding,
                Is.EqualTo(isConsenting));
        }

        [Test]
        public void throw_exception_if_player_not_found()
        {
            var cmd = new SetSoulbindingConsent {PlayerId = -123, IsConsenting = true};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID '-123' not found."));
        }

        [Test]
        public void throw_exception_if_player_item_not_found()
        {

            player = new PlayerBuilder()
                .With(p => p.Id, 345)
                .With(p => p.Location, "underworld")
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Item, null)
                .BuildAndSave();

            var cmd = new SetSoulbindingConsent { PlayerId =  player.Id, IsConsenting = true };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You are not inanimate or a pet."));
        }
    }

}
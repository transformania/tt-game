using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class CanInteractWithTests : TestBase
    {
        private Player npc;
        private Player player;

        [SetUp]
        public void Init()
        {
            npc = new PlayerBuilder().With(p => p.BotId, -15)
                .With(p => p.FirstName, "Bot")
                .With(p => p.LastName, "Bottonson")
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_130_SUNNYGLADE_DRIVE)
                .BuildAndSave();

            player = new PlayerBuilder().With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Id, 1)
                .With(p => p.FirstName, "Bob")
                .With(p => p.LastName, "Human")
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_130_SUNNYGLADE_DRIVE)
                .BuildAndSave();
        }

        [Test]
        public void Should_return_true_if_conditions_okay()
        {
            Assert.That(Repository.FindSingle(new CanInteractWith { BotId = npc.BotId, PlayerId = player.Id }), Is.True);
        }

        [Test]
        public void Should_throw_exception_if_npc_not_found()
        {
            Assert.That(() => Repository.FindSingle(new CanInteractWith {BotId = -99, PlayerId = player.Id}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("NPC with BotId '-99' does not exist"));
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            Assert.That(() => Repository.FindSingle(new CanInteractWith {BotId = npc.BotId, PlayerId = -99}),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with Id '-99' does not exist"));
        }

        [Test]
        public void Should_throw_exception_if_player_not_animate()
        {
            var inanimatePlayer = new PlayerBuilder().With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Id, 2)
                .With(p => p.FirstName, "Bob")
                .With(p => p.LastName, "Human")
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Location, LocationsStatics.STREET_130_SUNNYGLADE_DRIVE)
                .BuildAndSave();

            Assert.That(
                () => Repository.FindSingle(new CanInteractWith {BotId = npc.BotId, PlayerId = inanimatePlayer.Id}),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must be animate in order to interact with Bot Bottonson."));
        }

        [Test]
        public void Should_throw_exception_if_player_in_duel()
        {
            var duelPlayer = new PlayerBuilder().With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Id, 100)
                .With(p => p.FirstName, "Bob")
                .With(p => p.LastName, "Human")
                .With(p => p.InDuel, 50)
                .BuildAndSave();

            Assert.That(() => Repository.FindSingle(new CanInteractWith {BotId = npc.BotId, PlayerId = duelPlayer.Id}),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must conclude your duel in order to interact with Bot Bottonson."));
        }

        [Test]
        public void Should_throw_exception_if_player_in_quest()
        {
            var questPlayer = new PlayerBuilder().With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Id, 500)
                .With(p => p.FirstName, "Bob")
                .With(p => p.LastName, "Human")
                .With(p => p.InQuest, 50)
                .BuildAndSave();

            Assert.That(() => Repository.FindSingle(new CanInteractWith {BotId = npc.BotId, PlayerId = questPlayer.Id}),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must conclude your quest in order to interact with Bot Bottonson."));
        }

        [Test]
        public void Should_throw_exception_if_bot_not_animate()
        {
            var inanimateNPC = new PlayerBuilder().With(p => p.BotId, -16)
                .With(p => p.Id, 123)
                .With(p => p.FirstName, "Bot")
                .With(p => p.LastName, "Bottonson")
                .With(p => p.Mobility, PvPStatics.MobilityInanimate)
                .With(p => p.Location, LocationsStatics.STREET_130_SUNNYGLADE_DRIVE)
                .BuildAndSave();

            Assert.That(
                () => Repository.FindSingle(new CanInteractWith {BotId = inanimateNPC.BotId, PlayerId = player.Id}),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Bot Bottonson must be animate in order for you to interact with him."));
        }

        [Test]
        public void Should_throw_exception_if_wrong_location()
        {
            var wrongPlacePlayer = new PlayerBuilder().With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .With(p => p.Id, 35)
                .With(p => p.FirstName, "Bob")
                .With(p => p.LastName, "Human")
                .With(p => p.Mobility, PvPStatics.MobilityFull)
                .With(p => p.Location, LocationsStatics.STREET_270_WEST_9TH_AVE)
                .BuildAndSave();

            Assert.That(
                () => Repository.FindSingle(new CanInteractWith {BotId = npc.BotId, PlayerId = wrongPlacePlayer.Id}),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You must be in the same location as Bot Bottonson in order to interact with him."));
        }
    }
}

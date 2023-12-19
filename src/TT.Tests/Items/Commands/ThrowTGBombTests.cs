using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class ThrowTGBombTests : TestBase
    {

        private const string RegularGuy = "Regular Guy";
        private const string RegularGirl = "Regular Girl";
        private const string TavernLocation = "tavern_bar";

        private BuffBox buffs = new BuffBox
        {
            FromItems_Agility = 50,
            FromForm_Agility = 50,
            FromEffects_Agility = 50,
            FromItems_Luck = 50,
            FromForm_Luck= 50,
            FromEffects_Luck = 50
        };

        [Test]
        public void can_throw_tg_bomb()
        {

            var regularGuyForm = new FormSourceBuilder()
                .With(p => p.AltSexFormSource, new FormSourceBuilder()
                    .With(p => p.FriendlyName, "Alt Sex Regular Guy -> Regular Girl Form")
                    .With(p => p.Gender, PvPStatics.GenderFemale)
                    .BuildAndSave())
                .With(p => p.FriendlyName, RegularGuy)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var regularGirlForm = new FormSourceBuilder()
                .With(p => p.AltSexFormSource, new FormSourceBuilder()
                    .With(p => p.FriendlyName, "Alt Sex Regular Girl -> Regular Guy Form")
                    .With(p => p.Gender, PvPStatics.GenderMale)
                    .BuildAndSave())
                .With(p => p.FriendlyName, RegularGirl)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var nonBaseForm = new FormSourceBuilder()
                .With(p => p.AltSexFormSource, null)
                .With(p => p.FriendlyName, "Some Other Form")
                .BuildAndSave();

            var bomb = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder().BuildAndSave())
                .With(i => i.Id, 5)
                .BuildAndSave();

            var thrower = new PlayerBuilder()
                .With(p => p.Id, 5)
                .With(p => p.FirstName, "Orb")
                .With(p => p.LastName, "Thrower")
                .With(p => p.Location, TavernLocation)
                .With(p => p.Items, new List<Item>())
                .With(p => p.XP, 3)
                .With(i => i.User, new UserBuilder()
                    .With(u => u.Stats, new List<Stat>())
                    .BuildAndSave())
                .BuildAndSave();
            thrower.GiveItem(bomb);

            // should get hit and turned into Regular Guy
            var femaleBystander = new PlayerBuilder()
               .With(p => p.Id, 6)
               .With(p => p.FirstName, "Female")
               .With(p => p.LastName, "Bystander")
               .With(p => p.Location, TavernLocation)
               .With(p => p.Gender, PvPStatics.GenderFemale)
               .With(p => p.FormSource, regularGirlForm)
               .With(p => p.PlayerLogs, new List<PlayerLog>())
               .BuildAndSave();

            // should get hit and turned into Regular Girl
            var maleBystander = new PlayerBuilder()
               .With(p => p.Id, 7)
               .With(p => p.Location, TavernLocation)
               .With(p => p.FirstName, "Male")
               .With(p => p.LastName, "Bystander")
               .With(p => p.Gender, PvPStatics.GenderMale)
               .With(p => p.FormSource, regularGuyForm)
               .With(p => p.PlayerLogs, new List<PlayerLog>())
               .BuildAndSave();

            // shouldn't get hit, in SuperProtection mode
            var wrongMode = new PlayerBuilder()
               .With(p => p.Id, 8)
               .With(p => p.FirstName, "Super")
               .With(p => p.LastName, "Protection")
               .With(p => p.Location, TavernLocation)
               .With(p => p.Gender, PvPStatics.GenderMale)
               .With(p => p.FormSource, regularGuyForm)
               .With(p => p.GameMode, (int)GameModeStatics.GameModes.Superprotection)
               .With(p => p.PlayerLogs, new List<PlayerLog>())
               .BuildAndSave();

            // shouldn't get hit, offline
            var offline = new PlayerBuilder()
               .With(p => p.Id, 9)
               .With(p => p.FirstName, "Off")
               .With(p => p.LastName, "Line")
               .With(p => p.Location, TavernLocation)
               .With(p => p.Gender, PvPStatics.GenderMale)
               .With(p => p.FormSource, regularGuyForm)
               .With(p => p.LastActionTimestamp, DateTime.UtcNow.AddMinutes(-1 - TurnTimesStatics.GetOfflineAfterXMinutes()))
               .With(p => p.PlayerLogs, new List<PlayerLog>())
               .BuildAndSave();

            // shouldn't get hit, not in a base form
            var nonBaseFormPlayer = new PlayerBuilder()
               .With(p => p.Id, 10)
               .With(p => p.FirstName, "Non")
               .With(p => p.LastName, "Base")
               .With(p => p.Location, TavernLocation)
               .With(p => p.Gender, PvPStatics.GenderMale)
               .With(p => p.FormSource, nonBaseForm)
               .With(p => p.PlayerLogs, new List<PlayerLog>())
               .BuildAndSave();

            Assert.That(Repository.Execute(new ThrowTGBomb {ItemId = bomb.Id, PlayerId = thrower.Id, Buffs = buffs }),
                Is.EqualTo(
                    "You throw your TG Splash Orb and swap the sex of 2 other mages near you: <b>Female Bystander</b> and <b>Male Bystander</b> and gain <b>6</b> XP!"));

            var players = DataContext.AsQueryable<Player>();

            var loadedThrower = players.First(p => p.Id == thrower.Id);
            Assert.That(loadedThrower.XP, Is.EqualTo(9));
            Assert.That(loadedThrower.User.Stats.First().AchievementType,
                Is.EqualTo(StatsProcedures.Stat__TgOrbVictims));
            Assert.That(loadedThrower.User.Stats.First().Amount, Is.EqualTo(2));

            var loadedfemaleBystander = players.First(p => p.Id == femaleBystander.Id);
            Assert.That(loadedfemaleBystander.FormSource.FriendlyName,
                Is.EqualTo("Alt Sex Regular Girl -> Regular Guy Form"));
            Assert.That(loadedfemaleBystander.Gender, Is.EqualTo(PvPStatics.GenderMale));
            Assert.That(loadedfemaleBystander.PlayerLogs.First().Message,
                Is.EqualTo(
                    "You yelp and feel your body change to that of the opposite sex from <b>Orb Thrower</b>'s use of a TG Splash Orb in your location!"));

            var loadedMaleBystander = players.First(p => p.Id == maleBystander.Id);
            Assert.That(loadedMaleBystander.FormSource.FriendlyName,
                Is.EqualTo("Alt Sex Regular Guy -> Regular Girl Form"));
            Assert.That(loadedMaleBystander.Gender, Is.EqualTo(PvPStatics.GenderFemale));
            Assert.That(loadedMaleBystander.PlayerLogs.First().Message,
                Is.EqualTo(
                    "You yelp and feel your body change to that of the opposite sex from <b>Orb Thrower</b>'s use of a TG Splash Orb in your location!"));

            Assert.That(players.First(p => p.Id == wrongMode.Id).FormSource.FriendlyName,
                Is.EqualTo(wrongMode.FormSource.FriendlyName)); // unchanged

            Assert.That(players.First(p => p.Id == offline.Id).FormSource.FriendlyName,
                Is.EqualTo(offline.FormSource.FriendlyName)); // unchanged

            Assert.That(players.First(p => p.Id == nonBaseFormPlayer.Id).FormSource.FriendlyName,
                Is.EqualTo(nonBaseFormPlayer.FormSource.FriendlyName)); // unchanged
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var bomb = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder().BuildAndSave())
                .With(i => i.Id, 5)
                .BuildAndSave();

            var cmd = new ThrowTGBomb { ItemId = bomb.Id, PlayerId = 12, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("player with ID 12 could not be found."));
        }

        [Test]
        public void should_throw_exception_if_player_id_not_specified()
        {
            var cmd = new ThrowTGBomb { ItemId = 12, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("PlayerId is required."));
        }

        [Test]
        public void should_throw_exception_if_item_not_found()
        {
            var thrower = new PlayerBuilder()
                .With(p => p.Id, 5)
                .With(p => p.Location, TavernLocation)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            var cmd = new ThrowTGBomb { ItemId = 2365, PlayerId = thrower.Id, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Item with ID 2365 could not be found or does not belong to you."));
        }

        [Test]
        public void should_throw_exception_if_item_id_not_specified()
        {
            var cmd = new ThrowTGBomb { PlayerId = 12, Buffs = buffs };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemId is required."));
        }

        [Test]
        public void should_throw_exception_if_buffs_not_specified()
        {
            var thrower = new PlayerBuilder()
                .With(p => p.Id, 5)
                .With(p => p.Location, TavernLocation)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            var bomb = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder().BuildAndSave())
                .With(i => i.Id, 5)
                .BuildAndSave();

            thrower.GiveItem(bomb);

            var cmd = new ThrowTGBomb { ItemId = bomb.Id, PlayerId = thrower.Id };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Buffs are missing."));
        }
    }
}

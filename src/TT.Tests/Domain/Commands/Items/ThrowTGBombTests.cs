using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Items;
using TT.Domain.Entities.Players;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Tests.Builders.Form;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Items
{
    [TestFixture]
    public class ThrowTGBombTests : TestBase
    {

        private const string RegularGuy = "Regular Guy";
        private const string RegularGirl = "Regular Girl";
        private const string BaseMaleForm = "man_01";
        private const string BaseFemaleForm = "woman_01";
        private const string TavernLocation = "tavern_bar";

        [Test]
        public void can_throw_tg_bomb()
        {

            var regularGuyForm = new FormSourceBuilder()
                .With(p => p.dbName, BaseMaleForm)
                .With(p => p.FriendlyName, RegularGuy)
                .With(p => p.Gender, PvPStatics.GenderMale)
                .BuildAndSave();

            var regularGirlForm = new FormSourceBuilder()
                .With(p => p.dbName, BaseFemaleForm)
                .With(p => p.FriendlyName, RegularGirl)
                .With(p => p.Gender, PvPStatics.GenderFemale)
                .BuildAndSave();

            var nonBaseForm = new FormSourceBuilder()
                .With(p => p.dbName, "nonbaseForm")
                .With(p => p.FriendlyName, "Some Other Form")
                .BuildAndSave();

            var bomb = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder().BuildAndSave())
                .With(i => i.dbName, PvPStatics.ItemType_TGBomb)
                .With(i => i.Id, 5)
                .BuildAndSave();

            var thrower = new PlayerBuilder()
                .With(p => p.Id, 5)
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
               .With(p => p.FirstName, "Lily")
               .With(p => p.FirstName, "Poole")
               .With(p => p.Location, TavernLocation)
               .With(p => p.Gender, PvPStatics.GenderFemale)
               .With(p => p.FormSource, regularGirlForm)
               .With(p => p.Form, BaseFemaleForm)
               .With(p => p.PlayerLogs, new List<PlayerLog>())
               .BuildAndSave();

            // should get hit and turned into Regular Girl
            var maleBystander = new PlayerBuilder()
              .With(p => p.Id, 7)
              .With(p => p.Location, TavernLocation)
              .With(p => p.FirstName, "Albert")
              .With(p => p.FirstName, "Smith")
              .With(p => p.Gender, PvPStatics.GenderMale)
              .With(p => p.FormSource, regularGuyForm)
              .With(p => p.Form, BaseMaleForm)
              .With(p => p.PlayerLogs, new List<PlayerLog>())
              .BuildAndSave();

            // shouldn't get hit, in SuperProtection mode
            var wrongMode = new PlayerBuilder()
             .With(p => p.Id, 8)
             .With(p => p.Location, TavernLocation)
             .With(p => p.Gender, PvPStatics.GenderMale)
             .With(p => p.Form, BaseMaleForm)
             .With(p => p.FormSource, regularGuyForm)
             .With(p => p.GameMode, GameModeStatics.SuperProtection)
             .With(p => p.PlayerLogs, new List<PlayerLog>())
             .BuildAndSave();

            // shouldn't get hit, offline
            var offline = new PlayerBuilder()
             .With(p => p.Id, 9)
             .With(p => p.Location, TavernLocation)
             .With(p => p.Gender, PvPStatics.GenderMale)
             .With(p => p.Form, BaseMaleForm)
             .With(p => p.FormSource, regularGuyForm)
             .With(p => p.LastActionTimestamp, DateTime.UtcNow.AddHours(-5))
             .With(p => p.PlayerLogs, new List<PlayerLog>())
             .BuildAndSave();

            // shouldn't get hit, offline
            var nonBaseFormPlayer = new PlayerBuilder()
             .With(p => p.Id, 10)
             .With(p => p.Location, TavernLocation)
             .With(p => p.Gender, PvPStatics.GenderMale)
             .With(p => p.Form, "otherform")
             .With(p => p.FormSource, nonBaseForm)
             .With(p => p.LastActionTimestamp, DateTime.UtcNow.AddHours(-5))
             .With(p => p.PlayerLogs, new List<PlayerLog>())
             .BuildAndSave();

            var result = Repository.Execute(new ThrowTGBomb { ItemId = bomb.Id, PlayerId = thrower.Id });
            result.Should().Be("You throw your TG Splash Orb and swap the sex of 2 other mages near you: <b>Poole Doe</b> and <b>Smith Doe</b> and gain <b>10</b> XP!");

            var players = DataContext.AsQueryable<Player>();

            var loadedThrower = players.First(p => p.Id == thrower.Id);
            loadedThrower.XP.Should().Be(13);
            loadedThrower.User.Stats.First().AchievementType.Should().Be(StatsProcedures.Stat__TgOrbVictims);
            loadedThrower.User.Stats.First().Amount.Should().Be(2);

            var loadedfemaleBystander = players.First(p => p.Id == femaleBystander.Id);
            loadedfemaleBystander.FormSource.FriendlyName.Should().Be("Regular Guy");
            loadedfemaleBystander.Gender.Should().Be(PvPStatics.GenderMale);
            loadedfemaleBystander.PlayerLogs.First().Message.Should().Be("You yelp and feel your body change to that of the opposite sex from <b>John Doe</b>'s use of a TG Splash Orb in your location!");

            var loadedMaleBystander = players.First(p => p.Id == maleBystander.Id);
            loadedMaleBystander.FormSource.FriendlyName.Should().Be("Regular Girl");
            loadedMaleBystander.Gender.Should().Be(PvPStatics.GenderFemale);
            loadedMaleBystander.PlayerLogs.First().Message.Should().Be("You yelp and feel your body change to that of the opposite sex from <b>John Doe</b>'s use of a TG Splash Orb in your location!");

            var wrongModeLoaded = players.First(p => p.Id == wrongMode.Id);
            wrongModeLoaded.FormSource.FriendlyName.Should().Be(wrongMode.FormSource.FriendlyName); // unchanged

            var offlineLoaded = players.First(p => p.Id == offline.Id);
            offlineLoaded.FormSource.FriendlyName.Should().Be(offline.FormSource.FriendlyName); // unchanged

            var nonBaseFormLoaded = players.First(p => p.Id == nonBaseFormPlayer.Id);
            nonBaseFormLoaded.FormSource.FriendlyName.Should().Be(nonBaseFormPlayer.FormSource.FriendlyName); // unchanged

        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var bomb = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder().BuildAndSave())
                .With(i => i.dbName, PvPStatics.ItemType_TGBomb)
                .With(i => i.Id, 5)
                .BuildAndSave();

            var cmd = new ThrowTGBomb {ItemId = bomb.Id, PlayerId = 12};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("player with ID 12 could not be found.");

        }

        [Test]
        public void should_throw_exception_if_player_id_not_specified()
        {
            var cmd = new ThrowTGBomb { ItemId = 12 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("PlayerId is required.");
        }

        [Test]
        public void should_throw_exception_if_item_not_found()
        {
            var thrower = new PlayerBuilder()
                .With(p => p.Id, 5)
                .With(p => p.Location, TavernLocation)
                .With(p => p.Items, new List<Item>())
                .BuildAndSave();

            var cmd = new ThrowTGBomb { ItemId = 2365, PlayerId = thrower.Id };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Item with ID 2365 could not be found or does not belong to you.");

        }

        [Test]
        public void should_throw_exception_if_item_id_not_specified()
        {
            var cmd = new ThrowTGBomb { PlayerId = 12 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ItemId is required.");
        }
    }
}

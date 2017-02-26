using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Domain.Statics;
using TT.Tests.Builders.AI;

namespace TT.Tests.Domain.Commands.Players
{
    public class CreatePlayersTests : TestBase
    {

        CreatePlayer cmd;
        User user;

        [SetUp]
        public void Init()
        {
            user = new UserBuilder().With(u => u.Id, "guid")
                .With(u => u.Donator, new DonatorBuilder()
                    .With(d => d.Tier, 2)
                    .BuildAndSave())
                .BuildAndSave();

            cmd = new CreatePlayer();
            cmd.FirstName = "Bob";
            cmd.LastName = "McBobbinson";
            cmd.UserId = user.Id;
            cmd.Form = "werewolf";
            cmd.Health = 100;
            cmd.MaxHealth = 100;
            cmd.Location = "now here is nowhere";
            cmd.Gender = PvPStatics.GenderMale;
        }


        [Test]
        public void Should_create_new_player()
        {

            Repository.Execute(cmd);

            var player = DataContext.AsQueryable<Player>().First();
            player.FirstName.Should().Be("Bob");
            player.LastName.Should().Be("McBobbinson");
            player.User.Id.Should().Be(user.Id);
            player.DonatorLevel.Should().Be(2);

        }

        [Test]
        public void Should_create_new_player_without_user()
        {

            cmd.UserId = null;

            Repository.Execute(cmd);

            var player = DataContext.AsQueryable<Player>().First();
            player.FirstName.Should().Be("Bob");
            player.LastName.Should().Be("McBobbinson");
            player.User.Should().Be(null);
            player.DonatorLevel.Should().Be(0);

        }

        [Test]
        public void Can_create_new_player_with_no_user()
        {
            cmd.UserId = null;
            Repository.Execute(cmd);

            DataContext.AsQueryable<Player>().Count(p =>
                p.FirstName == "Bob" &&
                p.LastName == "McBobbinson" &&
                p.User == null)
            .Should().Be(1);
        }

        [Test]
        public void Should_not_allow_empty_first_name()
        {
            cmd.FirstName = "";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("First name is required");
        }

        [Test]
        public void Should_not_allow_empty_last_name()
        {
            cmd.LastName = "";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Last name is required");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_willpower_less_than_or_equal_to_zero(decimal wp)
        {
            cmd.Health = wp;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Willpower must be greater than 0");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_max_willpower_less_than_or_equal_to_zero(decimal wp)
        {
            cmd.MaxHealth = wp;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Maximum willpower must be greater than 0");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_mana_less_than_or_equal_to_zero(decimal mana)
        {
            cmd.Mana = mana;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Mana must be greater than 0");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_max_mana_less_than_or_equal_to_zero(decimal mana)
        {
            cmd.MaxMana = mana;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Maximum mana must be greater than 0");
        }

        [Test]
        public void Should_default_level_to_one()
        {
            var playerId = Repository.Execute(cmd);

            var memPlayer = DataContext.AsQueryable<Player>().First(p =>
                p.User.Id == user.Id &&
                p.Level == 1);

            memPlayer.Level.Should().Be(1);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_level_lower_than_one(int level)
        {
            cmd.Level = 0;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Level must be at least one.");
        }

        [Test]
        public void Should_not_allow_times_attacking_this_update_lower_than_zero()
        {
            cmd.TimesAttackingThisUpdate = -1;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("TimesAttackingThisUpdate must be at least 0");
        }

        [Test]
        public void Should_not_allow_xp_lower_than_zero()
        {
            cmd.XP = -1;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("XP must be at least 0");
        }

        [Test]
        public void Should_not_allow_action_points_lower_than_zero()
        {
            cmd.ActionPoints = -1;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ActionPoints must be at least 0");
        }

        [Test]
        public void Should_not_allow_action_point_refill_lower_than_zero()
        {
            cmd.ActionPoints_Refill = -1;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ActionPoints_Refill must be at least 0");
        }

        [Test]
        public void Should_not_allow_action_points_greater_than_max()
        {
            cmd.ActionPoints = 121;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("ActionPoints must be less than " + PvPStatics.MaximumStoreableActionPoints));
        }

        [Test]
        public void Should_not_allow_action_point_refills_greater_than_max()
        {
            cmd.ActionPoints_Refill = 361;

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("ActionPoints_Refill must be less than " + PvPStatics.MaximumStoreableActionPoints_Refill);
        }

        [Test]
        public void Should_not_allow_invalid_gender_type()
        {
            cmd.Gender = "wasda";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Gender must be either " + PvPStatics.GenderMale + " or " + PvPStatics.GenderFemale);
        }

        [Test]
        public void Should_not_allow_invalid_mobility_type()
        {
            cmd.Mobility = "wasda";

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Mobility must be one of the following: " + PvPStatics.MobilityFull + ", " + PvPStatics.MobilityInanimate + ", or " + PvPStatics.MobilityPet);
        }

        [Test]
        public void Should_have_NPC()
        {
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();
            cmd.NPCId = npc.Id;

            Repository.Execute(cmd);

            DataContext.AsQueryable<Player>().Count(p =>
                p.NPC.Id == npc.Id)
            .Should().Be(1);

        }

    }
}

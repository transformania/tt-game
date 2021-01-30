using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Domain.Statics;
using TT.Tests.Builders.AI;
using TT.Tests.Builders.Form;

namespace TT.Tests.Players.Commands
{
    public class CreatePlayersTests : TestBase
    {
        private CreatePlayer cmd;
        private User user;
        private FormSource formSource;

        [SetUp]
        public void Init()
        {
            user = new UserBuilder().With(u => u.Id, "guid")
                .With(u => u.Donator, new DonatorBuilder()
                    .With(d => d.Tier, 2)
                    .BuildAndSave())
                .BuildAndSave();

            formSource = new FormSourceBuilder()
                .With(f => f.Id, 100)
                .With(f => f.FriendlyName, "Some Form")
                .BuildAndSave();

            cmd = new CreatePlayer
            {
                FirstName = "Bob",
                LastName = "McBobbinson",
                UserId = user.Id,
                Health = 100,
                MaxHealth = 100,
                Location = "now here is nowhere",
                Gender = PvPStatics.GenderMale,
                FormSourceId = formSource.Id
            };
        }


        [Test]
        public void Should_create_new_player()
        {

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var player = DataContext.AsQueryable<Player>().First();
            Assert.That(player.FirstName, Is.EqualTo("Bob"));
            Assert.That(player.LastName, Is.EqualTo("McBobbinson"));
            Assert.That(player.OriginalFirstName, Is.EqualTo("Bob"));
            Assert.That(player.OriginalLastName, Is.EqualTo("McBobbinson"));
            Assert.That(player.User.Id, Is.EqualTo(user.Id));
            Assert.That(player.DonatorLevel, Is.EqualTo(2));

        }

        [Test]
        public void Should_create_new_player_without_user()
        {
            cmd.UserId = null;

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var player = DataContext.AsQueryable<Player>().First();
            Assert.That(player.FirstName, Is.EqualTo("Bob"));
            Assert.That(player.LastName, Is.EqualTo("McBobbinson"));
            Assert.That(player.OriginalFirstName, Is.EqualTo("Bob"));
            Assert.That(player.OriginalLastName, Is.EqualTo("McBobbinson"));
            Assert.That(player.User, Is.Null);
            Assert.That(player.DonatorLevel, Is.EqualTo(0));
            Assert.That(DataContext.AsQueryable<Player>().Where(p =>
                    p.FirstName == "Bob" &&
                    p.LastName == "McBobbinson" &&
                    p.User == null), Has.Exactly(1).Items);
        }

        [Test]
        public void Should_not_allow_empty_first_name()
        {
            cmd.FirstName = "";

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("First name is required"));
        }

        [Test]
        public void Should_not_allow_empty_last_name()
        {
            cmd.LastName = "";

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Last name is required"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_willpower_less_than_or_equal_to_zero(decimal wp)
        {
            cmd.Health = wp;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Willpower must be greater than 0"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_max_willpower_less_than_or_equal_to_zero(decimal wp)
        {
            cmd.MaxHealth = wp;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Maximum willpower must be greater than 0"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_mana_less_than_or_equal_to_zero(decimal mana)
        {
            cmd.Mana = mana;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Mana must be greater than 0"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_max_mana_less_than_or_equal_to_zero(decimal mana)
        {
            cmd.MaxMana = mana;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Maximum mana must be greater than 0"));
        }

        [Test]
        public void Should_default_level_to_one()
        {
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p =>
                p.User.Id == user.Id &&
                p.Level == 1).Level, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Should_not_allow_level_lower_than_one(int level)
        {
            cmd.Level = 0;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Level must be at least one."));
        }

        [Test]
        public void Should_not_allow_times_attacking_this_update_lower_than_zero()
        {
            cmd.TimesAttackingThisUpdate = -1;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("TimesAttackingThisUpdate must be at least 0"));
        }

        [Test]
        public void Should_not_allow_xp_lower_than_zero()
        {
            cmd.XP = -1;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("XP must be at least 0"));
        }

        [Test]
        public void Should_not_allow_action_points_lower_than_zero()
        {
            cmd.ActionPoints = -1;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ActionPoints must be at least 0"));
        }

        [Test]
        public void Should_not_allow_action_point_refill_lower_than_zero()
        {
            cmd.ActionPoints_Refill = -1;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ActionPoints_Refill must be at least 0"));
        }

        [Test]
        public void Should_not_allow_action_points_greater_than_max()
        {
            cmd.ActionPoints = TurnTimesStatics.GetActionPointLimit() + 1;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo($"ActionPoints must be less than {TurnTimesStatics.GetActionPointLimit()}"));
        }

        [Test]
        public void Should_not_allow_action_point_refills_greater_than_max()
        {
            cmd.ActionPoints_Refill = TurnTimesStatics.GetActionPointReserveLimit() + 1;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo($"ActionPoints_Refill must be less than {TurnTimesStatics.GetActionPointReserveLimit()}"));
        }

        [Test]
        public void Should_not_allow_invalid_gender_type()
        {
            cmd.Gender = "wasda";

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo($"Gender must be either {PvPStatics.GenderMale} or {PvPStatics.GenderFemale}"));
        }

        [Test]
        public void Should_not_allow_invalid_mobility_type()
        {
            cmd.Mobility = "wasda";

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    $"Mobility must be one of the following: {PvPStatics.MobilityFull}, {PvPStatics.MobilityInanimate}, or {PvPStatics.MobilityPet}"));
        }

        [Test]
        public void Should_have_NPC()
        {
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();
            cmd.NPCId = npc.Id;

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().Where(p =>
                p.NPC.Id == npc.Id), Has.Exactly(1).Items);
        }

    }
}

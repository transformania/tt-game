using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{
    [TestFixture()]
    public class ChangeGenderTests : TestBase
    {
        [Test]
        public void should_change_to_female_when_male()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.Gender, "male")
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeGender
                    {MembershipId = player.User.Id, changeGender = "female"}), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").Gender, Is.EqualTo("female"));
        }

        [Test]
        public void should_change_to_male_when_female()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.Gender, "female")
                .BuildAndSave();

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeGender
                { MembershipId = player.User.Id, changeGender = "male" }), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").Gender, Is.EqualTo("male"));
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeGender {MembershipId = "fake", changeGender = "male"};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with MembershipID 'fake' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeGender { MembershipId = null, changeGender = "male" };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("MembershipID is required!"));
        }
    }
}

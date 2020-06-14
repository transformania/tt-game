using NUnit.Framework;
using TT.Domain.Procedures;

namespace TT.Tests.Services
{
    [TestFixture]
    class PlayerProcedureTests
    {

        [TestFixture]
        public class Tests_for_base_willpower_mana_and_xp_to_next_level : TestBase
        {
            [Test]
            [TestCase(1, 100)]
            [TestCase(2, 140)]
            [TestCase(3, 200)]
            [TestCase(12, 1680)]
            [TestCase(27, 8120)]
            public void Should_retrieve_correct_xp_requirement_for_levelup(int level, int expectedXp)
            {
                Assert.That(PlayerProcedures.GetXPNeededForLevelUp(level), Is.EqualTo(expectedXp));
            }

            [Test]
            [TestCase(1, 50)]
            [TestCase(2, 55)]
            [TestCase(7, 80)]
            [TestCase(55, 320)]
            public void Should_retrieve_correct_mana_base_by_level(int level, int expectedMana)
            {
                Assert.That(PlayerProcedures.GetManaBaseByLevel(level), Is.EqualTo(expectedMana));
            }


            [Test]
            [TestCase(1, 100)]
            [TestCase(2, 110)]
            [TestCase(7, 160)]
            [TestCase(55, 640)]
            public void Should_retrieve_correct_wp_base_by_level(int level, int expectedWillpower)
            {
                Assert.That(PlayerProcedures.GetWillpowerBaseByLevel(level), Is.EqualTo(expectedWillpower));
            }
        }
    }
}

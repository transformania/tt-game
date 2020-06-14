using NUnit.Framework;
using TT.Domain.Procedures;

namespace TT.Tests.Services
{
    [TestFixture]
    class ItemProcedureTests : TestBase
    {
        [Test]
        [TestCase(1, 100)]
        [TestCase(2, 140)]
        [TestCase(3, 200)]
        [TestCase(12, 1680)]
        [TestCase(27, 8120)]
        public void Should_retrieve_correct_xp_requirement_for_inanimate_levelup(int level, int expectedXp)
        {
            Assert.That(ItemProcedures.GetXPRequiredForItemPetLevelup(level), Is.EqualTo(expectedXp));
        }
    }
}

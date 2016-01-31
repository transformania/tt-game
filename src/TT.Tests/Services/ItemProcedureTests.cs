using NUnit.Framework;
using TT.Domain.Procedures;

namespace TT.Tests.Services
{
    class ItemProcedureTests
    {

        [TestFixture]
        public class QuestTests
        {

            // privates

            [SetUp]
            public void SetUp()
            {

            }

            [Test]
            public void Should_retrieve_correct_xp_requirement_for_inanimate_levelup()
            {
                float xpRequired_lvl1 = ItemProcedures.GetXPRequiredForItemPetLevelup(1);
                Assert.AreEqual(100, xpRequired_lvl1);

                float xpRequired_lvl2 = ItemProcedures.GetXPRequiredForItemPetLevelup(2);
                Assert.AreEqual(140, xpRequired_lvl2);

                float xpRequired_lvl3 = ItemProcedures.GetXPRequiredForItemPetLevelup(3);
                Assert.AreEqual(200, xpRequired_lvl3);

                float xpRequired_lvl12 = ItemProcedures.GetXPRequiredForItemPetLevelup(12);
                Assert.AreEqual(1680, xpRequired_lvl12);

                float xpRequired_lvl27 = ItemProcedures.GetXPRequiredForItemPetLevelup(27);
                Assert.AreEqual(8120, xpRequired_lvl27);

            }

           
        }

    }
}

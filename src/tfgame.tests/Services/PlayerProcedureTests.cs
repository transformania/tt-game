using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tfgame.Procedures;

namespace tfgame.Tests.Services
{
    class PlayerProcedureTests
    {

        [TestFixture]
        public class Tests_for_base_willpower_mana_and_xp_to_next_level
        {

            // privates

            [SetUp]
            public void SetUp()
            {

            }

            [Test]
            public void Should_retrieve_correct_xp_requirement_for_levelup()
            {
                float xpRequired_lvl1 = PlayerProcedures.GetXPNeededForLevelUp(1);
                Assert.AreEqual(100, xpRequired_lvl1);

                float xpRequired_lvl2 = PlayerProcedures.GetXPNeededForLevelUp(2);
                Assert.AreEqual(140, xpRequired_lvl2);

                float xpRequired_lvl3 = PlayerProcedures.GetXPNeededForLevelUp(3);
                Assert.AreEqual(200, xpRequired_lvl3);

                float xpRequired_lvl12 = PlayerProcedures.GetXPNeededForLevelUp(12);
                Assert.AreEqual(1680, xpRequired_lvl12);

                float xpRequired_lvl27 = PlayerProcedures.GetXPNeededForLevelUp(27);
                Assert.AreEqual(8120, xpRequired_lvl27);

            }

            [Test]
            public void Should_retrieve_correct_mana_base_by_level()
            {
                float manaBase_lvl1 = PlayerProcedures.GetManaBaseByLevel(1);
                Assert.AreEqual(50, manaBase_lvl1);

                float manaBase_lvl2 = PlayerProcedures.GetManaBaseByLevel(2);
                Assert.AreEqual(55, manaBase_lvl2);

                float manaBase_lvl7 = PlayerProcedures.GetManaBaseByLevel(7);
                Assert.AreEqual(80, manaBase_lvl7);

                float manaBase_lvl55 = PlayerProcedures.GetManaBaseByLevel(55);
                Assert.AreEqual(320, manaBase_lvl55);
            }


            [Test]
            public void Should_retrieve_correct_wp_base_by_level()
            {
                float wpBase_lvl1 = PlayerProcedures.GetWillpowerBaseByLevel(1);
                Assert.AreEqual(100, wpBase_lvl1);

                float wpBase_lvl2 = PlayerProcedures.GetWillpowerBaseByLevel(2);
                Assert.AreEqual(115, wpBase_lvl2);

                float wpBase_lvl7 = PlayerProcedures.GetWillpowerBaseByLevel(7);
                Assert.AreEqual(190, wpBase_lvl7);

                float wpBase_lvl55 = PlayerProcedures.GetWillpowerBaseByLevel(55);
                Assert.AreEqual(910, wpBase_lvl55);
            }
        }

    }
}

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
        public class QuestTests
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
        }

    }
}

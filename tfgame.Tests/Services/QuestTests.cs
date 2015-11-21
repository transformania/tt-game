using System.Linq;
using System.Security.Principal;
using System.Web;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using tfgame.dbModels;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Queries.DMRoll;
using tfgame.Services;
using tfgame.Procedures;
using tfgame.dbModels.Models;
using tfgame.ViewModels;
using System.Collections.Generic;
using tfgame.Statics;

namespace tfgame.Tests.Services
{

    /// <summary>
    /// Builder object for QuestConnections
    /// </summary>
    public class QuestConnectionBuilder
    {
        private QuestConnection _questConnection { get; set; }

        /// <summary>
        /// Constructor method
        /// </summary>
        public QuestConnectionBuilder()
        {
            this._questConnection = new QuestConnection();
            this._questConnection.QuestConnectionRequirements = new List<QuestConnectionRequirement>();
        }

        /// <summary>
        /// Add a quest connection requirement that must be passed, not a roll
        /// </summary>
        /// <param name="RequirementType"></param>
        /// <param name="RequirementValue"></param>
        /// <param name="Operator">Operator for the requirement, ie \>, \<, ==</param>
        public void AddStrictRequirement(int RequirementType, string RequirementValue, int Operator)
        {
            this._questConnection.QuestConnectionRequirements.Add(new QuestConnectionRequirement
            {
                IsRandomRoll = false,
                RequirementType = RequirementType,
                RequirementValue = RequirementValue,
                Operator = Operator
            });
        }

        /// <summary>
        /// Add a quest connection requirement that is based on a probabilistic roll
        /// </summary>
        /// <param name="RequirementType"></param>
        /// <param name="Modififer"></param>
        /// <param name="Offset"></param>
        public void AddRollRequirement(int RequirementType, float Modififer, float Offset)
        {
            this._questConnection.QuestConnectionRequirements.Add(new QuestConnectionRequirement
            {
                RequirementType = RequirementType,
                IsRandomRoll = true,
                RollModifier = Modififer,
                RollOffset = Offset
            });
        }

        /// <summary>
        /// Return the built QuestConnection object
        /// </summary>
        /// <returns></returns>
        public QuestConnection GetQuestConnection()
        {
            return this._questConnection;
        }
    }

    [TestFixture]
    public class QuestTests
    {

        private QuestConnectionBuilder b;
        private BuffBox buffs;

        [SetUp]
        public void SetUp()
        {
            b = new QuestConnectionBuilder();
            buffs = new BuffBox();
        }

        [Test]
        public void Should_print_correct_choice_text_1_roll_requirement()
        {
            b.AddRollRequirement((int)QuestStatics.RequirementType.Luck, 1.25f, 10);
            QuestConnection q = b.GetQuestConnection();
            buffs.FromEffects_Luck = 50;

            string message = QuestProcedures.GetRequirementsAsString(q, buffs);

            message.Should().Be("[Luck - 72.5%]");
        }

        [Test]
        public void Should_print_correct_choice_text_2_roll_requirements()
        {
            b.AddRollRequirement((int)QuestStatics.RequirementType.Charisma, .5f, 0);
            b.AddRollRequirement((int)QuestStatics.RequirementType.Magicka, .5f, 15);
            QuestConnection q = b.GetQuestConnection();


            buffs.FromEffects_Charisma = 25;

            string message = QuestProcedures.GetRequirementsAsString(q, buffs);

            message.Should().Be("[Charisma - 12.5%, Magicka - 15%]");
        }

        [Test]
        public void Should_print_correct_choice_text_1_strict_requirement()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Luck, "150", (int)QuestStatics.Operator.Greater_Than);
            QuestConnection q = b.GetQuestConnection();

            string message = QuestProcedures.GetRequirementsAsString(q, buffs);

            message.Should().Be("[150 Luck]");
        }

        [Test]
        public void Should_print_correct_choice_text_2_strict_requirement()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Luck, "150", (int)QuestStatics.Operator.Greater_Than);
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Perception, "35", (int)QuestStatics.Operator.Equal_To);

            QuestConnection q = b.GetQuestConnection();

            buffs.FromEffects_Luck = 50;

            string message = QuestProcedures.GetRequirementsAsString(q, buffs);

            message.Should().Be("[150 Luck, 35 Perception]");
        }

        [Test]
        public void Should_print_correct_choice_text_mixed_requirements()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Fortitude, "10", (int)QuestStatics.Operator.Greater_Than_Or_Equal);
            b.AddRollRequirement((int)QuestStatics.RequirementType.Magicka, 1.0f, -50);
            QuestConnection q = b.GetQuestConnection();

            buffs.FromEffects_Magicka = 60;

            string message = QuestProcedures.GetRequirementsAsString(q, buffs);

            message.Should().Be("[10 Fortitude, Magicka - 10%]");
        }

    }
}

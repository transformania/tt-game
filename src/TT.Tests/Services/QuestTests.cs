using System;
using System.Collections.Generic;
using NUnit.Framework;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Tests.Services
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
        /// <param name="Operator">Operator for the requirement, ie &gt;, &lt;, ==</param>
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
    public class QuestTests : TestBase
    {

        private QuestConnectionBuilder b;
        private BuffBox buffs;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            b = new QuestConnectionBuilder();
            buffs = new BuffBox();
        }

        [Test]
        public void Should_print_correct_choice_text_1_roll_requirement()
        {
            b.AddRollRequirement((int)QuestStatics.RequirementType.Luck, 1.25f, 10);
            var q = b.GetQuestConnection();
            buffs.FromEffects_Luck = 50;

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.EqualTo("[Luck - 72.5%]"));
        }

        [Test]
        public void Should_print_correct_choice_text_2_roll_requirements()
        {
            b.AddRollRequirement((int)QuestStatics.RequirementType.Charisma, .5f, 0);
            b.AddRollRequirement((int)QuestStatics.RequirementType.Magicka, .5f, 15);
            var q = b.GetQuestConnection();


            buffs.FromEffects_Charisma = 25;

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs),
                Is.EqualTo("[Charisma - 12.5%, Magicka - 15%]"));
        }

        [Test]
        public void Should_print_correct_choice_text_1_strict_requirement()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Luck, "150", (int)QuestStatics.Operator.Greater_Than);
            var q = b.GetQuestConnection();

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.EqualTo("[> 150 Luck]"));
        }

        [Test]
        public void Should_print_correct_choice_text_2_strict_requirement()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Luck, "150", (int)QuestStatics.Operator.Greater_Than);
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Perception, "35", (int)QuestStatics.Operator.Equal_To);

            var q = b.GetQuestConnection();

            buffs.FromEffects_Luck = 50;

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.EqualTo("[> 150 Luck, = 35 Perception]"));
        }

        [Test]
        public void Should_print_correct_choice_text_mixed_requirements()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Fortitude, "10", (int)QuestStatics.Operator.Greater_Than_Or_Equal);
            b.AddRollRequirement((int)QuestStatics.RequirementType.Magicka, 1.0f, -50);
            var q = b.GetQuestConnection();

            buffs.FromEffects_Magicka = 60;

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs),
                Is.EqualTo("[>= 10 Fortitude, Magicka - 10%]"));
        }

        [Test]
        public void Should_not_print_details_for_required_gender()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Gender, PvPStatics.GenderMale, 0);
            var q = b.GetQuestConnection();

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.Empty);
        }

        [Test]
        public void Should_not_print_details_for_required_form()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Form, "derpform", 0);
            var q = b.GetQuestConnection();

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.Empty);
        }

        [Test]
        public void Should_not_print_details_for_required_variable()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Variable, "variable", 0);
            var q = b.GetQuestConnection();

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.Empty);
        }

        [Test]
        public void Should_not_print_details_for_mixed_hidden_requirements()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Variable, "variable", 0);
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Gender, PvPStatics.GenderMale, 0);
            var q = b.GetQuestConnection();

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.Empty);
        }

        [Test]
        [Ignore("TODO")]
        public void Should_hide_only_hidden_requirements_when_mixed()
        {
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Variable, "variable", 0);
            b.AddStrictRequirement((int)QuestStatics.RequirementType.Succour, "25", (int)QuestStatics.Operator.Greater_Than_Or_Equal);
            var q = b.GetQuestConnection();

            Assert.That(QuestProcedures.GetRequirementsAsString(q, buffs), Is.EqualTo("[>= 25 Regeneration]"));
        }
    }

    [TestFixture]
    public class QuestTests_PlayerCanStartQuest : TestBase
    {
        private Player player;
        private QuestStart questStart;
        private int turnNumber;
        private List<QuestPlayerStatus> questPlayerStatuses;

        private int fakeTestId = 1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            player = new Player
            {
                dbLocationName = "location",
                Mobility = PvPStatics.MobilityFull,
                Gender = PvPStatics.GenderMale,
                LastCombatTimestamp = DateTime.UtcNow.AddYears(-1),
                LastCombatAttackedTimestamp = DateTime.UtcNow.AddYears(-1),
                BotId = AIStatics.ActivePlayerBotId,
                FirstName = "FirstName",
                LastName = "Lastname",
                Level = 1,
                ActionPoints = TurnTimesStatics.GetActionPointLimit(),
                Id = 1,
                LastActionTimestamp = DateTime.UtcNow.AddYears(-1),
                GameMode  = 0,
                Health = 100,
                MaxHealth = 100,
                Mana = 100,
                MaxMana = 100,
            };
            questStart = new QuestStart
            {
                RequiredGender = (int)QuestStatics.Gender.Any,
                PrerequisiteQuest = 0,
                IsLive = true,
                Location = "location",
                Id = fakeTestId,
                MinStartLevel = 0,
                MaxStartLevel = 99999,
                MinStartTurn = 0,
                MaxStartTurn = 99999,
                Name = "TestQuest",
                StartState = 1
            };
            turnNumber = 2000;
            questPlayerStatuses = new List<QuestPlayerStatus>();
        }

        [Test]
        public void Cant_start_quests_that_are_not_live()
        {
            questStart.IsLive = false;
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Cant_start_quests_if_level_too_low()
        {
            questStart.MinStartLevel = 3;
            player.Level = 2;
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Cant_start_quests_if_level_too_high()
        {
            questStart.MinStartLevel = 3;
            player.Level = 2;
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Cant_start_quests_if_not_male_and_needs_male()
        {
            questStart.RequiredGender = (int)QuestStatics.Gender.Male;
            player.Gender = PvPStatics.GenderFemale;
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Cant_start_quests_if_not_female_and_needs_female()
        {
            questStart.RequiredGender = (int)QuestStatics.Gender.Female;
            player.Gender = PvPStatics.GenderMale;
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Cant_start_quests_if_quest_already_completed()
        {
            questPlayerStatuses.Add(new QuestPlayerStatus
            {
                QuestId = fakeTestId,
                Outcome = (int)QuestStatics.QuestOutcomes.Completed
            });
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        [Ignore("TODO -- method doesn't actually have this coded in yet.")]
        public void Cant_start_quests_if_quest_recently_failed()
        {
            questPlayerStatuses.Add(new QuestPlayerStatus
            {
                QuestId = fakeTestId,
                Outcome = (int)QuestStatics.QuestOutcomes.Failed,
                LastEndedTurn = turnNumber - 1,
                StartedTurn = turnNumber - 1
            });
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Can_start_quests_if_quest_not_recently_failed()
        {
            questPlayerStatuses.Add(new QuestPlayerStatus
            {
                QuestId = fakeTestId,
                Outcome = (int)QuestStatics.QuestOutcomes.Failed,
                LastEndedTurn = turnNumber - QuestStatics.QuestFailCooldownTurnLength - 1
            });
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.True);
        }

        [Test]
        public void Cant_start_quest_if_prerequisite_test_not_completed()
        {
            questStart.PrerequisiteQuest = 2;
            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.False);
        }

        [Test]
        public void Can_start_quest_if_prerequisite_test_is_completed()
        {
            questStart.PrerequisiteQuest = 2;

            questPlayerStatuses.Add(new QuestPlayerStatus
            {
                QuestId = 2,
                Outcome = (int)QuestStatics.QuestOutcomes.Completed
            });

            Assert.That(QuestProcedures.PlayerCanBeginQuest(player, questStart, questPlayerStatuses, turnNumber),
                Is.True);
        }
    }
}
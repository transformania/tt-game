﻿namespace TT.Domain.Statics
{
    public static class QuestStatics
    {

        public const int ActionAPCost = 1;
        public const int QuestFailCooldownTurnLength = 12;

        public enum QuestOutcomes { Failed = 0, Completed = 1, RepeatCompleted = 2 };

        public enum Gender { Male = 0, Female = 1, Any = 2 };

        public enum Operator {
            Less_Than = 0,
            Less_Than_Or_Equal = 1,
            Equal_To = 2,
            Greater_Than_Or_Equal = 3,
            Greater_Than = 4,
            Not_Equal_To = 5
        };

        public enum RequirementType {
            Discipline = 0,
            Perception = 1,
            Charisma = 2,
            Fortitude = 3,
            Agility = 4,
            Allure = 5,
            Magicka = 6,
            Succour = 7,
            Luck = 8, 
            Variable = 9,
            Gender = 10,
            Form = 11,
            Item = 12,
            Hardmode = 13,
            PvPScore = 14,
            Mana = 15,
            Willpower = 16,
            Money = 17,
            Spell = 18,
        }

        public enum RewardType
        {
            Experience = 0,
            Item = 1,
            Effect = 2,
            Spell = 3,
            Money = 4,
            PvPScore = 5,
        }

        public enum PreactionType
        {
            Variable = 0,
            Form = 1,
            Willpower = 2,
            Mana = 3,
            MoveToLocation = 4,
            Money = 5,
            Item = 6,
            PvPScore = 7,
        }

        public enum AddOrSet
        {
            Set = 0,
            Add_Number = 1,
            Add_Percent = 2,
        }




    }
}
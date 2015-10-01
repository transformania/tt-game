using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.Statics
{
    public static class QuestStatics
    {
        public enum QuestOutcomes { Failed = 0, Completed = 1 };

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
        }

        public enum RewardType
        {
            Experience = 0,
            Item = 1,
            Effect = 2,
        }

        public enum QuestStatePreactionType
        {
            Variable = 0,
            Form = 1,
            Willpwer = 2,
            Mana = 3,
        }




    }
}
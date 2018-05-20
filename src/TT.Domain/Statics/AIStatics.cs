using System;

namespace TT.Domain.Statics
{
    public static class AIStatics
    {
        public const int ActivePlayerBotId = 0;
        public const int RerolledPlayerBotId = -1;
        public const int PsychopathBotId = -2;
        public const int LindellaBotId = -3;
        public const int DonnaBotId = -4;
        public const int ValentineBotId = -5;
        public const int JewdewfaeBotId = -6;
        public const int BimboBossBotId = -7;
        public const int MaleRatBotId = -8;
        public const int FemaleRatBotId = -9;
        public const int WuffieBotId = -10;
        public const int MouseNerdBotId = -11;
        public const int MouseBimboBotId = -12;
        public const int DemonBotId = -13;
        public const int BartenderBotId = -14;
        public const int LoremasterBotId = -15;
        public const int FaebossBotId = -16;
        public const int MotorcycleGangLeaderBotId = -17;

        public const int DungeonDemonFormId = 371;

        public const string ACTIVE = "active";
        public const string UNSTARTED = "unstarted";

        public static bool IsABoss(int id)
        {
            return id == ValentineBotId ||
                   id == DonnaBotId ||
                   id == BimboBossBotId ||
                   id == FemaleRatBotId ||
                   id == MaleRatBotId ||
                   id == MouseNerdBotId ||
                   id == MouseBimboBotId ||
                   id == FaebossBotId ||
                   id == MotorcycleGangLeaderBotId;
        }
    }
}
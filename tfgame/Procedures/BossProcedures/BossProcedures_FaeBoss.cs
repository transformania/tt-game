using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Procedures.BossProcedures
{
    public class BossProcedures_FaeBoss
    {

        // If you're a dark fae then you're corrupted. Then you can't attack her. If you're a great fairy then she'd be pissier with you I'd say because in her eyes, you're a traitor

        public const string FirstName = "Narcissa";
        public const string LastName = "the Exiled";
        public const string Form = "form_Corrupted_Lunar_Fae_Roxanne246810(Rachael_Victor/Yuki_Kitsu)";
        public const string SpawnLocation = "forest_pinecove";

        public static Player SpawnFaeBoss()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);

            if (faeboss == null)
            {
                faeboss = new Player()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    ActionPoints = 120,
                    dbLocationName = SpawnLocation,
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = Form,
                    Money = 1000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 25,
                    MembershipId = AIStatics.FaebossId.ToString(),
                    BotId = AIStatics.JewdewfaeBotId,
                    ActionPoints_Refill = 360,
                };

            }

            playerRepo.SavePlayer(faeboss);
            return faeboss;
        }

        public static bool SpellIsValid(string spellName, Player caster)
        {



            return false;
        }

        public static void RunTurnLogic()
        {
             
        }

        public static void CounterAttack()
        {

        }
    }
}
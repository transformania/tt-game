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

        public const string FreatFaeSpell = "skill_Midsummer's_Eve_Vivien_Gemai";
        public const string GreatFaeForm = "form_Great_Fairy_Vivien_Gemai";

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
                    Gender =  PvPStatics.GenderFemale,
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = Form,
                    Money = 1000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 25,
                    MembershipId = AIStatics.FaebossId.ToString(),
                    BotId = AIStatics.FaebossId,
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
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);
            List<Player> playersHere = GetEligibleTargetsInLocation(faeboss.dbLocationName, faeboss);

            foreach (Player p in playersHere)
            {
                string spell = ChooseSpell(p);
                AttackProcedures.Attack(faeboss, p, spell);
            }

        }

        public static void CounterAttack(Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);
            string spell = ChooseSpell(attacker);
            AttackProcedures.Attack(faeboss, attacker, spell);
        }

        public static string ChooseSpell(Player attacker)
        {
            if (attacker.Form != GreatFaeForm)
            {
                return FreatFaeSpell;
            }
            else if (attacker.Form == GreatFaeForm)
            {
                return "skill_Floral_Underlining_Passerby";
            }

            return "skill_Floral_Underlining_Passerby";
        }

        private static List<Player> GetEligibleTargetsInLocation(string location, Player player)
        {
            DateTime cutoff = DateTime.UtcNow.AddHours(-.5);
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(location).Where(m => m.Mobility == PvPStatics.MobilityFull &&
            m.Id != player.Id &&
            m.BotId >= AIStatics.PsychopathBotId &&
            m.LastActionTimestamp > cutoff &&
            m.InDuel <= 0 &&
            m.InQuest <= 0).ToList();

            return playersHere;
        }
    }
}
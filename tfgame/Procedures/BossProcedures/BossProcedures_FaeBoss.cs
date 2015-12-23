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

        public const string FirstName = "Narcissa";
        public const string LastName = "the Exiled";
        public const string Form = "form_Corrupted_Lunar_Fae_Roxanne246810(Rachael_Victor/Yuki_Kitsu)";
        public const string SpawnLocation = "fairygrove_greathall";

        public const string GreatFaeSpell = "skill_Midsummer's_Eve_Vivien_Gemai";
        public const string GreatFaeForm = "form_Great_Fairy_Vivien_Gemai";

        public const string DarkFaeSpell = "skill_Darkness_My_Old_Friend_Larissa_Fay";
        public const string DarkFaeForm = "form_Dark_Fae_Larissa_Fay";

        public const string EnchantedTreeSpell = "skill_Take_Root_Sherry_Gray";
        public const string EnchantedTreeForm = "form_Enchanted_Tree_Sherry_Gray";

        /// <summary>
        /// A list of all the animate spells Narcissa can cast
        /// </summary>
        public static readonly string[] animateSpellsToCast = { GreatFaeSpell, DarkFaeSpell, EnchantedTreeSpell };

        public const string FairyPetSpell = "skill_HEY!_LISTEN!_Varn";
        public const string FlowerSpell = ""; // SPELL PENDING
        public const string DarkFaePetSpell = ""; // SPELL PENDING

        /// <summary>
        /// A list of the inanimate and pet spells Narcissa can cast
        /// </summary>
        public static readonly string[] inanimateSpellsToCast = { FairyPetSpell };

        /// <summary>
        /// Probability of drawing Narcissa's aggro when she already has a target set
        /// </summary>
        private const double AggroChance = .2D;

        /// <summary>
        /// The base number of tiles Narcissa will move to catch up to her target or seek out new ones
        /// </summary>
        private const int MovementBaseDistance = 7;

        /// <summary>
        /// The random number of additional tiles Narcissa will move to catch up to her target or seek out new ones
        /// </summary>
        private const int MovementRandomExtraDistance = 6;

        /// <summary>
        /// Spawns Narcissa into the world and sets her initial blank AI Directive
        /// </summary>
        /// <returns></returns>
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

            AIDirective directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            return faeboss;
        }

        /// <summary>
        /// Returns whether the spell is valid against Narcissa.  TODO.
        /// </summary>
        /// <param name="spellName">db name of the spell whose cast is being attempted</param>
        /// <param name="caster">Player attempting to cast the spell</param>
        /// <returns></returns>
        public static bool SpellIsValid(string spellName, Player caster)
        {

            return false;
        }

        /// <summary>
        /// Perform Narcissa's regular actions when a new turn has started.  If Narcissa has no aggroed target, she seeks to transform random people into
        /// certain animate forms.  If she has aggro, she will attempt to chase them and cast a pet spell on them.  If she can't catch up, she'll cast the animate
        /// spells in the area instead and resume pursuit next turn.
        /// </summary>
        public static void RunTurnLogic()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);
            AIDirective directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            // no target, go out and hit some random people with animate spells
            if (HasValidTarget(directive) ==false)
            {
                ResetTarget(directive);
                string newTargetLocation = GetLocationWithMostEligibleTargets();
                string newActualLocation = AIProcedures.MoveTo(faeboss, newTargetLocation, GetRandomChaseDistance());
                faeboss.dbLocationName = newActualLocation;
                playerRepo.SavePlayer(faeboss);

                CastAnimateSpellsAtLocation(faeboss);
            }

            // Narcissa has a valid target, go for them
            else
            {
                Player target = PlayerProcedures.GetPlayer((int)directive.Var1);
                string newTargetLocation = target.dbLocationName;
                string newActualLocation = AIProcedures.MoveTo(faeboss, newTargetLocation, GetRandomChaseDistance());
                faeboss.dbLocationName = newActualLocation;
                playerRepo.SavePlayer(faeboss);

                if (faeboss.dbLocationName == target.dbLocationName)
                {
                    string spell = ChooseSpell(target, PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityPet);

                    for (int i = 0; i < 4; i++)
                    {
                        AttackProcedures.Attack(faeboss, target, spell);
                    }
                }
                else {
                    CastAnimateSpellsAtLocation(faeboss);
                }
            }

        }

        /// <summary>
        /// Attack an attacking player back.  There is a random chance to draw Narcissa's aggro from doing this if she has a target.  If she has no active target,
        /// the attacker instantly becomes her new target.
        /// </summary>
        /// <param name="attacker"></param>
        public static void CounterAttack(Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);
            string spell = ChooseSpell(attacker, PvPWorldStatProcedures.GetWorldTurnNumber(),PvPStatics.MobilityFull);
            AttackProcedures.Attack(faeboss, attacker, spell);

            AIDirective directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            // random chance to aggro faeboss

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble();

            if (num < AggroChance || directive.Var1==0)
            {
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective dbDirective = aiRepo.AIDirectives.FirstOrDefault(a => a.Id == directive.Id);

                dbDirective.Var1 = attacker.Id;
                aiRepo.SaveAIDirective(dbDirective);
            }
        }

        /// <summary>
        /// Calculate which spell for Narcissa to cast depending on the attacking player.  Narcissa will change spells every now and then based on the world turn
        /// number.
        /// </summary>
        /// <param name="attacker">The attacking player</param>
        /// <param name="turnNumber">World turn number</param>
        /// <param name="spellMobilityType">The spell type to cast with.  Narcissa targets neutral players with animate spells and her targets with inanimate/pet spells</param>
        /// <returns></returns>
        public static string ChooseSpell(Player attacker, int turnNumber, string spellMobilityType)
        {

            if (spellMobilityType==PvPStatics.MobilityFull)
            {
                int mod = turnNumber % animateSpellsToCast.Count();
                return animateSpellsToCast[mod];
            }

            return inanimateSpellsToCast[0];

        }

        /// <summary>
        /// Return a list of all the eligible players in a location for Narcissa to attack
        /// </summary>
        /// <param name="player">Narciss</param>
        /// <returns></returns>
        private static List<Player> GetEligibleTargetsInLocation(Player player)
        {
            DateTime cutoff = DateTime.UtcNow.AddMinutes(-30);
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(player.dbLocationName).Where(m => m.Mobility == PvPStatics.MobilityFull &&
            m.Id != player.Id &&
            m.BotId >= AIStatics.PsychopathBotId &&
            m.LastActionTimestamp > cutoff &&
            m.InDuel <= 0 &&
            m.InQuest <= 0).ToList();

            return playersHere;
        }

        /// <summary>
        /// Return the tile of the map which has the most targets that Narcissa is allowed to transform
        /// </summary>
        /// <returns>dbName of location found</returns>
        private static string GetLocationWithMostEligibleTargets()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            DateTime cutoff = DateTime.UtcNow.AddMinutes(-30);
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
            p.LastActionTimestamp > cutoff &&
            p.Form != GreatFaeForm &&
            p.dbLocationName.Contains("dungeon_") == false &&
            p.InDuel <= 0 &&
            p.InQuest <= 0).GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key);
            return locs.First();
        }

        /// <summary>
        /// Determines whether Narcissa is currently chasing someone who is valid to be transformed still
        /// </summary>
        /// <param name="directive">AI Directive containing target Id</param>
        /// <returns>True if target is valid, false if not</returns>
        private static bool HasValidTarget(AIDirective directive)
        {
            if (directive == null || directive.Var1 == 0)
            {
                return false;
            }

            Player target = PlayerProcedures.GetPlayer((int)directive.Var1);

            // TODO:  This can probably be swapped out with Arrhae's "CanBeAttacked" method in the future.  But for now...
            if (target == null ||
                        target.Mobility != PvPStatics.MobilityFull ||
                        PlayerProcedures.PlayerIsOffline(target) ||
                        target.IsInDungeon() == true ||
                        target.InDuel > 0 ||
                        target.InQuest > 0)
            {
                return false;
            }

            return true;
            
        }

        /// <summary>
        /// Clears Narciss'a target
        /// </summary>
        /// <param name="directive"></param>
        private static void ResetTarget(AIDirective directive)
        {
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective ai = aiRepo.AIDirectives.FirstOrDefault(a => a.Id == directive.Id);
            ai.Var1 = 0;
            aiRepo.SaveAIDirective(ai);
        }

        /// <summary>
        ///  Returns a number between 7 and 12 to give Narcissa some randomness on how far she is willing to chase a target in one turn.
        /// </summary>
        /// <returns>Random int between 7 and 12</returns>
        private static int GetRandomChaseDistance()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble()*6;
            return MovementBaseDistance + (int)MovementRandomExtraDistance;
        }

        /// <summary>
        /// Cast 1 animate spell on each player in Narcissa's current location.  She will not change her aggro for this.
        /// </summary>
        /// <param name="faeboss">Player casting the spells.  In this case, always Narcissa.</param>
        private static void CastAnimateSpellsAtLocation(Player faeboss)
        {
            List<Player> playersHere = GetEligibleTargetsInLocation(faeboss);

            foreach (Player p in playersHere)
            {
                string spell = ChooseSpell(p, PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityFull);
                AttackProcedures.Attack(faeboss, p, spell);
            }
        }

    }
}
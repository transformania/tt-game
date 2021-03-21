using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.World.Queries;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class BountyProcedures
    {
        private static readonly List<EffectSourceMapping> MAPPINGS = BountyEffects();

        private const int TURNS_OF_BOUNTY = 30;
        private const int TURNS_OF_IMMUNITY = 30;
        private const int BASE_REWARD = 60;
        private const int MAXIMUM_REWARD = 300;

        internal class EffectSourceMapping
        {
            public int EffectSourceId;
            public string Category;

            public EffectSourceMapping(int id, string category)
            {
                EffectSourceId = id;
                Category = category;
            }
        }

        private static List<EffectSourceMapping> BountyEffects()
        {
            var prefix = "effect_bounty_";
            IEffectRepository effectRepo = new EFEffectRepository();
            var effects = effectRepo.DbStaticEffects.Where(e => e.dbName.StartsWith(prefix)).ToList();

            var list = new List<EffectSourceMapping>();
            foreach (var effect in effects)
            {
                list.Add(new EffectSourceMapping(effect.Id, effect.dbName.Substring(prefix.Length)));
            }

            return list;
        }

        internal static int? PlaceBounty(Player player, Random rand = null)
        {
            // Only place bounties on PvP players
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }

            var numMappings = MAPPINGS.Count();
            if (numMappings == 0)
            {
                return null;
            }

            rand = rand ?? new Random();
            var effect = MAPPINGS[rand.Next(numMappings)];

            if (EffectProcedures.PlayerHasEffect(player, effect.EffectSourceId))
            {
                return null;
            }

            var possibleForms = JokeShopProcedures.STABLE_FORMS.Where(e => e.Category == effect.Category).Select(e => e.FormSourceId).ToArray();
            var numForms = possibleForms.Count();

            if (numForms == 0)
            {
                return null;
            }

            EffectProcedures.GivePerkToPlayer(effect.EffectSourceId, player.Id, TURNS_OF_BOUNTY, TURNS_OF_BOUNTY + TURNS_OF_IMMUNITY);

            return effect.EffectSourceId;
        }

        internal static BountyInfo BountyDetails(Player player, int effectId)
        {
            var effect = MAPPINGS.FirstOrDefault(e => e.EffectSourceId == effectId);

            if (effect == null || !EffectProcedures.PlayerHasEffect(player, effect.EffectSourceId))
            {
                return null;
            }

            var possibleFormSourceIds = JokeShopProcedures.STABLE_FORMS.Where(e => e.Category == effect.Category).Select(e => e.FormSourceId).ToArray();
            var numFormSourceIds = possibleFormSourceIds.Count();

            if (numFormSourceIds == 0)
            {
                return null;
            }

            var playerEffect = EffectProcedures.GetPlayerEffects2(player.Id).FirstOrDefault(e => e.dbEffect.EffectSourceId == effect.EffectSourceId);

            if (playerEffect == null || playerEffect.dbEffect.Duration == 0)
            {
                return null;
            }

            var duration = playerEffect.dbEffect.Duration;

            if (duration <= 0)
            {
                return null;
            }

            var turn = PvPStatics.LastGameTurn;

            if (turn == 0)
            {
                // Check server hasn't been restarted mid-game (in which case PvPStatics.LastGameTurn is not accurate)
                var worldStats = DomainRegistry.Repository.FindSingle(new GetWorld());
                turn = worldStats.TurnNumber;
            }

            var expiresTurn = turn + duration;
            var formIndex = player.Id + expiresTurn;
            var formSourceId = possibleFormSourceIds[formIndex % possibleFormSourceIds.Count()];

            // Locate the desired form
            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var form = formsRepo.DbStaticForms.FirstOrDefault(f => f.Id == formSourceId);

            if (form == null)
            {
                return null;
            }

            // Calculate the reward that could be claimed right now
            var reward = Math.Min(MAXIMUM_REWARD, BASE_REWARD + (MAXIMUM_REWARD - BASE_REWARD) * duration / TURNS_OF_BOUNTY);

            if (form.MobilityType == PvPStatics.MobilityFull)
            {
                reward /= 2;
            }

            var category = MAPPINGS.Where(m => m.EffectSourceId == effect.EffectSourceId).Select(m => m.Category).FirstOrDefault();

            return new BountyInfo { PlayerName = player.GetFullName(), Form = form, ExpiresTurn = expiresTurn, CurrentReward = reward, Category = category };
        }

        public static IEnumerable<BountyInfo> OutstandingBounties()
        {
            var bountyStaticEffectIds = MAPPINGS.Select(se => se.EffectSourceId);
            IEffectRepository effectRepo = new EFEffectRepository();
            var effects = effectRepo.Effects.Where(e => bountyStaticEffectIds.Contains(e.EffectSourceId));

            var results = new List<BountyInfo>();

            foreach (var effect in effects)
            {
                var entry = BountyDetails(PlayerProcedures.GetPlayer(effect.OwnerId), effect.EffectSourceId);

                if (entry != null)
                {
                    results.Add(entry);
                }
            }

            return results;
        }

        public static void ClaimReward(Player attacker, Player victim, DbStaticForm victimNewForm)
        {
            if (attacker == null || victim == null || victimNewForm == null)
            {
                return;
            }

            // Ensure victim is still in PvP, or not a player (in case we want to support bounties on NPCs)
            if (victim.BotId == AIStatics.ActivePlayerBotId && victim.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return;
            }

            // NPCs are not eligible to claim bounties
            if (attacker.BotId != AIStatics.ActivePlayerBotId)
            {
                return;
            }

            var bountyStaticEffectIds = MAPPINGS.Select(se => se.EffectSourceId);
            var victimEffects = EffectProcedures.GetPlayerEffects2(victim.Id).Where(e => bountyStaticEffectIds.Contains(e.dbEffect.EffectSourceId));
            var award = 0;

            foreach (var victimEffect in victimEffects)
            {
                var bounty = BountyDetails(victim, victimEffect.dbEffect.EffectSourceId);

                if (bounty == null)
                {
                    continue;
                }

                if (victimNewForm.Id == bounty.Form?.Id)
                {
                    // Victim has been turned into the requested form - full reward
                    award = bounty.CurrentReward;
                }
                else if (victimNewForm.ItemSourceId.HasValue)
                {
                    // Award half bounty for a different inanimate form of the same type
                    IDbStaticItemRepository itemsRepo = new EFDbStaticItemRepository();
                    var itemForm = itemsRepo.DbStaticItems.FirstOrDefault(i => i.Id == victimNewForm.ItemSourceId.Value);

                    if (itemForm?.ItemType == bounty.Category)
                    {
                        award = bounty.CurrentReward / 2;
                    }
                }

                if (award > 0)
                {
                    // Release the victim from the claimed bounty
                    EffectProcedures.SetPerkDurationToZero(victimEffect.dbEffect.EffectSourceId, victim);

                    // Award bounty funds to attacker
                    PlayerLogProcedures.AddPlayerLog(attacker.Id, $"For turning {victim.GetFullName()} into a {victimNewForm.FriendlyName} you claim a bounty of {award} arpeyjis.", true);
                    PlayerProcedures.GiveMoneyToPlayer(attacker, award);

                    StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__BountiesClaimed, award);

                    break;
                }
            }
        }

    }
}

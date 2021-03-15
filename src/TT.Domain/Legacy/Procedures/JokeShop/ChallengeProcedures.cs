using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.World.Queries;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public class Challenge
    {
        public Func<Player, bool> Eligible = p => true;

        public List<String> Criteria = new List<String>();
        public Func<Player, bool> Satisfied = p => true;

        public Func<string, int> ResourceUsed = p => 0;

        public int ByEndOfTurn = 0;

        public String Reward = "";
        public Action<Player> GiveReward = p => { };

        public String Penalty = "";
        public Action<Player> GivePenalty = p => { };

        public int Parts = 0;
        public int Difficulty = 0;
    }

    class ChallengeType
    {
        public int EffectSourceId;
        public int Duration;
        public int MaxParts;
        public int MaxDifficulty;
        public bool Penalty;
    }

    public static class ChallengeProcedures
    {
        // Resources allow us to avoid setting an impossible challenge, e.g. be in two form at the same time
        const String RESOURCE_FORM = "form";

        static List<ChallengeType> ChallengeTypes = null;

        internal static List<ChallengeType> CHALLENGE_TYPES
        {
            get
            {
                if (ChallengeTypes == null)
                {
                    ChallengeTypes = PopulateChallengeTypes() ?? new List<ChallengeType>();
                }

                return ChallengeTypes;
            }
            set
            {
                ChallengeTypes = value;
            }
        }

        static List<ChallengeType> PopulateChallengeTypes()
        {
            var output = new List<ChallengeType>();

            var effectSource = JokeShopProcedures.EffectWithName("effect_challenged_1");

            if (effectSource.HasValue)
            {
                output.Add(new ChallengeType{
                    EffectSourceId = effectSource.Value,
                    Duration = 5,
                    MaxParts = 1,
                    MaxDifficulty = 1,
                    Penalty = false,
                });
            }

            effectSource = JokeShopProcedures.EffectWithName("effect_challenged_2");

            if (effectSource.HasValue)
            {
                output.Add(new ChallengeType{
                    EffectSourceId = effectSource.Value,
                    Duration = 10,
                    MaxParts = 2,
                    MaxDifficulty = 4,
                    Penalty = false,
                });
            }

            effectSource = JokeShopProcedures.EffectWithName("effect_challenged_3");

            if (effectSource.HasValue)
            {
                output.Add(new ChallengeType{
                    EffectSourceId = effectSource.Value,
                    Duration = 20,
                    MaxParts = 3,
                    MaxDifficulty = 8,
                    Penalty = true,
                });
            }

            effectSource = JokeShopProcedures.EffectWithName("effect_challenged_4");

            if (effectSource.HasValue)
            {
                output.Add(new ChallengeType{
                    EffectSourceId = effectSource.Value,
                    Duration = 120,
                    MaxParts = 1,
                    MaxDifficulty = 20,
                    Penalty = true,
                });
            }

            // Try more difficult ones first, fall back on easier challenges
            output.Reverse();

            return output;
        }


        // Tries to give a player a challenge
        public static Challenge AwardChallenge(Player player, int minDuration, int maxDuration)
        {
            // Only allow one challenge to be ative at a time
            if (CurrentChallenge(player) != null)
            {
                return null;
            }

            foreach (var challengeType in CHALLENGE_TYPES.Where(c => c.Duration <= maxDuration && c.Duration >= minDuration))
            {
                var expiresTurnEnd = NewChallengeExpires(challengeType);
                var die = LoadedDie(player.Id, challengeType, expiresTurnEnd);
                var challenge = FormulateChallenge(challengeType, expiresTurnEnd, die);

                if (challenge.Eligible(player))
                {
                    EffectProcedures.GivePerkToPlayer(challengeType.EffectSourceId, player.Id, challengeType.Duration);

                    if (EffectProcedures.PlayerHasActiveEffect(player.Id, challengeType.EffectSourceId))
                    {
                        return challenge;
                    }
                }
            }

            return null;
        }

        // Returns the current challenge for a player, if any
        public static Challenge CurrentChallenge(Player player)
        {
            var effect = CurrentChallengeEffect(player);

            if (effect == null)
            {
                return null;
            }

            return CurrentChallenge(player, effect);
        }

        public static Challenge CurrentChallenge(Player player, Effect_VM effect)
        {
            var challengeType = CHALLENGE_TYPES.FirstOrDefault(c => c.EffectSourceId == effect.EffectSourceId);

            if (challengeType == null)
            {
                return null;
            }

            var expiresTurnEnd = ChallengeExpires(effect);
            var die = LoadedDie(player.Id, challengeType, expiresTurnEnd);

            return FormulateChallenge(challengeType, expiresTurnEnd, die);
        }

        // Tests whether the challenge is complete and make reward/punishment
        public static void CheckChallenge(Player player, bool expiringPhase)
        {
            var effect = CurrentChallengeEffect(player);

            if (effect == null)
            {
                return;
            }

            // Players in joke shop on cooldown shouldn't get a double hit
            if (!expiringPhase && effect.Duration == 0)
            {
                return;
            }

            var challenge = CurrentChallenge(player, effect);

            if (challenge == null)
            {
                return;
            }

            if (challenge.Satisfied(player))
            {
                challenge.GiveReward(player);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>Congratulations!</b>  You have passed a challenge and earn a reward of <b>{challenge.Reward}</b>!", true);
                EffectProcedures.SetPerkDurationToZero(effect.EffectSourceId, player);

                // Don't delete if something else is about to try to remove the effect
                if (effect.Duration != 0)
                {
                    StatsProcedures.AddStat(player.MembershipId, StatsProcedures.Stat__ChallengesPassed, 1);
                }
            }
            else if(effect.Duration == 0)
            {
                if (!challenge.Penalty.IsNullOrEmpty())
                {
                    challenge.GivePenalty(player);
                    PlayerLogProcedures.AddPlayerLog(player.Id, $"You have <b>failed</b> your recent challenge and are given a penalty of <b>{challenge.Reward}</b>!", true);
                }

                // The effect has probably cleared, but let's be certain
                //EffectProcedures.RemovePerkFromPlayer(effect.EffectSourceId, player);
            }
        }

        // Check people in joke shop - this will mostly be people who have potentially passed a challenge
        public static void CheckChallenges()
        {
            var playerRepo = new EFPlayerRepository();
            IEnumerable<Player> players = playerRepo.Players.Where(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                                                                        p.Mobility == PvPStatics.MobilityFull &&
                                                                        p.BotId == AIStatics.ActivePlayerBotId);

            foreach (var player in players)
            {
                CheckChallenge(player, false);
            }
        }

        // Check people anywhere with expiring and likely failed challenge
        public static void CheckExpiringChallenges(List<Effect> effects)
        {
            var challengeEffects = effects.Where(e => CHALLENGE_TYPES.Select(c => c.EffectSourceId).Contains(e.EffectSourceId));

            foreach (var expiringEffect in challengeEffects)
            {
                CheckChallenge(PlayerProcedures.GetPlayer(expiringEffect.OwnerId), true);
            }
        }

        // Important:  The selected challenge must depend entirely on stable environmental
        // factors, such as player ID, turn the challenge expires on and length of challenge,
        // and information derived from that (e.g. earlier parts of the challenge).
        // This allows us to repeatably and reliably determine what a challenge is without
        // holding further state about it.  If the challenge is not possible then it is not
        // given to the player.
        private static Random LoadedDie(int playerId, ChallengeType challengeType, int turnChallengeExpires)
        {
            return new Random(playerId + challengeType.Duration + turnChallengeExpires);
        }

        private static int TurnNumber()
        {
            var turn = PvPStatics.LastGameTurn;

            if (turn == 0)
            {
                // Check server hasn't been restarted mid-game (in which case PvPStatics.LastGameTurn is not accurate)
                var worldStats = DomainRegistry.Repository.FindSingle(new GetWorld());
                turn = worldStats.TurnNumber;
            }

            return turn;
        }
        private static int ChallengeExpires(Effect_VM effect)
        {
            return TurnOfExpiry(effect.Duration);
        }

        private static int NewChallengeExpires(ChallengeType challengeType)
        {
            return TurnOfExpiry(challengeType.Duration);
        }

        private static int TurnOfExpiry(int turnsFromNow)
        {
            // Effect duration is 1 on its last turn of effect, so take away 1 to
            // calculate 'expires at end of turn' rather than 'expires at start of turn'
            return TurnNumber() + turnsFromNow  - 1;
        }

        private static Effect_VM CurrentChallengeEffect(Player player)
        {
            var effects = EffectProcedures.GetPlayerEffects2(player.Id);
            return effects.FirstOrDefault(e => CHALLENGE_TYPES.Select(c => c.EffectSourceId).Contains(e.dbEffect.EffectSourceId))?.dbEffect;
        }

        // The following methods avoid self-recursive closures, preventing stack overflows

        private static void AddEligibilityCriterion(Challenge challenge, Func<Player, bool> criterion)
        {
            var local = challenge.Eligible;
            challenge.Eligible = p => local(p) && criterion(p);
        }

        private static void AddRequirement(Challenge challenge, String description, Func<Player, bool> test)
        {
            var local = challenge.Satisfied;
            challenge.Satisfied = p => local(p) && test(p);
            challenge.Criteria.Add(description);
        }

        private static void AddResourceContribution(Challenge challenge, Func<String, int> contribution)
        {
            var local = challenge.ResourceUsed;
            challenge.ResourceUsed = r => local(r) + contribution(r);
        }

        private static void AddReward(Challenge challenge, String description, Action<Player> rewardGiver)
        {
            var local = challenge.GiveReward;
            challenge.GiveReward = p => { local(p); rewardGiver(p); };

            if (challenge.Reward.IsNullOrEmpty())
            {
                challenge.Reward = description;
            }
            else
            {
                challenge.Reward += " and " + description;
            }
        }

        private static void AddPenalty(Challenge challenge, String description, Action<Player> penaltyGiver)
        {
            var local = challenge.GivePenalty;
            challenge.GivePenalty = p => { local(p); penaltyGiver(p); };

            if (challenge.Penalty.IsNullOrEmpty())
            {
                challenge.Penalty = description;
            }
            else
            {
                challenge.Penalty += " and " + description;
            }
        }


        private static Challenge FormulateChallenge(ChallengeType challengeType, int expires, Random die)
        {
            var challenge = new Challenge();
            challenge.ByEndOfTurn = expires;

            var attempts = 10;

            // Build up a multi-stage challenge that best fits the requested type
            while (attempts-- > 0 &&
                   challenge.Parts < challengeType.MaxParts &&
                   challenge.Difficulty < challengeType.MaxDifficulty)
            {
                // Pick between different challenges based on roll of die
                TryAddingAnimateFormRequirement(challengeType, die, challenge);
            }

            if (challenge.Parts == 0)
            {
                return null;
            }

            PickRewardsAndPenalties(challengeType, die, challenge);

            return challenge;
        }

        private static void PickRewardsAndPenalties(ChallengeType challengeType, Random die, Challenge challenge)
        {
            AddReward(challenge, "50 Arpeyjis",
                      p => { PlayerProcedures.GiveMoneyToPlayer(p, 50); });

            if (challengeType.Penalty)
            {
                AddPenalty(challenge, "50 Arpeyjis",
                           p => { PlayerProcedures.GiveMoneyToPlayer(p, -50); });
            }
        }

        private static void TryAddingAnimateFormRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_FORM) == 0)
            {
                var mobileForms = JokeShopProcedures.Forms(f => f.Category == PvPStatics.MobilityFull);
                var form = mobileForms.ElementAt(die.Next(mobileForms.Count()));

                AddEligibilityCriterion(challenge,
                                        p => p.FormSourceId != form.FormSourceId);
                AddRequirement(challenge,
                               $"Take on the form of a {form.FriendlyName}",
                               p => p.FormSourceId == form.FormSourceId);
                AddResourceContribution(challenge, r => (r == RESOURCE_FORM) ? 1 : 0);

                challenge.Parts++;
                challenge.Difficulty += difficulty;
            }
        }
    }
}

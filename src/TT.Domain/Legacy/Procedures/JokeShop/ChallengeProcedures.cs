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
        // Eligibility implicitly includes that the player has not already satisfied the whole challenge.
        // Include, for example, game mode or irreversible achievements e.g. min soulbound items.
        // Do not include things a player might have but could lose, e.g. form or time out of combat.
        public Func<Player, bool> Eligible = p => true;

        public List<ChallengePart> Parts = new List<ChallengePart>();

        public Func<string, int> ResourceUsed = p => 0;

        public int ByEndOfTurn = 0;

        public String Reward = "";
        public Action<Player> GiveReward = p => { };

        public String Penalty = "";
        public Action<Player> GivePenalty = p => { };

        public int Difficulty = 0;

        public bool Satisfied(Player player)
        {
            return Parts.All(p => p.Satisfied(player));
        }

        public string GetTimeLeft()
        {
            var turnsLeft = ByEndOfTurn - PvPWorldStatProcedures.GetWorldTurnNumber();
            var minutesLeft = TurnTimesStatics.GetTurnLengthInSeconds() * turnsLeft / 60;
            return (minutesLeft < 60) ? $"{minutesLeft} minutes" : $"{(minutesLeft + 3) / 60} hours";
        }
    }

    public class ChallengePart
    {
        public string Description;
        public Func<Player, (int, int)> Progress;
        public Func<Player, bool> Satisfied;
        public Func<Player, string> Status;
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
        const String RESOURCE_AP = "action points";
        const String RESOURCE_COMBAT_TIMER = "combat timer";
        const String RESOURCE_EQUIP = "equip";
        const String RESOURCE_FORM = "form";
        const String RESOURCE_HEALTH = "health";
        const String RESOURCE_LEVEL = "level";
        const String RESOURCE_MANA = "mana";
        const String RESOURCE_STAT = "stat";
        const String RESOURCE_STAT_DISCIPLINE = "stat discipline";
        const String RESOURCE_STAT_PERCEPTION = "stat perception";
        const String RESOURCE_STAT_CHARISMA = "stat charisma";
        const String RESOURCE_STAT_FORTITUDE = "stat fortitude";
        const String RESOURCE_STAT_AGILITY = "stat agility";
        const String RESOURCE_STAT_ALLURE = "stat allure";
        const String RESOURCE_STAT_MAGICKA = "stat magicka";
        const String RESOURCE_STAT_SUCCOUR = "stat succour";
        const String RESOURCE_STAT_LUCK = "stat luck";

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
            var output = new List<ChallengeType>
            {
                new ChallengeType
                {
                    EffectSourceId = 220,  // Challenged I
                    Duration = 5,
                    MaxParts = 1,
                    MaxDifficulty = 2,
                    Penalty = false,
                },

                new ChallengeType
                {
                    EffectSourceId = 221,  // Challenged II
                    Duration = 10,
                    MaxParts = 2,
                    MaxDifficulty = 5,
                    Penalty = false,
                },

                new ChallengeType
                {
                    EffectSourceId = 223,  // Challenged III
                    Duration = 20,
                    MaxParts = 3,
                    MaxDifficulty = 8,
                    Penalty = true,
                },

                new ChallengeType
                {
                    EffectSourceId = 224,  // Challenged IV
                    Duration = 60,
                    MaxParts = 3,
                    MaxDifficulty = 13,
                    Penalty = false,
                },

                new ChallengeType
                {
                    EffectSourceId = 225,  // Challenged V
                    Duration = 120,
                    MaxParts = 3,
                    MaxDifficulty = 21,
                    Penalty = true,
                }
            };

            return output;
        }


        // Tries to give a player a challenge
        public static Challenge AwardChallenge(Player player, int minDuration, int maxDuration, bool? withPenalties)
        {
            // Only allow one challenge to be ative at a time
            if (CurrentChallenge(player) != null)
            {
                return null;
            }

            var challengeTypes = CHALLENGE_TYPES.Where(c => c.Duration <= maxDuration && c.Duration >= minDuration);

            if (withPenalties.HasValue)
            {
                challengeTypes = challengeTypes.Where(c => c.Penalty == withPenalties.Value);
            }

            Challenge bestChallenge = null;
            ChallengeType bestChallengeType = null;

            // Find an appropriate challenge
            foreach (var challengeType in challengeTypes)
            {
                var expiresTurnEnd = NewChallengeExpires(challengeType);
                var die = LoadedDie(player.Id, challengeType, expiresTurnEnd);
                var challenge = DeviseChallenge(challengeType, expiresTurnEnd, die);

                if (challenge != null)
                {
                    // Try to pick challenge closest to challenge type's requirements
                    if (bestChallenge == null || challenge.Difficulty > bestChallenge.Difficulty ||
                        (challenge.Difficulty == bestChallenge.Difficulty && 
                         challengeType.MaxDifficulty - challenge.Difficulty < bestChallengeType.MaxDifficulty - bestChallenge.Difficulty))
                    {
                        // Confirm player is eligible for challenge and hasn't already satisfied its requirements
                        if (challenge.Eligible(player) && !challenge.Satisfied(player))
                        {
                            bestChallenge = challenge;
                            bestChallengeType = challengeType;
                        }
                    }
                }
            }

            // Assign the challenge to the player
            if (bestChallenge != null)
            {
                    EffectProcedures.GivePerkToPlayer(bestChallengeType.EffectSourceId, player.Id, bestChallengeType.Duration);

                    if (EffectProcedures.PlayerHasActiveEffect(player.Id, bestChallengeType.EffectSourceId))
                    {
                        return bestChallenge;
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

            return DeviseChallenge(challengeType, expiresTurnEnd, die);
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

            if (player.dbLocationName == LocationsStatics.JOKE_SHOP && challenge.Satisfied(player))
            {
                challenge.GiveReward(player);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>Congratulations!</b>  You have passed a challenge and earn a reward of <b>{challenge.Reward}</b>!", true);
                EffectProcedures.SetPerkDurationToZero(effect.EffectSourceId, player);
                StatsProcedures.AddStat(player.MembershipId, StatsProcedures.Stat__ChallengesPassed, 1);
            }
            else if (effect.Duration == 0 && !challenge.Penalty.IsNullOrEmpty())
            {
                challenge.GivePenalty(player);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have <b>failed</b> your recent challenge and are given a penalty of <b>{challenge.Penalty}</b>!", true);
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
            return PvPWorldStatProcedures.GetWorldTurnNumber() + turnsFromNow - 1;
        }

        private static Effect_VM CurrentChallengeEffect(Player player)
        {
            var effects = EffectProcedures.GetPlayerEffects2(player.Id);
            return effects.FirstOrDefault(e => CHALLENGE_TYPES.Select(c => c.EffectSourceId).Contains(e.dbEffect.EffectSourceId))?.dbEffect;
        }

        // Add eligibility criteria for a player to be awarded a challenge, to be evaluated after that challenge is devised
        private static void AddPrerequisite(Challenge challenge, Func<Player, bool> prerequisite)
        {
            // Capture local to avoid self-recursive closure, preventing stack overflows through self-capture
            var local = challenge.Eligible;
            challenge.Eligible = p => local(p) && prerequisite(p);
        }

        private static void AddPart(Challenge challenge,
                                    String description,
                                    Func<Player, bool> satisfied,
                                    Func<Player, (int, int)> progress = null,
                                    Func<Player, string> status = null)
        {
            challenge.Parts.Add(new ChallengePart
            {
                Description = description,
                Satisfied = satisfied,
                Progress = progress ?? (p => (satisfied(p) ? 1 : 0, 1)),
                Status = status ?? (p => satisfied(p) ? "Done" : "Not done")
            });
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


        private static Challenge DeviseChallenge(ChallengeType challengeType, int expires, Random die)
        {
            var challenge = new Challenge();
            challenge.ByEndOfTurn = expires;

            var attempts = 10;

            // Build up a multi-stage challenge that best fits the requested type
            while (attempts-- > 0 &&
                   challenge.Parts.Count() < challengeType.MaxParts &&
                   challenge.Difficulty < challengeType.MaxDifficulty)
            {
                // Pick between different challenges based on roll of die (which is repeatable due to the RNG seed being predictable)
                var roll = die.Next(100);

                if (roll < 14)  // 14%
                {
                    TryAddingAnimateFormRequirement(challengeType, die, challenge);
                }
                else if (roll < 18)  // 4%
                {
                    TryAddingImmobileFormRequirement(challengeType, die, challenge);
                }
                else if (roll < 30)  // 12%
                {
                    TryAddingEquipmentRequirement(challengeType, die, challenge);
                }
                else if (roll < 45)  // 15%
                {
                    TryAddingStatRequirement(challengeType, die, challenge);
                }
                else if (roll < 60)  // 15%
                {
                    TryAddingAchievementRequirement(challengeType, die, challenge);
                }
                else if (roll < 78)  // 18%
                {
                    TryAddingNonConsumableItemRequirement(challengeType, die, challenge);
                }
                else if (roll < 81)  // 3%
                {
                    TryAddingCombatRequirement(challengeType, die, challenge);
                }
                else if (roll < 84)  // 3%
                {
                    TryAddingHealthRequirement(challengeType, die, challenge);
                }
                else if (roll < 87)  // 3%
                {
                    TryAddingManaRequirement(challengeType, die, challenge);
                }
                else if (roll < 90)  // 3%
                {
                    TryAddingAPRequirement(challengeType, die, challenge);
                }
                else if (roll < 95)  // 5%
                {
                    TryAddingLevelRequirement(challengeType, die, challenge);
                }
                else  // 5%
                {
                    TryAddingQuestRequirement(challengeType, die, challenge);
                }
            }

            if (challenge.Parts.Count() == 0)
            {
                return null;
            }

            PickRewardsAndPenalties(challengeType, die, challenge);

            return challenge;
        }

        private static void PickRewardsAndPenalties(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var roll = die.Next(100);

            if (roll < 25)
            {
                var amount = (int)Math.Max(challenge.Difficulty * 10, 50);
                AddReward(challenge, $"{amount} Arpeyjis", p => { PlayerProcedures.GiveMoneyToPlayer(p, amount); });
            }
            else if (roll < 50)
            {
                var amount = Math.Max(1, challenge.Difficulty / 5 + 1);
                AddReward(challenge, $"{amount} spells", p => { SkillProcedures.GiveRandomFindableSkillsToPlayer(p, amount); });
            }
            else if (roll < 75)
            {
                AddReward(challenge, $"an effect to boost your skills", p => { CharacterPrankProcedures.GiveRandomEffect(p, CharacterPrankProcedures.BOOST_EFFECTS, die); });
            }
            else
            {
                AddReward(challenge, $"a random item", p => { EnvironmentPrankProcedures.RareFind(p, die); });
            }

            if (challengeType.Penalty)
            {
                roll = die.Next(100);

                if (roll < 75)
                {
                    var amount = (int)Math.Max(challenge.Difficulty * 15, 100);
                    AddPenalty(challenge, $"{amount} Arpeyjis", p => { PlayerProcedures.GiveMoneyToPlayer(p, -Math.Min(amount, p.Money)); });
                }
                else
                {
                    AddPenalty(challenge, $"a penalty effect", p => { CharacterPrankProcedures.GiveRandomEffect(p, CharacterPrankProcedures.PENALTY_EFFECTS, die); });
                }
            }
        }

        private static void TryAddingAnimateFormRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 2;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_FORM) == 0)
            {
                var mobileForms = JokeShopProcedures.Forms(f => f.Category == PvPStatics.MobilityFull);
                var form = mobileForms.ElementAt(die.Next(mobileForms.Count()));

                AddPart(challenge,
                        $"Take on the form of a {form.FriendlyName}",
                        p => p.FormSourceId == form.FormSourceId);
                AddResourceContribution(challenge, r => (r == RESOURCE_FORM) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingImmobileFormRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 3;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_FORM) == 0)
            {
                var immobileForms = JokeShopProcedures.Forms(f => f.Category == JokeShopProcedures.LIMITED_MOBILITY);
                var form = immobileForms.ElementAt(die.Next(immobileForms.Count()));

                AddPart(challenge,
                        $"Take on the form of a {form.FriendlyName}",
                        p => p.FormSourceId == form.FormSourceId);
                AddResourceContribution(challenge, r => (r == RESOURCE_FORM) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingCombatRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_COMBAT_TIMER) == 0)
            {
                var minMinutesOutOfCombat = 15;
                var maxMinutesOutOfCombat = Math.Min(30, challengeType.Duration * TurnTimesStatics.GetTurnLengthInSeconds() / 60 / 2);
                maxMinutesOutOfCombat = Math.Max(minMinutesOutOfCombat, Math.Min(30, maxMinutesOutOfCombat));

                var minutesToStayOutOfCombat = (int)die.Next(minMinutesOutOfCombat, maxMinutesOutOfCombat + 1);

                AddPart(challenge,
                        $"Stay out of combat for {minutesToStayOutOfCombat} minutes",
                        p => p.LastCombatTimestamp.AddMinutes(minutesToStayOutOfCombat) <= DateTime.UtcNow,
                        p => ((int)DateTime.UtcNow.Subtract(p.LastCombatTimestamp).TotalMinutes, minutesToStayOutOfCombat));
                AddResourceContribution(challenge, r => (r == RESOURCE_COMBAT_TIMER) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingHealthRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_HEALTH) == 0)
            {
                AddPart(challenge,
                        $"Cleanse to full willpower",
                        p => p.Health == p.MaxHealth,
                        p => ((int)(p.Health / p.MaxHealth * 100), 100));
                AddResourceContribution(challenge, r => (r == RESOURCE_HEALTH) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingManaRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_MANA) == 0)
            {
                AddPart(challenge,
                        $"Meditate to full mana",
                        p => p.Mana == p.MaxMana,
                        p => ((int)(p.Mana / p.MaxMana * 100), 100));
                AddResourceContribution(challenge, r => (r == RESOURCE_MANA) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingAPRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 2;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_AP) == 0)
            {
                var target = (int)Math.Min(challengeType.Duration * 10 * 3 / 5, TurnTimesStatics.GetActionPointLimit());

                AddPart(challenge,
                        $"Build up {target} Action Points (not including reserves)",
                        p => p.ActionPoints >= target,
                        p => ((int)(p.ActionPoints / (decimal)target * 100), 100));
                AddResourceContribution(challenge, r => (r == RESOURCE_AP) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingLevelRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 4;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_LEVEL) == 0)
            {
                var low = Math.Min(7, 2 + 5 * challenge.ByEndOfTurn / 2000);
                var high = Math.Min(12, 5 + 7 * challenge.ByEndOfTurn / 2000);
                var target = (int)die.Next(low, high + 1);

                AddPrerequisite(challenge, p => p.Level >= target - 2);

                AddPart(challenge,
                        $"Reach level {target}",
                        p => p.Level >= target,
                        p => (p.Level, target));
                AddResourceContribution(challenge, r => (r == RESOURCE_LEVEL) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingEquipmentRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 2;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_EQUIP) == 0)
            {
                var low = Math.Min(6, 1 + challenge.ByEndOfTurn/ 2000);
                var high = Math.Min(12, 2 + 4 * challenge.ByEndOfTurn / 2000);
                var target = die.Next(low, high + 1);

                int itemsEquipped(Player p) => ItemProcedures.GetAllPlayerItems(p.Id)
                                                             .Where(i => i.Item.ItemType != PvPStatics.ItemType_Consumable &&
                                                                         i.dbItem.IsEquipped).Count();

                AddPart(challenge,
                        $"Equip at least {target} items in total (not including single-use consumables)",
                        p => itemsEquipped(p) >= target,
                        p => (itemsEquipped(p), target));
                AddResourceContribution(challenge, r => (r == RESOURCE_EQUIP) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingStatRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 3;

            var maxStatTotal = Math.Min(250, 50 + challenge.ByEndOfTurn / 40);

            var low = 30;
            var high = Math.Min(120, 30 + 30 * challenge.ByEndOfTurn / 2000);
            var target = (int)die.Next(low, high + 1);

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_STAT) + target < maxStatTotal)
            {
                Func<Player, int> stat = null;
                string statName = null;

                var success = true;
                var roll = die.Next(9);

                if (roll == 0 && challenge.ResourceUsed(RESOURCE_STAT_DISCIPLINE) == 0)
                {
                    statName = "Discipline";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Discipline();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_DISCIPLINE) ? target : 0);
                }
                else if (roll == 1 && challenge.ResourceUsed(RESOURCE_STAT_PERCEPTION) == 0)
                {
                    statName = "Perception";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Perception();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_PERCEPTION) ? target : 0);
                }
                else if (roll == 2 && challenge.ResourceUsed(RESOURCE_STAT_CHARISMA) == 0)
                {
                    statName = "Charisma";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Charisma();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_CHARISMA) ? target : 0);
                }
                else if (roll == 3 && challenge.ResourceUsed(RESOURCE_STAT_FORTITUDE) == 0)
                {
                    statName = "Fortitude";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Fortitude();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_FORTITUDE) ? target : 0);
                }
                else if (roll == 4 && challenge.ResourceUsed(RESOURCE_STAT_AGILITY) == 0)
                {
                    statName = "Agility";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Agility();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_AGILITY) ? target : 0);
                }
                else if (roll == 5 && challenge.ResourceUsed(RESOURCE_STAT_ALLURE) == 0)
                {
                    statName = "Restoration";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Allure();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_ALLURE) ? target : 0);
                }
                else if (roll == 6 && challenge.ResourceUsed(RESOURCE_STAT_MAGICKA) == 0)
                {
                    statName = "Magicka";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Magicka();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_MAGICKA) ? target : 0);
                }
                else if (roll == 7 && challenge.ResourceUsed(RESOURCE_STAT_SUCCOUR) == 0)
                {
                    statName = "Regeneration";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Succour();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_SUCCOUR) ? target : 0);
                }
                else if (roll == 8 && challenge.ResourceUsed(RESOURCE_STAT_LUCK) == 0)
                {
                    statName = "Luck";
                    stat = p => (int)ItemProcedures.GetPlayerBuffs(p).Luck();
                    AddResourceContribution(challenge, r => (r == RESOURCE_STAT || r == RESOURCE_STAT_LUCK) ? target : 0);
                }
                else
                {
                    success = false;
                }

                if (success)
                {
                    AddPart(challenge, $"Make your {statName} stat {target} or higher",
                            p => stat(p) >= target,
                            p => (stat(p), target));
                    challenge.Difficulty += difficulty;
                }
            }
        }

        private static void TryAddingQuestRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 5;

            var quests = QuestProcedures.GetAllQuestStarts()
                                        .Where(q => q.Location != "x")  // filter out Welcome to Sunnyglade quest - only avaiable on game start
                                        .OrderBy(q =>q.Id);
            var numQuests = quests.Count();

            if (numQuests == 0)
            {
                return;
            }

            var quest = quests.ElementAt(die.Next(numQuests));
            var resource = $"quest {quest.Id}";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(resource) == 0)
            {
                AddPrerequisite(challenge, p => QuestProcedures.GetAllAvailableQuestsForPlayer(p, challenge.ByEndOfTurn - challengeType.Duration)
                                                                       .Any(q => q.Id == quest.Id));

                AddPart(challenge,
                        $"Pass the \"{quest.Name}\" quest",
                        p => QuestProcedures.PlayerHasCompletedQuest(p, quest.Id));
                AddResourceContribution(challenge, r => (r == resource) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingNonConsumableItemRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 3;

            var items = JokeShopProcedures.InanimateForms();
            var numItems = items.Count();

            if (numItems == 0)
            {
                return;
            }

            var item = items.ElementAt(die.Next(numItems));

            var low = Math.Min(6, 1 + challenge.ByEndOfTurn/ 2000);
            var high = Math.Min(12, 2 + 4 * challenge.ByEndOfTurn / 2000);
            var target = die.Next(low, high + 1);

            if (target >= 9)
            {
                difficulty = 5;
            }
            else if (target >= 5)
            {
                difficulty = 4;
            }

            var resource = $"slot {item.Category}";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(resource) == 0)
            {
                if (target >= 7)
                {
                    AddPrerequisite(challenge, p => p.GameMode == (int)GameModeStatics.GameModes.PvP);
                }

                AddPart(challenge,
                        $"Equip a level {target} or higher {item.FriendlyName} ({item.Category} slot)",
                        p => ItemProcedures.GetAllPlayerItems(p.Id).Any(i => i.dbItem.IsEquipped &&
                                                                             i.dbItem.Level >= target &&
                                                                             i.dbItem.FormerPlayerId.HasValue &&
                                                                             PlayerProcedures.GetPlayer(i.dbItem.FormerPlayerId).FormSourceId == item.FormSourceId));
                AddResourceContribution(challenge, r => (r == resource) ? 1 : 0);

                if (item.Category == PvPStatics.ItemType_Consumable_Reuseable)
                {
                    // Equipping consumable is harder due to combat timer
                    difficulty++;
                }

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingAchievementRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 10;

            var generalAchievements = new String[]{
                    StatsProcedures.Stat__SearchCount,
                    StatsProcedures.Stat__SpellsCast,
                    StatsProcedures.Stat__TimesMoved,
                    StatsProcedures.Stat__TimesCleansed,
                    StatsProcedures.Stat__TimesMeditated,
                    StatsProcedures.Stat__CovenantFurnitureUsed,
                    StatsProcedures.Stat__CovenantNetDonation,
                    StatsProcedures.Stat__TimesAnimateTFed,
                    StatsProcedures.Stat__TimesAnimateTFing,
                    StatsProcedures.Stat__PsychopathsDefeated,
                    StatsProcedures.Stat__TimesTeleported_Scroll,
                    StatsProcedures.Stat__TransmogsUsed,
                    StatsProcedures.Stat__CovenantCallbackCrystalsUsed,
                    StatsProcedures.Stat__DollsWPRestored,
                    StatsProcedures.Stat__LoreBooksRead,
                    StatsProcedures.Stat__TgOrbVictims,
                    StatsProcedures.Stat__LindellaNetProfit,
                    StatsProcedures.Stat__LindellaNetLoss,
                    StatsProcedures.Stat__WuffieNetProfit,
                    StatsProcedures.Stat__WuffieNetLoss,
                    StatsProcedures.Stat__LorekeeperSpellsLearned,
                    StatsProcedures.Stat__MinibossAttacks,
                    StatsProcedures.Stat__QuestsFailed,
                    StatsProcedures.Stat__QuestsPassed,
                    StatsProcedures.Stat__BusRides,
                };
            var pvpAchievements = new String[]{
                    StatsProcedures.Stat__TimesEnchanted,
                    StatsProcedures.Stat__TimesInanimateTFing,
                    StatsProcedures.Stat__TimesAnimalTFing,
                    StatsProcedures.Stat__MindControlCommandsIssued,
                    StatsProcedures.Stat__PvPPlayerNumberTakedowns,
                    StatsProcedures.Stat__PvPPlayerLevelTakedowns,
                    StatsProcedures.Stat__DungeonArtifactsFound,
                    StatsProcedures.Stat__DungeonDemonsDefeated,
                    StatsProcedures.Stat__DungeonPointsStolen,
                };

            var index = die.Next(generalAchievements.Count() + pvpAchievements.Count());
            string achievement;
            var pvp = false;

            if (index < generalAchievements.Count())
            {
                achievement = generalAchievements[index];
            }
            else
            {
                achievement = pvpAchievements[index - generalAchievements.Count()];
                pvp = true;
            }

            var resource = $"stat {achievement}";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(resource) == 0)
            {
                if (pvp)
                {
                    AddPrerequisite(challenge, p => p.GameMode == (int)GameModeStatics.GameModes.PvP);
                }

                bool playerIsInTopN(Player p, string ach, int place)
                {
                    var repo = new EFAchievementRepository();

                    var topN = repo.Achievements
                                   .Where(a => a.AchievementType == ach)
                                   .OrderByDescending(a => a.Amount).ThenBy(a => a.Timestamp)
                                   .Take(place);

                    if (topN.Any(a => a.OwnerMembershipId == p.MembershipId))
                    {
                        // Player is in top n places
                        return true;
                    }

                    // Also pass if stat is the same as player in first place so achievements with score caps (e.g. book reading) aren't impossible
                    var first = topN.FirstOrDefault();
                    var ceiling = first == null ? 1 : first.Amount;

                    var playerStat = repo.Achievements.FirstOrDefault(a => a.OwnerMembershipId == p.MembershipId && a.AchievementType == ach);
                    var currentAmount = playerStat == null ? 0 : playerStat.Amount;

                    return currentAmount == ceiling;
                }

                (int, int) progressTowardsPlace(Player p, string ach, int place)
                {
                    var repo = new EFAchievementRepository();
                    var stats = repo.Achievements
                                    .Where(a => a.AchievementType == ach)
                                    .OrderByDescending(a => a.Amount).ThenBy(a => a.Timestamp);

                    var first = stats.FirstOrDefault();
                    var max = first == null ? 1 : first.Amount;

                    // Target amount is 1 if space at end of board; score + 1 of current person at end of board, or just their score if player is that person
                    var amountToReach = 1;
                    if (stats.Count() >= place)
                    {
                        var currentOccupant = stats.ToArray().ElementAt(place - 1);
                        amountToReach = currentOccupant.OwnerMembershipId == p.MembershipId ? (int)currentOccupant.Amount : (int)currentOccupant.Amount + 1;
                    }

                    var playerStat = repo.Achievements.FirstOrDefault(a => a.OwnerMembershipId == p.MembershipId && a.AchievementType == ach);
                    var currentAmount = playerStat == null ? 0 : playerStat.Amount;

                    return ((int)currentAmount, (int)Math.Min(amountToReach, max));
                }

                string currentPlace(Player p, string ach)
                {
                    var repo = new EFAchievementRepository();
                    var entry = repo.Achievements
                                    .Where(a => a.AchievementType == ach)
                                    .OrderByDescending(a => a.Amount).ThenBy(a => a.Timestamp)
                                    .ToArray()
                                    .Select((a, i) => new { Rank = i + 1, MembershipId = a.OwnerMembershipId })
                                    .FirstOrDefault(e => e.MembershipId == p.MembershipId);

                    return entry == null ? "Not ranked" : $"Current rank: {entry.Rank}";
                }

                var places = 10 * (1 + (challenge.ByEndOfTurn - 1) / 2000);

                var achievementName = StatsProcedures.StatTypesMap[achievement].FriendlyName;
                AddPart(challenge,
                        $"Rank in the top {places} places for the \"{achievementName}\" achievement",
                        p => playerIsInTopN(p, achievement, places),
                        p => progressTowardsPlace(p, achievement, places),
                        p => currentPlace(p, achievement));
                AddResourceContribution(challenge, r => (r == resource) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

    }
}

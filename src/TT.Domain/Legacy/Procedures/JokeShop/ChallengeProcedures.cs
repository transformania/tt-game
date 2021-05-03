using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;

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
        const String RESOURCE_BRING_PLAYERS = "bring players";
        const String RESOURCE_COMBAT_TIMER = "combat timer";
        const String RESOURCE_CONSUMABLE_USE = "consumable use";
        const String RESOURCE_COVENANT = "covenant";
        const String RESOURCE_EQUIP = "equip";
        const String RESOURCE_FORM = "form";
        const String RESOURCE_HEALTH = "health";
        const String RESOURCE_LEVEL = "level";
        const String RESOURCE_MANA = "mana";
        const String RESOURCE_MIND_CONTROL = "mind control";
        const String RESOURCE_MONEY = "money";
        const String RESOURCE_RUNES_EMBEDDED = "runes embedded";
        const String RESOURCE_SOUL_ITEMS = "soul items";
        const String RESOURCE_SOULBINDINGS = "soulbindings";
        const String RESOURCE_SOULBOUND_RENAMES = "soulbound renames";
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
            var bestPartsUnsatisfied = 0;

            // Find an appropriate challenge
            foreach (var challengeType in challengeTypes)
            {
                var expiresTurnEnd = NewChallengeExpires(challengeType);

                // Challenges must complete before the round is over
                if (expiresTurnEnd < PvPStatics.RoundDuration)
                {
                    var die = LoadedDie(player.Id, challengeType, expiresTurnEnd);
                    var challenge = DeviseChallenge(challengeType, expiresTurnEnd, die);

                    if (challenge != null)
                    {
                        // Try to pick challenge closest to challenge type's requirements
                        if (bestChallenge == null || challenge.Difficulty > bestChallenge.Difficulty ||
                            (challenge.Difficulty == bestChallenge.Difficulty && 
                             challengeType.MaxDifficulty - challenge.Difficulty <= bestChallengeType.MaxDifficulty - bestChallenge.Difficulty))
                        {
                            // Confirm player is eligible for challenge and hasn't already satisfied its requirements
                            if (challenge.Eligible(player))
                            {
                                var partsUnsatisfied = challenge.Parts.Count(part => !part.Satisfied(player));

                                if (partsUnsatisfied > bestPartsUnsatisfied)
                                {
                                    bestChallenge = challenge;
                                    bestChallengeType = challengeType;
                                    bestPartsUnsatisfied = partsUnsatisfied;
                                }
                            }
                        }
                    }

                }

            }

            // Assign the challenge to the player
            if (bestChallenge != null)
            {
                EffectProcedures.GivePerkToPlayer(bestChallengeType.EffectSourceId, player.Id, bestChallengeType.Duration, bestChallengeType.Duration + 5);

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
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have <b>failed</b> your recent challenge and incur a penalty of <b>{challenge.Penalty}</b>!", true);
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

                if (roll <  9)  // 9%
                {
                    TryAddingAnimateFormRequirement(challengeType, die, challenge);
                }
                else if (roll < 12)  // 3%
                {
                    TryAddingImmobileFormRequirement(challengeType, die, challenge);
                }
                else if (roll < 14)  // 2%
                {
                    TryAddingLearnSpellRequirement(challengeType, die, challenge);
                }
                else if (roll < 20)  // 6%
                {
                    TryAddingEquipmentRequirement(challengeType, die, challenge);
                }
                else if (roll < 26)  // 6%
                {
                    TryAddingRuneEmbedRequirement(challengeType, die, challenge);
                }
                else if (roll < 32)  // 6%
                {
                    TryAddingSoulItemRequirement(challengeType, die, challenge);
                }
                else if (roll < 35)  // 3%
                {
                    TryAddingConsumableItemRequirement(challengeType, die, challenge);
                }
                else if (roll < 44)  // 9%
                {
                    TryAddingNonConsumableItemRequirement(challengeType, die, challenge);
                }
                else if (roll < 46)  // 2%
                {
                    TryAddingRecentPurchaseRequirement(challengeType, die, challenge);
                }
                else if (roll < 48)  // 2% 
                {
                    TryAddingConsumableUseRequirement(challengeType, die, challenge);
                }
                else if (roll < 51)  // 3%
                {
                    TryAddingSoulbindingRequirement(challengeType, die, challenge);
                }
                else if (roll < 53)  // 2%
                {
                    TryAddingSoulboundRenameRequirement(challengeType, die, challenge);
                }
                else if (roll < 61)  // 8%
                {
                    TryAddingStatRequirement(challengeType, die, challenge);
                }
                else if (roll < 69)  // 8%
                {
                    TryAddingAchievementRequirement(challengeType, die, challenge);
                }
                else if (roll < 71)  // 2%
                {
                    TryAddingDungeonPointsRequirement(challengeType, die, challenge);
                }
                else if (roll < 73)  // 2%
                {
                    TryAddingCombatRequirement(challengeType, die, challenge);
                }
                else if (roll < 75)  // 2%
                {
                    TryAddingHealthRequirement(challengeType, die, challenge);
                }
                else if (roll < 77)  // 2%
                {
                    TryAddingManaRequirement(challengeType, die, challenge);
                }
                else if (roll < 79)  // 2%
                {
                    TryAddingAPRequirement(challengeType, die, challenge);
                }
                else if (roll < 81)  // 2%
                {
                    TryAddingMoneyRequirement(challengeType, die, challenge);
                }
                else if (roll < 83)  // 2%
                {
                    TryAddingCovenantRequirement(challengeType, die, challenge);
                }
                else if (roll < 86)  // 3%
                {
                    TryAddingLevelRequirement(challengeType, die, challenge);
                }
                else if (roll < 89)  // 3%
                {
                    TryAddingQuestRequirement(challengeType, die, challenge);
                }
                else if (roll < 92)  // 3%
                {
                    TryAddingMindControlRequirement(challengeType, die, challenge);
                }
                else if (roll < 96)  // 4%
                {
                    TryAddingBringCovenantRequirement(challengeType, die, challenge);
                }
                else  // 4%
                {
                    TryAddingBringFormRequirement(challengeType, die, challenge);
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
            PickRewards(challenge, die);

            if (challengeType.Penalty)
            {
                PickPenalties(challenge, die);
            }
        }

        private static void PickRewards(Challenge challenge, Random die)
        {
            var value = challenge.Difficulty * 10;

            var roll = die.Next(100);

            if (roll < 20)  // 20%
            {
                var amount = (int)Math.Max(challenge.Difficulty * 10, 50);
                value -= amount;
                AddReward(challenge, $"a {amount} Arpeyjis bonus", p => PlayerProcedures.GiveMoneyToPlayer(p, amount));
            }
            else if (roll < 40)  // 20%
            {
                var amount = Math.Max(1, challenge.Difficulty / 5 + 1);
                value -= amount * 50;
                AddReward(challenge, $"{amount} spells", p => PlayerLogProcedures.AddPlayerLog(
                    p.Id, $"You have been rewarded with the knowledge of {ListifyHelper.Listify(SkillProcedures.GiveRandomFindableSkillsToPlayer(p, amount), true)}!", true));
            }
            else if (roll < 65)  // 25%
            {
                value -= AddEffectReward(challenge, die);
            }
            else  // 35%
            {
                value -= AddItemReward(challenge, die);
            }

            // Boost value of low value rewards
            if (value > 0)
            {
                AddReward(challenge, $"a {value} Arpeyjis bonus", p => PlayerProcedures.GiveMoneyToPlayer(p, value));
            }
        }

        private static int AddEffectReward(Challenge challenge, Random die)
        {
            int sourceId;
            string description;
            int relativePotency = 1;

            var effectRoll = die.Next(11);
            if (effectRoll == 0)
            {
                description = "boosted Agility";
                sourceId = CharacterPrankProcedures.AGILITY_BOOST;
            }
            else if (effectRoll == 1)
            {
                description = "boosted Charisma";
                sourceId = CharacterPrankProcedures.CHARISMA_BOOST;
            }
            else if (effectRoll == 2)
            {
                description = "boosted Discipline";
                sourceId = CharacterPrankProcedures.DISCIPLINE_BOOST;
            }
            else if (effectRoll == 3)
            {
                description = "boosted Fortitude";
                sourceId = CharacterPrankProcedures.FORTITUDE_BOOST;
            }
            else if (effectRoll == 4)
            {
                description = "increased inventory capacity";
                sourceId = CharacterPrankProcedures.INVENTORY_BOOST;
            }
            else if (effectRoll == 5)
            {
                description = "boosted Luck";
                sourceId = CharacterPrankProcedures.LUCK_BOOST;
            }
            else if (effectRoll == 6)
            {
                description = "boosted Magicka";
                sourceId = CharacterPrankProcedures.MAGICKA_BOOST;
            }
            else if (effectRoll == 7)
            {
                description = "improved mobility";
                sourceId = CharacterPrankProcedures.MOBILITY_BOOST;
                relativePotency = 2;
            }
            else if (effectRoll == 8)
            {
                description = "boosted Perception";
                sourceId = CharacterPrankProcedures.PERCEPTION_BOOST;
            }
            else if (effectRoll == 9)
            {
                description = "boosted Regeneration";
                sourceId = CharacterPrankProcedures.REGENERATION_BOOST;
            }
            else
            {
                description = "boosted Restoration";
                sourceId = CharacterPrankProcedures.RESTORATION_BOOST;
            }

            var duration = Math.Max(challenge.Difficulty / (2 * relativePotency), 1);

            AddReward(challenge, $"{duration} turns of {description}",
                      p => EffectProcedures.MergePlayerPerk(sourceId, p, duration));

            return duration * 20 * relativePotency;
        }

        public static int AddItemReward(Challenge challenge, Random die)
        {
            int itemSourceId;
            Action<Player> action;
            var roll = die.Next(3);

            if (roll < 2)
            {
                // Consumable
                int[] itemTypes = { ItemStatics.CurseLifterItemSourceId,
                                    ItemStatics.AutoTransmogItemSourceId,
                                    ItemStatics.WillpowerBombVolatileItemSourceId,
                                    ItemStatics.WillpowerBombStrongItemSourceId,
                                    ItemStatics.WillpowerBombWeakItemSourceId,
                                    ItemStatics.SelfRestoreItemSourceId,
                                    ItemStatics.LullabyWhistleItemSourceId,
                                    ItemStatics.SpellbookSmallItemSourceId,
                                    ItemStatics.SpellbookMediumItemSourceId,
                                    ItemStatics.SpellbookLargeItemSourceId,
                                    ItemStatics.SpellbookGiantItemSourceId,
                                    ItemStatics.TeleportationScrollItemSourceId,
                                    ItemStatics.TgSplashOrbItemSourceId,
                                    ItemStatics.WillflowerFreshItemSourceId,
                                    ItemStatics.WillflowerRootItemSourceId,
                                    ItemStatics.SpellWeaverFreshItemSourceId,
                                    ItemStatics.SpellWeaverRootItemSourceId,
                                    ItemStatics.CovenantCrystalItemSourceId,
                                    ItemStatics.ConcealmentCookieSourceId,
                                    ItemStatics.FireFritterSourceId,
                                    ItemStatics.BarricadeBrownieSourceId,
                                    ItemStatics.TrueshotTrufflesSourceId,
                                    ItemStatics.NirvanaNuggetSourceId,
                                    ItemStatics.PerceptionPuffSourceId,
                                    ItemStatics.LuckyLemoncakeSourceId,
                                    ItemStatics.DanishOfDiscoverySourceId };

                itemSourceId = itemTypes[die.Next(itemTypes.Count())];
                action =  p => ItemProcedures.GiveNewItemToPlayer(p, itemSourceId);
            }
            else
            {
                // Rune
                var levelRoll = die.Next(100);
                int level;

                if (levelRoll < 20)  // 20%
                {
                    level = 3;  // Standard
                }
                else if (levelRoll < 85)  // 65%
                {
                    level = 5;  // Great
                }
                else if (levelRoll < 97)  // 12%
                {
                    level = 7;  // Major
                }
                else  // 3%
                {
                    level = 9;  // Superior
                }

                itemSourceId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = level, Random = die });
                action = p => DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = itemSourceId, PlayerId = p.Id });
            }

            var itemRepo = new EFItemRepository();
            var staticItem = itemRepo.DbStaticItems.FirstOrDefault(i => i.Id == itemSourceId);

            if (staticItem == null || staticItem.MoneyValueSell > challenge.Difficulty * 10)
            {
                return 0;
            }

            AddReward(challenge, $"a {staticItem.FriendlyName}", action);
            return (int)(staticItem.MoneyValueSell > 0 ? staticItem.MoneyValueSell : 50);
        }

        private static void PickPenalties(Challenge challenge, Random die)
        {
            var roll = die.Next(100);

            if (roll < 40)
            {
                var amount = (int)Math.Max(challenge.Difficulty * 7.5, 50);
                AddPenalty(challenge, $"a {amount} Arpeyjis fine",
                           p => PlayerProcedures.GiveMoneyToPlayer(p, -Math.Min(amount, p.Money)));
            }
            else if (roll < 80)
            {
                AddEffectPenalty(challenge, die);
            }
            else if (roll < 90)
            {
                var duration = challenge.Difficulty * 4;
                AddPenalty(challenge, $"a {duration} turn ban from the Joke Shop",
                           p => EffectProcedures.MergePlayerPerk(JokeShopProcedures.BANNED_FROM_JOKE_SHOP_EFFECT, p, duration));
            }
            else
            {
                var amount = Math.Max(1, challenge.Difficulty / 5);
                AddPenalty(challenge, $"forgetting {amount} spells",
                           p => PlayerLogProcedures.AddPlayerLog(p.Id, EnvironmentPrankProcedures.UnlearnSpell(p, number: amount), true));
            }
        }

        private static void AddEffectPenalty(Challenge challenge, Random die)
        {
            int sourceId;
            string description;
            int relativePotency = 1;

            var effectRoll = die.Next(15);
            if (effectRoll == 0)
            {
                description = "reduced Agility";
                sourceId = CharacterPrankProcedures.AGILITY_PENALTY;
            }
            else if (effectRoll == 1)
            {
                description = "reduced Charisma";
                sourceId = CharacterPrankProcedures.CHARISMA_PENALTY;
            }
            else if (effectRoll == 2)
            {
                description = "reduced Discipline";
                sourceId = CharacterPrankProcedures.DISCIPLINE_PENALTY;
            }
            else if (effectRoll == 3)
            {
                description = "reduced Fortitude";
                sourceId = CharacterPrankProcedures.FORTITUDE_PENALTY;
            }
            else if (effectRoll == 4)
            {
                description = "reduced inventory capacity";
                sourceId = CharacterPrankProcedures.INVENTORY_PENALTY;
            }
            else if (effectRoll == 5)
            {
                description = "reduced Luck";
                sourceId = CharacterPrankProcedures.LUCK_PENALTY;
            }
            else if (effectRoll == 6)
            {
                description = "reduced Magicka";
                sourceId = CharacterPrankProcedures.MAGICKA_PENALTY;
            }
            else if (effectRoll == 7)
            {
                description = "immobility";
                sourceId = CharacterPrankProcedures.MOBILITY_PENALTY;
                relativePotency = 3;
            }
            else if (effectRoll == 8)
            {
                description = "reduced Perception";
                sourceId = CharacterPrankProcedures.PERCEPTION_PENALTY;
            }
            else if (effectRoll == 9)
            {
                description = "reduced Regeneration";
                sourceId = CharacterPrankProcedures.REGENERATION_PENALTY;
            }
            else if (effectRoll == 10)
            {
                description = "reduced Restoration";
                sourceId = CharacterPrankProcedures.RESTORATION_PENALTY;
            }
            else if (effectRoll == 11)
            {
                description = "dizziness";
                sourceId = CharacterPrankProcedures.DIZZY_EFFECT;
                relativePotency = 2;
            }
            else if (effectRoll == 12)
            {
                description = "blindness";
                sourceId = CharacterPrankProcedures.BLINDED_EFFECT;
                relativePotency = 2;
            }
            else if (effectRoll == 13)
            {
                description = "being unable to talk";
                sourceId = CharacterPrankProcedures.HUSHED_EFFECT;
                relativePotency = 2;
            }
            else
            {
                description = "players being able to see your location on your profile";
                sourceId = CharacterPrankProcedures.SNEAK_REVEAL_1;
            }

            var duration = (int)Math.Max(challenge.Difficulty / (2 * relativePotency) * 0.8, 1);

            AddPenalty(challenge, $"{duration} turns of {description}",
                       p => EffectProcedures.MergePlayerPerk(sourceId, p, duration));
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

        private static void TryAddingLearnSpellRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 2;

            var forms = JokeShopProcedures.Forms(_ => true);
            var form = forms.ElementAt(die.Next(forms.Count()));
            var resource = $"spell for {form.FormSourceId}";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(resource) == 0)
            {
                var skillSource = new EFDbStaticSkillRepository().DbStaticSkills.FirstOrDefault(spell => spell.FormSourceId == form.FormSourceId);

                AddPart(challenge,
                        $"Learn the spell \"{skillSource.FriendlyName}\", which turns its victim into a {form.FriendlyName}",
                        p => SkillProcedures.GetSkillsOwnedByPlayer(p.Id).Any(s => s.SkillSourceId == skillSource.Id));
                AddResourceContribution(challenge, r => (r == resource) ? 1 : 0);

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

        private static void TryAddingCovenantRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 2;

            if (challengeType.Duration < 60 &&
                challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_COVENANT) == 0)
            {
                AddPart(challenge,
                        $"Join a covenant or start your own",
                        p => p.Covenant > 0,
                        p => ((p.Covenant <= 0 ? 0 : 1), 1),
                        p => p.Covenant <= 0 ? "Not yet accepted into a covenant" : "Done");
                AddResourceContribution(challenge, r => (r == RESOURCE_COVENANT) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingMoneyRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var low = Math.Min(50, 10 * challengeType.Duration);
            var high = Math.Min(1000, 50 * challengeType.Duration);
            var target = (int)die.Next(low, high + 1);

            var difficulty = target / challengeType.Duration / 20 + 1;

            if (challenge.ByEndOfTurn < 2000)
            {
                // Items to sell less readily available early in round
                difficulty++;
            }

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_MONEY) == 0)
            {
                AddPart(challenge,
                        $"Save up {target} Arpeyjis",
                        p => p.Money >= target,
                        p => ((int)p.Money, target));
                AddResourceContribution(challenge, r => (r == RESOURCE_MONEY) ? target : 0);

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
                                                             .Count(i => i.Item.ItemType != PvPStatics.ItemType_Consumable &&
                                                                         i.dbItem.IsEquipped);

                AddPart(challenge,
                        $"Equip at least {target} items in total (not including single-use consumables)",
                        p => itemsEquipped(p) >= target,
                        p => (itemsEquipped(p), target));
                AddResourceContribution(challenge, r => (r == RESOURCE_EQUIP) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingRuneEmbedRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var low = 1 + challenge.ByEndOfTurn / 2000;
            var high = Math.Min(12, 1 + challenge.ByEndOfTurn / 1000);
            var target = die.Next(low, high + 1);

            var difficulty = 2 + target / 3;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_RUNES_EMBEDDED) == 0)
            {
                int runesEmbedded(Player p)
                {
                    // Player may not appear as the owner for runes they have, so look up by item
                    var itemRepo = new EFItemRepository();
                    var playerItemIds = ItemProcedures.GetAllPlayerItems(p.Id).Select(i => i.dbItem.Id);
                    var runeItemTypeIds = itemRepo.DbStaticItems.Where(i => i.ItemType == PvPStatics.ItemType_Rune).Select(i => i.Id);
                    return itemRepo.Items.Count(i => i.EmbeddedOnItemId.HasValue &&
                                                     playerItemIds.Contains(i.EmbeddedOnItemId.Value) &&
                                                     runeItemTypeIds.Contains(i.ItemSourceId));
                }

                AddPart(challenge,
                        $"Embed at least {target} runes in the items you are wearing or carrying",
                        p => runesEmbedded(p) >= target,
                        p => (runesEmbedded(p), target));
                AddResourceContribution(challenge, r => (r == RESOURCE_RUNES_EMBEDDED) ? target : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingConsumableUseRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_CONSUMABLE_USE) == 0)
            {
                int itemsOnCooldown(Player p) => DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = p.Id }).Count(i => i.TurnsUntilUse > 0);

                AddPart(challenge,
                        $"Use a reusable consumable and keep carrying it (you can remove it from your consumable slot after using it)",
                        p => itemsOnCooldown(p) >= 1,
                        p => (itemsOnCooldown(p), 1));
                AddResourceContribution(challenge, r => (r == RESOURCE_CONSUMABLE_USE) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingSoulItemRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var high = 1 + challenge.ByEndOfTurn / 2000;
            var target = die.Next(1, high + 1);

            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_SOUL_ITEMS) == 0)
            {
                if (target > 1 + challenge.ByEndOfTurn / 4000)
                {
                    AddPrerequisite(challenge, p => p.GameMode == (int)GameModeStatics.GameModes.PvP);
                }

                int playerItems(Player me)
                {
                    var itemPlayerIds = ItemProcedures.GetAllPlayerItems(me.Id)
                                          .Where(i => i.dbItem.FormerPlayerId != null)
                                          .Select(i => i.dbItem.FormerPlayerId);
                    return new EFPlayerRepository().Players.Count(p => (p.BotId == AIStatics.ActivePlayerBotId ||
                                                                        p.BotId == AIStatics.RerolledPlayerBotId) &&
                                                                       itemPlayerIds.Contains(p.Id));
                }

                AddPart(challenge,
                        $"Carry {target} items that used to be other players (not including NPCs)",
                        p => playerItems(p) >= target,
                        p => (playerItems(p), target));
                AddResourceContribution(challenge, r => (r == RESOURCE_SOUL_ITEMS) ? target : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingRecentPurchaseRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            string[] itemTypes =
            {
                PvPStatics.ItemType_Accessory,
                PvPStatics.ItemType_Consumable,
                PvPStatics.ItemType_Consumable_Reuseable,
                PvPStatics.ItemType_Hat,
                PvPStatics.ItemType_Pants,
                PvPStatics.ItemType_Pet,
                PvPStatics.ItemType_Rune,
                PvPStatics.ItemType_Shirt,
                PvPStatics.ItemType_Shoes,
                PvPStatics.ItemType_Underpants,
                PvPStatics.ItemType_Undershirt,
            };
            var itemType = itemTypes[die.Next(itemTypes.Count())];
            var resource = $"slot {itemType}";

            var difficulty = 1;

            if (itemType == PvPStatics.ItemType_Consumable_Reuseable || itemType == PvPStatics.ItemType_Consumable)
            {
                // Equipping consumable is harder due to combat timer
                difficulty++;
            }
            else if (itemType == PvPStatics.ItemType_Pet)
            {
                // Equipping a pet is harder as only one may be carried at a time
                difficulty += 2;
            }

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(resource) == 0)
            {
                int embeddedRecentRunePurchases(Player p)
                {
                    var cutoff = DateTime.UtcNow.AddHours(-1);

                    return ItemProcedures.GetAllPlayerItems(p.Id)
                                    .Count(i => i.Item.ItemType == PvPStatics.ItemType_Rune &&
                                                i.dbItem.LastSold > cutoff &&
                                                i.dbItem.EmbeddedOnItemId.HasValue);
                }

                int equippedRecentPurchases(Player p, string type)
                {
                    var cutoff = DateTime.UtcNow.AddHours(-1);

                    return ItemProcedures.GetAllPlayerItems(p.Id)
                                    .Count(i => i.Item.ItemType == type &&
                                                i.dbItem.LastSold > cutoff &&
                                                i.dbItem.IsEquipped);
                }

                if (itemType == PvPStatics.ItemType_Rune)
                {
                    AddPart(challenge,
                            $"Embed a rune within an hour of purchasing it from Lindella",
                            p => embeddedRecentRunePurchases(p) >= 1,
                            p => (embeddedRecentRunePurchases(p), 1));
                }
                else if (itemType == PvPStatics.ItemType_Pet)
                {
                    AddPart(challenge,
                            $"Tame a pet by purchasing it from Wüffie (complete the challenge within an hour of purchase)",
                            p => equippedRecentPurchases(p, itemType) >= 1,
                            p => (equippedRecentPurchases(p, itemType), 1));
                }
                else
                {
                    AddPart(challenge,
                            $"Equip an item that belongs in the {itemType.Replace("_", " ")} slot within an hour of purchasing it from Lindella",
                            p => equippedRecentPurchases(p, itemType) >= 1,
                            p => (equippedRecentPurchases(p, itemType), 1));
                }

                AddResourceContribution(challenge, r => (r == resource) ? 1 : 0);
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

        private static void TryAddingConsumableItemRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            // difficulty: 1 = easy find, 2 = occasional find, 3 = rare find, easily bought, 4 = rarely sold
            (int, int)[] items =
            {
                (3, ItemStatics.AutoTransmogItemSourceId),
                (3, ItemStatics.CurseLifterItemSourceId),
                (4, ItemStatics.WillpowerBombVolatileItemSourceId),
                (3, ItemStatics.WillpowerBombStrongItemSourceId),
                (2, ItemStatics.WillpowerBombWeakItemSourceId),
                (3, ItemStatics.SelfRestoreItemSourceId),
                (3, ItemStatics.LullabyWhistleItemSourceId),
                (2, ItemStatics.SpellbookSmallItemSourceId),
                (3, ItemStatics.SpellbookMediumItemSourceId),
                (3, ItemStatics.SpellbookLargeItemSourceId),
                (4, ItemStatics.SpellbookGiantItemSourceId),
                (3, ItemStatics.TeleportationScrollItemSourceId),
                (2, ItemStatics.TgSplashOrbItemSourceId),
                (1, ItemStatics.WillflowerDryItemSourceId),
                (2, ItemStatics.WillflowerFreshItemSourceId),
                (3, ItemStatics.WillflowerRootItemSourceId),
                (1, ItemStatics.SpellWeaverDryItemSourceId),
                (2, ItemStatics.SpellWeaverFreshItemSourceId),
                (3, ItemStatics.SpellWeaverRootItemSourceId),
                (2, ItemStatics.CovenantCrystalItemSourceId),
                (2, ItemStatics.ConcealmentCookieSourceId),
                (2, ItemStatics.FireFritterSourceId),
                (2, ItemStatics.BarricadeBrownieSourceId),
                (2, ItemStatics.TrueshotTrufflesSourceId),
                (2, ItemStatics.NirvanaNuggetSourceId),
                (2, ItemStatics.PerceptionPuffSourceId),
                (2, ItemStatics.LuckyLemoncakeSourceId),
                (2, ItemStatics.DanishOfDiscoverySourceId)
            };

            (var difficulty, var itemSourceId) = items[die.Next(items.Count())];
            var staticItem = new EFItemRepository().DbStaticItems.FirstOrDefault(i => i.Id == itemSourceId);
            var resource = $"slot {PvPStatics.ItemType_Consumable}";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(resource) < 2 &&
                staticItem != null)
            {
                AddPart(challenge,
                        $"Equip a {staticItem.FriendlyName} (consumable slot)",
                        p => ItemProcedures.GetAllPlayerItems(p.Id).Any(i => i.dbItem.IsEquipped && i.Item.Id == itemSourceId));
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
                        $"Equip a level {target} or higher souled {item.FriendlyName} ({item.Category.Replace("_", " ")} slot, can be an NPC item)",
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
                else if (item.Category == PvPStatics.ItemType_Pet)
                {
                    // Equipping a pet is harder as only one may be carried at a time
                    difficulty += 2;
                }

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingBringCovenantRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var numPlayers = Math.Max(1, die.Next(4));
            var difficulty = numPlayers * (numPlayers - 1) + 2;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_BRING_PLAYERS) + numPlayers < 4)
            {
                // Either player must be in a coven or they must have that already as part of this challenge
                if (challenge.ResourceUsed(RESOURCE_COVENANT) == 0)
                {
                    AddPrerequisite(challenge, p => p.Covenant > 0);
                }

                int playersFromCovenBrought(Player me) => me.Covenant <= 0 ? 0 : new EFPlayerRepository().Players
                        .Count(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                                    p.Id != me.Id &&
                                    p.Mobility == PvPStatics.MobilityFull &&
                                    p.Covenant == me.Covenant &&
                                    p.IpAddress != me.IpAddress &&
                                    p.BotId == AIStatics.ActivePlayerBotId);

                AddPart(challenge,
                        $"Bring {numPlayers} other animate players from your covenant (not including alts) to the Joke Shop",
                        p => playersFromCovenBrought(p) >= numPlayers,
                        p => (playersFromCovenBrought(p), numPlayers));
                AddResourceContribution(challenge, r => (r == RESOURCE_BRING_PLAYERS) ? numPlayers : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingBringFormRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var numPlayers = Math.Max(1, die.Next(3));
            var difficulty = numPlayers * (numPlayers - 1) + 2;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_BRING_PLAYERS) + numPlayers < 4)
            {
                var animateForms = JokeShopProcedures.Forms(f => f.Category == PvPStatics.MobilityFull || f.Category == JokeShopProcedures.LIMITED_MOBILITY);
                var form = animateForms.ElementAt(die.Next(animateForms.Count()));

                int playersInFormBrought(Player me, int formSourceId) => new EFPlayerRepository().Players
                        .Count(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                                    p.Id != me.Id &&
                                    p.FormSourceId == formSourceId &&
                                    p.IpAddress != me.IpAddress &&
                                    p.BotId == AIStatics.ActivePlayerBotId);

                AddPart(challenge,
                        $"Bring {numPlayers} other players to the Joke Shop (not including alts), each in the form of a {form.FriendlyName}",
                        p => playersInFormBrought(p, form.FormSourceId) >= numPlayers,
                        p => (playersInFormBrought(p, form.FormSourceId), numPlayers));
                AddResourceContribution(challenge, r => (r == RESOURCE_BRING_PLAYERS) ? numPlayers : 0);

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

            // Mitigate conflicting achievements
            var achievementResource = achievement;
            if (achievementResource == StatsProcedures.Stat__LindellaNetLoss)
            {
                achievementResource = StatsProcedures.Stat__LindellaNetProfit;
            }
            else if (achievementResource == StatsProcedures.Stat__WuffieNetLoss)
            {
                achievementResource = StatsProcedures.Stat__WuffieNetProfit;
            }
            else if (achievementResource == StatsProcedures.Stat__QuestsFailed)
            {
                achievementResource = StatsProcedures.Stat__QuestsPassed;
            }
            var resource = $"stat {achievementResource}";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challengeType.Duration >= 60 &&
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

                // Don't give seller achievement task if player is in the running for buyer achievement
                AddPrerequisite(challenge, p => progressTowardsPlace(p, achievement, places).Item1 >= 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingDungeonPointsRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 6;
            var resource = $"stat dungeon points";

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challengeType.Duration >= 60 &&
                challenge.ResourceUsed(resource) == 0)
            {
                AddPrerequisite(challenge, p => p.GameMode == (int)GameModeStatics.GameModes.PvP);

                bool playerIsInTopN(Player me, int place)
                {
                    return PlayerProcedures.GetLeadingPlayers__PvP(place).Any(p => p.Id == me.Id);
                }

                (int, int) progressTowardsPlace(Player p, int place)
                {
                    var stats = PlayerProcedures.GetLeadingPlayers__PvP(place);

                    // Target amount is 1 if space at end of board; score + 1 of current person at end of board, or just their score if player is that person
                    var amountToReach = 1;
                    if (stats.Count() >= place)
                    {
                        var currentOccupant = stats.ToArray().ElementAt(place - 1);
                        amountToReach = currentOccupant.Id == p.Id ? (int)currentOccupant.PvPScore : (int)currentOccupant.PvPScore + 1;
                    }

                    return ((int)p.PvPScore, (int)amountToReach);
                }

                string currentPlace(Player me)
                {
                    var numPlaces = 100;
                    var entry = PlayerProcedures.GetLeadingPlayers__PvP(numPlaces)
                                    .ToArray()
                                    .Select((p, i) => new { Rank = i + 1, PlayerId = p.Id })
                                    .FirstOrDefault(e => e.PlayerId == me.Id);

                    return entry == null ? $"Not currently ranked in the top {numPlaces} places" : $"Current rank: {entry.Rank}";
                }

                var places = 10 * (1 + (challenge.ByEndOfTurn - 1) / 2000);

                AddPart(challenge,
                        $"Rank in the top {places} places on the Dungeon Points leaderboard by finding dungeon artifacts, defeating dungeon demons, and stealing points from other players",
                        p => playerIsInTopN(p, places),
                        p => progressTowardsPlace(p, places),
                        p => currentPlace(p));
                AddResourceContribution(challenge, r => (r == resource) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingMindControlRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 4;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_MIND_CONTROL) == 0)
            {
                AddPrerequisite(challenge, p => p.GameMode == (int)GameModeStatics.GameModes.PvP);

                AddPart(challenge,
                        $"Mind control another player (not including alts)",
                        p => MindControlProcedures.GetAllMindControlVMsWithPlayer(p).Any(mc => mc.Master.Player.Id == p.Id && mc.Victim.Player.IpAddress != p.IpAddress),
                        p => (MindControlProcedures.GetAllMindControlVMsWithPlayer(p).Count(mc => mc.Master.Player.Id == p.Id && mc.Victim.Player.IpAddress != p.IpAddress), 1));
                AddResourceContribution(challenge, r => (r == RESOURCE_MIND_CONTROL) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingSoulbindingRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var min = challenge.ByEndOfTurn / 3000 + 1;
            var max = challenge.ByEndOfTurn / 1000 + 1;
            var target = die.Next(min, max + 1);

            var difficulty = Math.Max(0, target - 3) + 3;

            if (challenge.ByEndOfTurn < 2000)
            {
                difficulty++;
            }

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_SOULBINDINGS) == 0)
            {
                AddPart(challenge,
                        $"Soulbind {target} items",
                        p => DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = p.Id }).Count() >= target,
                        p => (DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = p.Id }).Count(), target));
                AddResourceContribution(challenge, r => (r == RESOURCE_SOULBINDINGS) ? target : 0);

                challenge.Difficulty += difficulty;
            }
        }

        private static void TryAddingSoulboundRenameRequirement(ChallengeType challengeType, Random die, Challenge challenge)
        {
            var difficulty = 1;

            if (challenge.Difficulty + difficulty <= challengeType.MaxDifficulty &&
                challenge.ResourceUsed(RESOURCE_SOULBOUND_RENAMES) == 0)
            {
                AddPrerequisite(challenge, p => DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = p.Id }).Any());

                int SouboundRenames(Player p) => DomainRegistry.Repository
                    .Find(new GetItemsSoulboundToPlayer { OwnerId = p.Id })
                    .Count(i => i.FormerPlayer != null &&
                               (i.FormerPlayer.FirstName != i.FormerPlayer.OriginalFirstName ||
                                i.FormerPlayer.LastName != i.FormerPlayer.OriginalLastName));

                AddPart(challenge,
                        $"Rename 1 soulbound item",
                        p => SouboundRenames(p) >= 1,
                        p => (SouboundRenames(p), 1));
                AddResourceContribution(challenge, r => (r == RESOURCE_SOULBOUND_RENAMES) ? 1 : 0);

                challenge.Difficulty += difficulty;
            }

        }

    }

}

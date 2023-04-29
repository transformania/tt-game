using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Services;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class NovelPrankProcedures
    {

        #region Novel pranks

        public static string DiceGame(Player player)
        {
            // /roll 4d20
            var die1 = PlayerProcedures.RollDie(20);
            var die2 = PlayerProcedures.RollDie(20);
            var die3 = PlayerProcedures.RollDie(20);
            var die4 = PlayerProcedures.RollDie(20);
            var total = die1 + die2 + die3 + die4;

            var score = CalculateDiceGameScore(total);

            StatsProcedures.AddStat(player.MembershipId, StatsProcedures.Stat__DiceGameScore, score);

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} rolls {die1}, {die2}, {die3} and {die4}, giving a total of <b>{total}</b>." + "<br />" + CalculateOdds());

            return $"You pick up four 20-sided dice and roll {die1}, {die2}, {die3} and {die4}, giving a total of <b>{total}</b>.  Your score is <b>{score}</b>.";
        }

        private static void Roll(int[] rolls, int remaining, int sides, int runningTotal)
        {
            for (var i = 1; i <= sides; i++)
            {
                if (remaining == 1)
                {
                    rolls[i + runningTotal] += 1;
                }
                else
                {
                    Roll(rolls, remaining - 1, sides, runningTotal + i);
                }
            }
        }

        private static string CalculateOdds()
        {
            var rolls = new int[100];
            Roll(rolls, 4, 20, 0);

            var text = "";
            var total = 0;
            for (var i = 4; i < rolls.Length; i++)
            {
                var score = CalculateDiceGameScore(i);
                total += rolls[i] * score;
                text += i + " = " + rolls[i] + " -- " + score + "<br>";
            }

            return total + "<br>" + text;
        }

        private static int CalculateDiceGameScore(int total)
        {
            // Aims:
            // - Scores 69 for the target total of 69
            // - Scores further from the target score less
            // - Weighted average score is 0, i.e. no advantage for rolling more times
            // - Good variety of possible scores

            if (total == 69)
            {
                // Bullseye
                return total;
            }
            else if (total >= 42)
            {
                // Near miss
                var dist = 69 - total;
                var quarter = Math.PI / 2;
                var angle = quarter * dist / 27;
                var cos = Math.Cos(angle);
                var amp = 45 * cos * cos + 0.9;
                return (int)amp;
            }
            else
            {
                // Linear taper off (low scores)
                var m = 1.583;
                var c = -42 * m;
                var score = m * total + c;

                // Fine tune to average 0
                if (total <= 16)
                {
                    --score;
                }
                if (total <= 11)
                {
                    ++score;
                }
                if (total <= 6)
                {
                    ++score;
                }
                if (total <= 4)
                {
                    score += 2;
                }
                return (int)score;
            }
        }

        public static string PlaceBountyOnPlayersHead(Player player, Random rand = null)
        {
            // Only place bounties on PvP players
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }

            rand = rand ?? new Random();
            var bountyEffectSourceId = BountyProcedures.PlaceBounty(player, rand);

            if (!bountyEffectSourceId.HasValue)
            {
                return null;
            }

            var details = BountyProcedures.BountyDetails(player, bountyEffectSourceId.Value);

            if (details == null)
            {
                return null;
            }

            StatsProcedures.AddStat(player.MembershipId, StatsProcedures.Stat__BountyCount, 1);

            // Put up some wanted posters
            var locations = LocationsStatics.LocationList.GetLocation.Select(l => l.dbName).ToList();
            var locationMessage = $"<b>Wanted:</b>  A reward is on offer to whoever turns <b>{player.GetFullName()}</b> into a <b>{details.Form?.FriendlyName}</b>!";

            for (var i = 0; i < 10; i++)
            {
                var loc = locations[rand.Next(locations.Count())];
                LocationLogProcedures.AddLocationLog(loc, locationMessage);
                locations.Remove(loc);
            }

            var playerMessage = $"The spirits in the store are enraged by your actions!  They are too out of phase to attack you directly and instead choose to place a bounty on your head!  Beware of the townspeople trying to turn you into a <b>{details.Form?.FriendlyName}</b>!";
            PlayerLogProcedures.AddPlayerLog(player.Id, playerMessage, true);

            return playerMessage;
        }

        public static string SummonPsychopath(Player player, Random rand = null, int? strengthOverride = null, bool? aggro = null)
        {
            rand = rand ?? new Random();

            var baseStrength = Math.Min(Math.Max(0, (player.Level - 1) / 3), 3);
            var strength = strengthOverride ?? (baseStrength + Math.Max(0, rand.Next(7) - 1));

            var prefix = "";
            int level;
            int perk;
            int? extraPerk = null;
            var gender = rand.Next(2);
            int form;

            var turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();

            if (strength <= 0 || (turnNumber < 300 && !strengthOverride.HasValue))
            {
                strength = 0;
                level = 1;
                perk = AIProcedures.PsychopathicForLevelOneEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho1MId : AIProcedures.Psycho1FId;
            }
            else if (strength == 1 || (turnNumber < 600 && !strengthOverride.HasValue))
            {
                strength = 1;
                level = 3;
                prefix = "Fierce";
                perk = AIProcedures.PsychopathicForLevelThreeEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho3MId : AIProcedures.Psycho3FId;
            }
            else if (strength == 2 || (turnNumber < 900 && !strengthOverride.HasValue))
            {
                strength = 2;
                level = 5;
                prefix = "Wrathful";
                perk = AIProcedures.PsychopathicForLevelFiveEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho5MId : AIProcedures.Psycho5FId;
            }
            else if (strength == 3 || (turnNumber < 1500 && !strengthOverride.HasValue))
            {
                strength = 3;
                level = 7;
                prefix = "Loathful";
                perk = AIProcedures.PsychopathicForLevelSevenEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho7MId : AIProcedures.Psycho7FId;
            }
            else if (strength == 4 || (turnNumber < 2400 && !strengthOverride.HasValue))
            {
                strength = 4;
                level = 9;
                prefix = "Soulless";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho9MId : AIProcedures.Psycho9FId;
            }
            else if (strength == 5 || (turnNumber < 3600 && !strengthOverride.HasValue))
            {
                strength = 5;
                level = 11;
                prefix = "Ruthless";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                extraPerk = AIProcedures.PsychopathicForLevelThreeEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho9MId : AIProcedures.Psycho9FId;
            }
            else if (strength == 6 || (turnNumber < 5000 && !strengthOverride.HasValue))
            {
                strength = 6;
                level = 13;
                prefix = "Eternal";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                extraPerk = AIProcedures.PsychopathicForLevelFiveEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho9MId : AIProcedures.Psycho9FId;
            }
            else
            {
                strength = 7;
                level = 15;
                prefix = "";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                extraPerk = AIProcedures.PsychopathicForLevelSevenEffectSourceId;
                form = gender == 0 ? AIProcedures.PsychoChadId : AIProcedures.PsychoChadId;
            }

            var firstName = "Psychopath";

            // Chad doesn't need a title.
            if (strength == 7)
            {
                firstName = "Chad";
            }

            var lastName = NameService.GetRandomLastName();

            if (!prefix.IsEmpty())
            {
                firstName = $"{prefix} {firstName}";
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cmd = new CreatePlayer
            {
                FirstName = firstName,
                LastName = lastName,
                Location = player.dbLocationName,
                FormSourceId = form,
                Level = level,
                Health = 100000,
                MaxHealth = 100000,
                Mana = 100000,
                MaxMana = 100000,
                BotId = AIStatics.PsychopathBotId,
                UnusedLevelUpPerks = 0,
                XP = 0,
                Money = (strength + 1) * 50,
                Gender = gender == 0 ? PvPStatics.GenderMale : PvPStatics.GenderFemale,
            };

            var botId = DomainRegistry.Repository.Execute(cmd);

            // Give spells
            var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
            SkillProcedures.GiveSkillToPlayer(botId, eligibleSkills[rand.Next(eligibleSkills.Count())].Id);

            if (strength >= 5)
            {
                SkillProcedures.GiveSkillToPlayer(botId, PvPStatics.Spell_WeakenId);
            }

            if (strength >= 6)
            {
                var limitedMobilityForms = JokeShopProcedures.AnimateForms().Where(f => f.Category == JokeShopProcedures.LIMITED_MOBILITY).ToArray();

                if (limitedMobilityForms.Any())
                {
                    IDbStaticSkillRepository skillsRepo = new EFDbStaticSkillRepository();

                    var formId = limitedMobilityForms[rand.Next(limitedMobilityForms.Count())].FormSourceId;
                    var immobileSkill = skillsRepo.DbStaticSkills.FirstOrDefault(spell => spell.FormSourceId == formId);

                    if (immobileSkill != null)
                    {
                        SkillProcedures.GiveSkillToPlayer(botId, immobileSkill.Id);
                    }
                }
            }

            // Give bonuses
            EffectProcedures.GivePerkToPlayer(perk, botId);

            if (extraPerk.HasValue)
            {
                EffectProcedures.GivePerkToPlayer(extraPerk.Value, botId);
            }

            // Give runes
            var quantity = rand.Next(1, 3);

            for (var c = 0; c < quantity; c++)
            {
                // Determine the level of rune.
                var runeStrength = strength * 2 + 1;

                // Fucking Chad, man.
                if (strength == 7)
                {
                    runeStrength = 13;
                }

                var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = runeStrength, Random = rand });

                DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = botId });
            }

            // Balance stats
            var psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == botId);
            psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
            playerRepo.SavePlayer(psychoEF);

            // Tell the bot to attack the player
            if (!aggro.HasValue || aggro.Value)
            {
                AIDirectiveProcedures.SetAIDirective_Attack(botId, player.Id);
            }

            if (strength == 7)
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>Oh, fuck! It's {firstName} {lastName}!</b>  His dad is a lawyer!!", true);
            }
            else
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>You have summoned {firstName} {lastName}!</b>  Beware!  They are not friendly!!", true);
            }

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} has summoned <b>{firstName} {lastName}</b>!");

            return "Near the counter is an altar with a leather-bound book resting open upon it.  You take a look and try to read one of the jokes aloud.  It seems to be some consonant-heavy tongue twister that soon leaves you faltering.  You're not quite sure what the set up means, but hope the punchline will be worth it.  As you spurt out the last syllable a puff of red smoke explodes out from the book with an audible bang.  You're not laughing, and that remains the case when you close the book to see a large pentagram seared into the cover.  As the smoke subsides there seems to be a strange neon flicker to the light and a crackling to the air.  You turn sharply to see the psychopath you just summoned readying their attack against you!";
        }

        public static string SummonDoppelganger(Player player, Random rand = null, bool? aggro = null)
        {
            rand = rand ?? new Random();

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cmd = new CreatePlayer
            {
                FirstName = $"Evil {player.FirstName}",
                LastName = player.LastName,
                Location = player.dbLocationName,
                FormSourceId = player.FormSourceId,
                Level = Math.Min(9, player.Level),
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                Mana = player.Mana,
                MaxMana = player.MaxMana,
                BotId = AIStatics.PsychopathBotId,
                UnusedLevelUpPerks = 0,
                XP = 0,
                Money = 100 + player.Money / 10,
                Gender = player.Gender,
            };

            var botId = DomainRegistry.Repository.Execute(cmd);

            // Give spells
            var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
            SkillProcedures.GiveSkillToPlayer(botId, eligibleSkills[rand.Next(eligibleSkills.Count())].Id);
            SkillProcedures.GiveSkillToPlayer(botId, PvPStatics.Spell_WeakenId);

            // Give bonuses - we exclude some with no stats, with side effects, or that serve no purpose on psychos
            var sourcePerks = EffectProcedures.GetPlayerEffects2(player.Id);

            int[] effectsToExclude = { JokeShopProcedures.FIRST_WARNING_EFFECT,
                                       JokeShopProcedures.SECOND_WARNING_EFFECT,
                                       JokeShopProcedures.BANNED_FROM_JOKE_SHOP_EFFECT,
                                       JokeShopProcedures.INVISIBILITY_EFFECT,  // Ensure twin is visible, even if player can't yet attack
                                       JokeShopProcedures.PSYCHOTIC_EFFECT,     // Prevent psycho twin leaving rehab  (Sorry, Pinocchio)
                                       JokeShopProcedures.INSTINCT_EFFECT,      // Should be safe, but just to be on the safe side
                                       JokeShopProcedures.AUTO_RESTORE_EFFECT,  // No free second chances for twins, bad ends are forever
                                       };

            foreach (var sourcePerk in sourcePerks)
            {
                var effect = sourcePerk.dbEffect;
                if (!effectsToExclude.Contains(effect.EffectSourceId) && effect.Duration > 0)
                {
                    EffectProcedures.GivePerkToPlayer(effect.EffectSourceId, botId, effect.Duration, effect.Cooldown);
                }
            }

            // Approximately mirror the buffs the player gets from their items.
            // (Actually equipping equivalent items and runes could easily be abused so use perks)
            var itemBuffs = ItemProcedures.GetPlayerBuffs(player).ItemBuffs();

            itemBuffs += 30;  // level 1 psycho effect gives -30 stats
            if (itemBuffs >= 315)
            {
                itemBuffs -= 315;
                EffectProcedures.GivePerkToPlayer(AIProcedures.PsychopathicForLevelNineEffectSourceId, botId);
            }
            if (itemBuffs >= 225)
            {
                itemBuffs -= 225;
                EffectProcedures.GivePerkToPlayer(AIProcedures.PsychopathicForLevelSevenEffectSourceId, botId);
            }
            if (itemBuffs >= 135)
            {
                itemBuffs -= 135;
                EffectProcedures.GivePerkToPlayer(AIProcedures.PsychopathicForLevelFiveEffectSourceId, botId);
            }
            if (itemBuffs >= 70)
            {
                itemBuffs -= 70;
                EffectProcedures.GivePerkToPlayer(AIProcedures.PsychopathicForLevelThreeEffectSourceId, botId);
            }
            if (itemBuffs < 30)
            {
                itemBuffs += 30;
                EffectProcedures.GivePerkToPlayer(AIProcedures.PsychopathicForLevelOneEffectSourceId, botId);
            }

            // Give runes (round level down to odd)
            var runeLevel = cmd.Level - 1;
            runeLevel = runeLevel - (runeLevel % 2) + 1;
            var quantity = rand.Next(1, 3);

            for (var c = 0; c < quantity; c++)
            {
                var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = runeLevel, Random = rand });
                DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = botId });
            }

            // Balance stats
            var psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == botId);
            psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
            playerRepo.SavePlayer(psychoEF);

            // Tell the bot to attack the player
            if (!aggro.HasValue || aggro.Value)
            {
                AIDirectiveProcedures.SetAIDirective_Attack(botId, player.Id);
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>You have summoned your evil twin!</b>  Beware!  They are not friendly!", true);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} has summoned their evil twin!");

            return "In the corner of the room is a freestanding mirror with a silver gilt frame.  You smear some of the dust off the glass to see your reflection staring back at you through the clearing in the misty haze.  As you look into it you catch sight of yourself twitching slightly, but you don't actually feel anything.  Then your reflection narrows their gaze into a scowl and steps through the rippling glazed portal, bringing you face-to-face with your mirror self!  You should be careful.  Meeting your doppelganger never ends well..";
        }

        public static string OpenPsychoNip(Player player)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
            var idleBots = directiveRepo.AIDirectives.Where(d => d.State == "idle")
                                                     .Select(d => d.OwnerId)
                                                     .ToArray();

            // Set three lowest level psychos without targets on the player
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var botsToAttract = playerRepo.Players.Where(p => idleBots.Contains(p.Id)
                                                           && p.BotId == AIStatics.PsychopathBotId
                                                           && p.Mobility == PvPStatics.MobilityFull)
                                                  .OrderBy(p => p.Level)
                                                  .Select(p => p.Id)
                                                  .Take(3)
                                                  .ToArray();

            foreach (var botId in botsToAttract)
            {
                AIDirectiveProcedures.SetAIDirective_Attack(botId, player.Id);
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You open a tin pf PsychoNip", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} opened a tin of PsychoNip!");

            return "You spot a tin with a colorful insignia on a shelf.  You move over to take a closer look, but accidentally knock the tin to the floor and spill its contents!  You gather up the fallen leaves and place them back in the tin.  \"PsychoNip,\" it reads.  Perhaps you should stay alert for the next few turns in case the scent has caught anyone's attention...";
        }

        public static string ForceAttack(Player attacker, bool strongAttackerAlerts = false, Random rand = null)
        {
            if (attacker.TimesAttackingThisUpdate >= PvPStatics.MaxAttacksPerUpdate)
            {
                return null;
            }

            if (attacker.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }

            rand = rand ?? new Random();
            var candidates = JokeShopProcedures.ActivePlayersInJokeShopApartFrom(attacker)
                                    .Where(p => p.GameMode == (int)GameModeStatics.GameModes.PvP &&
                                                JokeShopProcedures.PlayerHasBeenWarned(p)).ToList();

            if (candidates == null || candidates.IsEmpty())
            {
                return null;
            }

            var victim = candidates[rand.Next(candidates.Count())];

            var spells = SkillProcedures.AvailableSkills(attacker, victim, true);
            if (spells == null || spells.IsEmpty())
            {
                return null;
            }

            var spellList = spells.ToArray();
            var spell = spellList[rand.Next(spellList.Count())];

            var message = $"You are compelled to attack {victim.GetFullName()}!";
            PlayerLogProcedures.AddPlayerLog(attacker.Id, message, strongAttackerAlerts);
            PlayerLogProcedures.AddPlayerLog(victim.Id, $"{attacker.GetFullName()} is compelled to attack you!", true);
            LocationLogProcedures.AddLocationLog(attacker.dbLocationName, $"{attacker.GetFullName()} is compelled to attack {victim.GetFullName()}!");

            // Note we do not apply the full gamut of preconditions of a manual attack present in the controller
            var attack = AttackProcedures.AttackSequence(attacker, victim, spell, false);

            if (strongAttackerAlerts)
            {
                PlayerLogProcedures.AddPlayerLog(attacker.Id, attack, true);
            }

            return $"{message}<br />{attack}";
        }

        public static string Incite(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var candidates = JokeShopProcedures.ActivePlayersInJokeShopApartFrom(player)
                                .Where(p => p.GameMode == (int)GameModeStatics.GameModes.PvP &&
                                            JokeShopProcedures.PlayerHasBeenWarned(p)).ToList();

            if (candidates == null || candidates.IsEmpty())
            {
                return null;
            }

            var attacker = candidates[rand.Next(candidates.Count())];

            if (ForceAttack(attacker, true, rand) == null)
            {
                return null;
            }

            return $"You incite {attacker.GetFullName()} to attack another player!";
        }

        public static string RandomShout(Player player, Random rand = null)
        {
            rand = rand ?? new Random();

            string[] memes = {
                // More memes here
                };
            string meme = null;

            var specialCases = 2;

            var selection = rand.Next(memes.Count() + specialCases);

            if (selection == 0)
            {
                var covens = CovenantProcedures.GetCovenantsList();

                if (player.Covenant.HasValue)
                {
                    covens = covens.Where(c => c.dbCovenant.Id == player.Covenant.Value);
                }

                if (covens.Any())
                {
                    var coven = covens.ElementAt(rand.Next(covens.Count()));
                    meme = $"All hail {coven.Leader.GetFullName()}, leader of {coven.dbCovenant.Name}!";
                }
            }
            else if (selection == 1)
            {
                meme = $"I'm {(rand.Next(2) == 0 ? "" : "not ")}{(rand.Next(2) == 0 ? "cute" : "cyoot")}!!!";
            }
            else
            {
                meme = memes[selection - specialCases];
            }

            if (meme == null)
            {
                return null;
            }

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} shouted <b>\"{meme}\"</b> here.");

            return $"You shouted \"{meme}\"";
        }

        public static string LocatePlayerInCombat(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling());

            var playerRepo = new EFPlayerRepository();
            var playersInCombat = playerRepo.Players
                .Where(p => p.LastCombatTimestamp >= cutoff &&
                            p.Id != player.Id &&
                            p.Mobility == PvPStatics.MobilityFull &&
                            p.InDuel <= 0 &&
                            p.InQuest <= 0 &&
                            p.BotId == AIStatics.ActivePlayerBotId).ToList();

            if (playersInCombat.IsEmpty())
            {
                return null;
            }

            var detected = playersInCombat[rand.Next(playersInCombat.Count())];
            var location = LocationsStatics.GetConnectionName(detected.dbLocationName);

            return $"You hear a beep from a machine.  It has a radar-like display and shows that <b>{detected.GetFullName()}</b> has recently been in combat and is currently in <b>{location}</b>!";
        }

        public static string AwardChallenge(Player player, int minDuration, int maxDuration, bool? withPenalties = null, Random rand = null)
        {
            var challenge = ChallengeProcedures.AwardChallenge(player, minDuration, maxDuration, withPenalties);

            var message = DescribeChallenge(player, challenge, rand);

            if (message == null)
            {
                return null;
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, message, true);

            return "You have been set a challenge!";
        }

        public static string DescribeChallenge(Player player, Challenge challenge, Random rand = null)
        {
            if (challenge == null)
            {
                return null;
            }

            rand = rand ?? new Random();  // RNG selects challenge text but NOT the challenge itself

            string[] adjectives = {"interdimensional", "dark", "tormented", "eternal", "cruel", "malevolent", "mischievous"};
            string[] subjects = {"spirits", "souls", "entities", "apparitions", "beings"};
            string[] ambience = {"strange", "magical", "haunted", "eerie", "unsettling", "lost"};
            string[] venues = {"plane", "shop", "store", "realm"};
            string[] issues = {"question whether you are worthy enough to be here",
                               "laugh at your feeble powers",
                               "are alarmed by your presence",
                               "think you are too weak to stay here",
                               "are angered by your intrusion into their domain"};

            var interdimensional = adjectives[rand.Next(adjectives.Count())];
            var spirits = subjects[rand.Next(subjects.Count())];
            var strange = ambience[rand.Next(ambience.Count())];
            var plane = venues[rand.Next(venues.Count())];
            var questionYourWorthiness = issues[rand.Next(issues.Count())];

            var message = $"The {interdimensional} {spirits} that inhabit this {strange} {plane} {questionYourWorthiness}.  <b>They decide to set you a challenge!</b>  ";
            message += $"You must {ListifyHelper.Listify(challenge.Parts.Select(p => p.Description).ToList(), true)}.  ";
            message += $"If you succeed you will be rewarded with <b>{challenge.Reward}</b>";

            if (!challenge.Penalty.IsNullOrEmpty())
            {
                message += $" but if you fail you will suffer <b>a penalty of {challenge.Penalty}</b>";
            }

            message += $".<br />To pass the challenge you must be in the Joke Shop and meet all the success criteria by the end of <b>turn {challenge.ByEndOfTurn}</b>, about {challenge.GetTimeLeft()} from now.  You can check your progress with Rusty in the Tavern.";

            return message;
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Legacy.Services;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain.World.DTOs;

namespace TT.Domain.Procedures
{
    public static class AIProcedures
    {

        private const string Psycho1MName = "botform_psychopathic_spellslinger_male";
        private const string Psycho1FName = "botform_psychopathic_spellslinger_female";
        private const string Psycho3MName = "botform_psychopathic_spellslinger_male_3";
        private const string Psycho3FName = "botform_psychopathic_spellslinger_female_3";
        private const string Psycho5MName = "botform_psychopathic_spellslinger_male_5";
        private const string Psycho5FName = "botform_psychopathic_spellslinger_female_5";
        private const string Psycho7MName = "botform_psychopathic_spellslinger_male_7";
        private const string Psycho7FName = "botform_psychopathic_spellslinger_male_7";
        private const string Psycho9MName = "botform_psychopathic_spellslinger_male_9";
        private const string Psycho9FName = "botform_psychopathic_spellslinger_male_9";

        public const int Psycho1MId = 13;
        public const int Psycho1FId = 14;
        public const int Psycho3MId = 837;
        public const int Psycho3FId = 838;
        public const int Psycho5MId = 839;
        public const int Psycho5FId = 840;
        public const int Psycho7MId = 841;
        public const int Psycho7FId = 842;
        public const int Psycho9MId = 843;
        public const int Psycho9FId = 844;
        public const int PsychoChadId = 295;

        public const int PsychopathicForLevelOneEffectSourceId = 19;
        public const int PsychopathicForLevelThreeEffectSourceId = 20;
        public const int PsychopathicForLevelFiveEffectSourceId = 21;
        public const int PsychopathicForLevelSevenEffectSourceId = 22;
        public const int PsychopathicForLevelNineEffectSourceId = 23;

        private static Tuple<int, string> GetPsychoFormFromLevelAndSex(int level, string sex)
        {
            if (level == 1)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho1MId, Psycho1MName) : new Tuple<int, string>(Psycho1FId, Psycho1FName);
            }
            else if (level == 3)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho3MId, Psycho3MName) : new Tuple<int, string>(Psycho3FId, Psycho3FName);
            }
            else if (level == 5)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho5MId, Psycho5MName) : new Tuple<int, string>(Psycho5FId, Psycho5FName);
            }
            else if (level == 7)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho7MId, Psycho7MName) : new Tuple<int, string>(Psycho7FId, Psycho7FName);
            }
            else
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho9MId, Psycho9MName) : new Tuple<int, string>(Psycho9FId, Psycho9FName);
            }
        }

        public static void SpawnAIPsychopaths(int count)
        {

            var rand = new Random();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();
            var botCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId);

            for (var i = (0 + botCount); i < (count + botCount); i++)
            {

                // Get the strength based on the turn number.
                var strength = GetPsychopathLevel(turnNumber);

                var spawnLocation = "";

                // Determine spawn location for psychos.
                if (strength >= 11)
                {
                    // Ruthless and Eternal for the dungeons.
                    spawnLocation = LocationsStatics.GetRandomLocation_InDungeon();
                }
                else
                {
                    spawnLocation = LocationsStatics.GetRandomLocationNotInDungeon();
                }

                var cmd = new CreatePlayer
                {
                    FirstName = "Psychopath",
                    Location = spawnLocation,
                    Health = 100000,
                    MaxHealth = 100000,
                    Mana = 100000,
                    MaxMana = 100000,
                    BotId = AIStatics.PsychopathBotId,
                    UnusedLevelUpPerks = 0,
                    XP = 0,
                    Money = 100,
                    LastName = NameService.GetRandomLastName(),
                    Gender = i % 2 == 1 ? PvPStatics.GenderMale : PvPStatics.GenderFemale,
                };

                if (strength == 1)
                {
                    cmd.Level = 1;
                }
                else if (strength == 3)
                {
                    cmd.FirstName = "Fierce " + cmd.FirstName;
                    cmd.Level = 3;
                }
                else if (strength == 5)
                {
                    cmd.FirstName = "Wrathful " + cmd.FirstName;
                    cmd.Level = 5;
                }
                else if (strength == 7)
                {
                    cmd.FirstName = "Loathful " + cmd.FirstName;
                    cmd.Level = 7;
                }
                else if (strength == 9)
                {
                    cmd.FirstName = "Soulless " + cmd.FirstName;
                    cmd.Level = 9;
                }
                else if (strength == 11)
                {
                    cmd.FirstName = "Ruthless " + cmd.FirstName;
                    cmd.Level = 11;
                }
                else if (strength == 13)
                {
                    cmd.FirstName = "Eternal " + cmd.FirstName;
                    cmd.Level = 13;
                }

                var idAndFormName = GetPsychoFormFromLevelAndSex(cmd.Level, cmd.Gender);
                cmd.FormSourceId = idAndFormName.Item1;

                // assert this name isn't already taken
                var ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == cmd.FirstName && p.LastName == cmd.LastName);
                if (ghost != null)
                {
                    continue;
                }

                var id = DomainRegistry.Repository.Execute(cmd);

                // give this bot a random skill
                var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();

                double max = eligibleSkills.Count;
                var randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                var skillToLearn = eligibleSkills.ElementAt(randIndex);
                SkillProcedures.GiveSkillToPlayer(id, skillToLearn.Id);

                // give Ruthless and Eternal the Weaken skill.
                if (strength >= 11)
                {
                    SkillProcedures.GiveSkillToPlayer(id, PvPStatics.Spell_WeakenId);
                }

                // give this bot the Psychpathic perk
                if (strength == 1)
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelOneEffectSourceId, id);
                }
                else if (strength == 3)
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelThreeEffectSourceId, id);
                }
                else if (strength == 5)
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelFiveEffectSourceId, id);
                }
                else if (strength == 7)
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelSevenEffectSourceId, id);
                }
                else if (strength == 9)
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelNineEffectSourceId, id);
                }
                else if (strength == 11) //Ruthless 
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelNineEffectSourceId, id);
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelThreeEffectSourceId, id);
                }
                else if (strength == 13) //Eternal
                {
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelNineEffectSourceId, id);
                    EffectProcedures.GivePerkToPlayer(PsychopathicForLevelFiveEffectSourceId, id);
                }

                // give this psycho a new rune with some random chance it is a higher level than they are, to a max of level 13
                var random = new Random(Guid.NewGuid().GetHashCode());
                var roll = random.NextDouble();

                if (strength < 11 && roll < .1)
                {
                    strength += 4;
                }
                else if (strength < 11 && roll < .3)
                {
                    strength += 2;
                }

                var quantity = Math.Floor(random.NextDouble() * 2) + 1; // 1 or 2

                for (var c = 0; c < quantity; c++)
                {
                    var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = strength, Random = random });
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = id });
                }

                var psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
                playerRepo.SavePlayer(psychoEF);

            }
        }

        public static List<Exception> RunPsychopathActions(WorldDetail worldDetail)
        {
            var rand = new Random(DateTime.Now.Millisecond);

            var errors = new List<Exception>();

            IPlayerRepository playerRepo = new EFPlayerRepository();


            //spawn in more bots if there are less than the default
            var botCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull);
            if (botCount < PvPStatics.PsychopathDefaultAmount)
            {
                SpawnAIPsychopaths(PvPStatics.PsychopathDefaultAmount - botCount);
            }

            var bots = playerRepo.Players.Where(p => p.BotId == AIStatics.PsychopathBotId && p.Mobility == PvPStatics.MobilityFull).ToList();

            foreach (var bot in bots)
            {

                try
                {

                    // if bot is no longer fully animate or is null, skip them
                    if (bot == null || bot.Mobility != PvPStatics.MobilityFull)
                    {
                        continue;
                    }

                    bot.LastActionTimestamp = DateTime.UtcNow;

                    if (!EffectProcedures.PlayerHasActiveEffect(bot.Id, JokeShopProcedures.PSYCHOTIC_EFFECT))
                    {
                        #region drop excess items

                        var botItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPsychopath { OwnerId = bot.Id }).ToList();

                        string[] itemTypes =
                        {
                            PvPStatics.ItemType_Hat, PvPStatics.ItemType_Accessory, PvPStatics.ItemType_Pants,
                            PvPStatics.ItemType_Pet, PvPStatics.ItemType_Shirt, PvPStatics.ItemType_Shoes,
                            PvPStatics.ItemType_Underpants, PvPStatics.ItemType_Undershirt
                        };

                        foreach (var typeToDrop in itemTypes)
                        {
                            if (botItems.Count(i => i.ItemSource.ItemType == typeToDrop) > 1)
                            {
                                var dropList = botItems.Where(i => i.ItemSource.ItemType == typeToDrop).Skip(1);

                                foreach (var i in dropList)
                                {
                                    // Keep runes back until psycho is defeated
                                    DomainRegistry.Repository.Execute(new UnbembedRunesOnItem { ItemId = i.Id });

                                    ItemProcedures.DropItem(i.Id);

                                    var name = "a";

                                    if (i.FormerPlayer != null)
                                    {
                                        name = "<b>" + i.FormerPlayer.FullName + "</b> the";
                                    }

                                    if (i.ItemSource.ItemType == PvPStatics.ItemType_Pet)
                                    {
                                        LocationLogProcedures.AddLocationLog(bot.dbLocationName,
                                            "<b>" + bot.GetFullName() + "</b> released " + name + " pet <b>" + i.ItemSource.FriendlyName + "</b> here.");
                                    }
                                    else
                                    {
                                        LocationLogProcedures.AddLocationLog(bot.dbLocationName,
                                            "<b>" + bot.GetFullName() + "</b> dropped " + name + " <b>" + i.ItemSource.FriendlyName + "</b> here.");
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    var botbuffs = ItemProcedures.GetPlayerBuffs(bot);

                    var meditates = 0;

                    // meditate if needed
                    if (bot.Mana < bot.MaxMana * .5M)
                    {
                        var manaroll = (int)Math.Floor(rand.NextDouble() * 4.0D);
                        for (var i = 0; i < manaroll; i++)
                        {
                            DomainRegistry.Repository.Execute(new Meditate
                            {
                                PlayerId = bot.Id,
                                Buffs = botbuffs,
                                NoValidate = true
                            });
                            meditates++;
                        }
                    }

                    // cleanse if needed, less if psycho has cleansed lately
                    if (bot.Health < bot.MaxHealth * .5M)
                    {
                        var healthroll = (int)Math.Floor(rand.NextDouble() * 4.0D);
                        for (var i = meditates; i < healthroll; i++)
                        {
                            DomainRegistry.Repository.Execute(new Cleanse
                            {
                                PlayerId = bot.Id,
                                Buffs = botbuffs,
                                NoValidate = true
                            });
                        }
                    }

                    var directive = AIDirectiveProcedures.GetAIDirective(bot.Id);

                    // the bot has an attack target, so go chase it
                    if (directive.State == "attack")
                    {
                        var myTarget = PlayerProcedures.GetPlayer(directive.TargetPlayerId);
                        var (mySkills, weakenSkill, inanimateSkill) = GetPsychopathSkills(bot);

                        // if the target is offline, no longer animate, in the dungeon, or in the same form as the spells' target, go into idle mode
                       if (PlayerProcedures.PlayerIsOffline(myTarget) ||
                            myTarget.Mobility != PvPStatics.MobilityFull ||
                            mySkills.IsEmpty() || inanimateSkill == null ||
                            myTarget.FormSourceId == inanimateSkill.StaticSkill.FormSourceId ||
                            myTarget.InDuel > 0 ||
                            myTarget.InQuest > 0)
                        {
                            AIDirectiveProcedures.SetAIDirective_Idle(bot.Id);
                        }
                       else if (bot.IsInDungeon() != myTarget.IsInDungeon())
                        {
                            // Toggle idling when the target isn't in the dungeon.
                            AIDirectiveProcedures.SetAIDirective_Idle(bot.Id);
                        }

                        // the target is okay for attacking
                        else
                        {
                            // the bot must move to its target location.
                            if (myTarget.dbLocationName != bot.dbLocationName)
                            {
                                if (botbuffs.MoveActionPointDiscount() > -100 && CanMove(worldDetail, myTarget))
                                {
                                    var maxSpaces = NumPsychopathMoveSpaces(bot);
                                    var newplace = MoveTo(bot, myTarget.dbLocationName, maxSpaces);
                                    bot.dbLocationName = newplace;
                                }
                            }

                            // if the bot is now in the same place as the target, attack away, so long as the target is online and animate
                            if (bot.dbLocationName == myTarget.dbLocationName &&
                                !PlayerProcedures.PlayerIsOffline(myTarget) &&
                                myTarget.Mobility == PvPStatics.MobilityFull &&
                                CanAttack(worldDetail, bot, myTarget)
                                )
                            {
                                playerRepo.SavePlayer(bot);

                                var numAttacks = Math.Min(3, (int)(bot.Mana / PvPStatics.AttackManaCost));
                                var complete = false;
                                for (var attackIndex = 0; attackIndex < numAttacks && !complete; ++attackIndex)
                                {
                                    var skill = SelectPsychopathSkill(myTarget, mySkills, weakenSkill, rand);
                                    (complete, _) = AttackProcedures.Attack(bot, myTarget, skill);
                                }

                                if (complete)
                                {
                                    EquipDefeatedPlayer(bot, myTarget);
                                }

                            }
                        }

                    }

                    // the bot has no target, so wander and try to find new targets and attack them.
                    else
                    {
                        if (botbuffs.MoveActionPointDiscount() > -100)
                        {
                            // Psychos are coming to the dungeon.
                            if (bot.IsInDungeon())
                            {
                                var newplace = MoveTo(bot, LocationsStatics.GetRandomLocation_InDungeon(), 5);
                                bot.dbLocationName = newplace;
                            }
                            else
                            {
                                var newplace = MoveTo(bot, LocationsStatics.GetRandomLocationNotInDungeon(), 5);
                                bot.dbLocationName = newplace;
                            }
                        }


                        // attack stage
                        var playersHere = playerRepo.Players.Where
                            (p => p.dbLocationName == bot.dbLocationName && p.Mobility == PvPStatics.MobilityFull &&
                                p.Id != bot.Id && p.BotId == AIStatics.PsychopathBotId && p.Level >= bot.Level-1).ToList();

                        // filter out offline players and Lindella
                        var onlinePlayersHere = playersHere.Where(p => !PlayerProcedures.PlayerIsOffline(p)).ToList();

                        if (onlinePlayersHere.Count > 0)
                        {
                            var roll = Math.Floor(rand.NextDouble() * onlinePlayersHere.Count);
                            var victim = onlinePlayersHere.ElementAt((int)roll);
                            AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, victim.Id);
                            playerRepo.SavePlayer(bot);

                            var (mySkills, weakenSkill, inanimateSkill) = GetPsychopathSkills(bot);
                            if (!mySkills.IsEmpty())
                            {
                                var numAttacks = Math.Min(3, (int)(bot.Mana / PvPStatics.AttackManaCost));
                                var complete = false;
                                for (var attackIndex = 0; attackIndex < numAttacks && !complete; ++attackIndex)
                                {
                                    var skill = SelectPsychopathSkill(victim, mySkills, weakenSkill, rand);
                                    (complete, _) = AttackProcedures.Attack(bot, victim, skill);
                                }

                                if (complete)
                                {
                                    EquipDefeatedPlayer(bot, victim);
                                }
                            }
                        }
                    }

                    playerRepo.SavePlayer(bot);

                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }

            return errors;

        }

        private static int NumPsychopathMoveSpaces(Player bot)
        {
            var maxSpaces = 5;

            if (bot.FirstName.Contains("Evil "))
            {
                maxSpaces = 8;
            }
            else if (bot.FirstName.Contains("Ruthless "))
            {
                maxSpaces = 6;
            }
            else if (bot.FirstName.Contains("Eternal "))
            {
                maxSpaces = 7;
            }
            else if (bot.FirstName.Contains("Chad"))
            {
                maxSpaces = 10; // Chad wants your ass!
            }

            return maxSpaces;
        }

        public static void CheckAICounterattackRoutine(Player personAttacking, Player bot)
        {
            // person attacking is a boss and not a psychopath, so do nothing
            if (personAttacking.BotId < AIStatics.PsychopathBotId)
            {
                return;
            }

            // attacking the psychopath.  Random chance the psychopath will set the attacker as their target.
            if (bot.BotId == AIStatics.PsychopathBotId)
            {
                if (personAttacking.BotId == AIStatics.ActivePlayerBotId)
                {
                    var rand = new Random();
                    var numAttacks = NumPsychopathCounterAttacks(bot, rand);

                    var (mySkills, weakenSkill, _) = GetPsychopathSkills(bot);

                    if (!mySkills.IsEmpty())
                    {
                        var complete = false;
                        for (int i = 0; i < numAttacks && !complete; i++)
                        {
                            var skill = SelectPsychopathSkill(personAttacking, mySkills, weakenSkill, rand);
                            (complete, _) = AttackProcedures.Attack(bot, personAttacking, skill);
                        }

                        if (complete)
                        {
                            EquipDefeatedPlayer(bot, personAttacking);
                        }
                    }
                }

                var directive = AIDirectiveProcedures.GetAIDirective(bot.Id);

                // no previous target, so set this player as the new one 
                if (directive.TargetPlayerId == -1 || directive.State == "idle")
                {
                    AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, personAttacking.Id);
                }

                // random chance to see if the attacker becomes the new target
                else
                {
                    var rand = new Random();
                    var roll = rand.NextDouble();
                    if (roll < .08)
                    {
                        AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, personAttacking.Id);
                    }
                }

            }

            // if the target is Donna, counterattack and set that player as her target immediately
            if (bot.BotId == AIStatics.DonnaBotId)
            {
                BossProcedures_Donna.DonnaCounterattack(personAttacking, bot);
            }

            // Valentine counterattack
            if (bot.BotId == AIStatics.ValentineBotId)
            {
                BossProcedures_Valentine.CounterAttack(personAttacking, bot);
            }

            // Bimbo boss counterattack
            else if (bot.BotId == AIStatics.BimboBossBotId)
            {
                BossProcedures_BimboBoss.CounterAttack(personAttacking, bot);
            }

            // rat thieves counterattack
            else if (bot.BotId == AIStatics.MaleRatBotId || bot.BotId == AIStatics.FemaleRatBotId)
            {
                AIProcedures.DealBossDamage(bot, personAttacking, true, 1);
                BossProcedures_Thieves.CounterAttack(personAttacking);
            }

            // fae boss counterattack
            else if (bot.BotId == AIStatics.FaebossBotId)
            {
                AIProcedures.DealBossDamage(bot, personAttacking, true, 1);
                BossProcedures_FaeBoss.CounterAttack(personAttacking);
            }

            // motocycle boss counterattack
            else if (bot.BotId == AIStatics.MotorcycleGangLeaderBotId)
            {
                AIProcedures.DealBossDamage(bot, personAttacking, true, 1);
                BossProcedures_MotorcycleGang.CounterAttack(personAttacking, bot);
            }

            // mouse sisters counterattack
            else if (bot.BotId == AIStatics.MouseNerdBotId || bot.BotId == AIStatics.MouseBimboBotId)
            {
                BossProcedures_Sisters.CounterAttack(personAttacking, bot);
            }

            // demon counterattack
            else if (bot.BotId == AIStatics.DemonBotId)
            {
                BossProcedures_DungeonDemon.CounterAttack(bot, personAttacking);
            }

            // miniboss counterattack
            else if (AIStatics.IsAMiniboss(bot.BotId))
            {
                BossProcedures_Minibosses.CounterAttack(personAttacking, bot);
            }


        }

        private static int NumPsychopathCounterAttacks(Player bot, Random rand = null)
        {
            rand = rand ?? new Random();
            var numAttacks = 0;

            if (bot.FirstName.Contains("Loathful "))
            {
                numAttacks = 1;
            }
            else if (bot.FirstName.Contains("Soulless ") || bot.FirstName.Contains("Evil "))
            {
                numAttacks = 2;
            }
            else if (bot.FirstName.Contains("Ruthless "))
            {
                numAttacks = 3;
            }
            else if (bot.FirstName.Contains("Eternal "))
            {
                numAttacks = rand.Next(3, 6);
            }
            else if (bot.FirstName.Contains("Chad"))
            {
                numAttacks = 6; // Chad doesn't fuck around.
            }

            return numAttacks;
        }

        private static SkillViewModel SelectPsychopathSkill(Player victim, IEnumerable<SkillViewModel> psychoSkills, SkillViewModel weakenSkill, Random rand = null)
        {
            rand = rand ?? new Random();

            var selectedSkill = psychoSkills.ElementAt(rand.Next(psychoSkills.Count()));

            // If victim is already in target form, weaken them
            if (selectedSkill.StaticSkill.FormSourceId == victim.FormSourceId && selectedSkill.MobilityType == victim.Mobility)
            {
                selectedSkill = weakenSkill ?? selectedSkill;
            }

            // If we're trying to weaken a player with no WP, switch to spell not matching the player's current form
            if (victim.Health == 0 && selectedSkill.StaticSkill.Id == PvPStatics.Spell_WeakenId)
            {
                selectedSkill = psychoSkills.FirstOrDefault(s => s.StaticSkill.Id != PvPStatics.Spell_WeakenId &&
                                                                 s.StaticSkill.FormSourceId != victim.FormSourceId) ?? selectedSkill;
            }

            return selectedSkill;
        }

        private static (IEnumerable<SkillViewModel> selectedSkills, SkillViewModel weakenSkill, SkillViewModel inanimateSkill) GetPsychopathSkills(Player bot)
        {
            var allMySkills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id).Where(s =>
                s.StaticSkill.ExclusiveToFormSourceId == null && s.StaticSkill.ExclusiveToItemSourceId == null);
            var selectedSkills = new List<SkillViewModel>();
            SkillViewModel weakenSkill = null;

            var inanimateSkill = allMySkills.Where(s => s.MobilityType == PvPStatics.MobilityInanimate || s.MobilityType == PvPStatics.MobilityPet)
                                            .OrderBy(s => s.dbSkill.Id)
                                            .FirstOrDefault();
            if (inanimateSkill != null)
            {
                selectedSkills.Add(inanimateSkill);
            }

            if (bot.FirstName.Contains("Evil ") || bot.FirstName.Contains("Ruthless ") || bot.FirstName.Contains("Eternal "))
            {
                weakenSkill = allMySkills.FirstOrDefault( s => s.StaticSkill.Id == PvPStatics.Spell_WeakenId);
                if (weakenSkill != null)
                {
                    selectedSkills.Add(weakenSkill);
                }
            }

            if (bot.FirstName.Contains("Eternal ") || bot.FirstName.Contains("Chad"))
            {
                var animateSkill = allMySkills.FirstOrDefault(s => s.MobilityType == PvPStatics.MobilityFull);
                if (animateSkill != null)
                {
                    selectedSkills.Add(animateSkill);
                }
            }

            return (selectedSkills, weakenSkill, inanimateSkill);
        }

        public static string MoveTo(Player bot, string locationDbName, int distance, Action<Player, string> playerEnteredTile = null)
        {
            var botLocation = bot.dbLocationName;

            if (botLocation == locationDbName)
            {
                return botLocation;
            }

            var start = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == botLocation);
            var end = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locationDbName);

            var pathTiles = PathfindingProcedures.GetMovementPath(start, end);

            if (pathTiles.Count == 0)
            {
                return botLocation;
            }

            var botName = bot.FirstName + " " + bot.LastName;

            var nextTileIndex = 0;
            var nextTile = botLocation;
            var nextTileName = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == nextTile).Name;

            var numberOfSteps = Math.Min(distance, pathTiles.Count);

            while (nextTileIndex < numberOfSteps)
            {
                var currentTile = nextTile;
                var currentTileName = nextTileName;

                nextTile = pathTiles[nextTileIndex++];
                nextTileName = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == nextTile).Name;

                LocationLogProcedures.AddLocationLog(currentTile, botName + " left toward " + nextTileName);
                LocationLogProcedures.AddLocationLog(nextTile, botName + " entered from " + currentTileName);

                if (playerEnteredTile != null)
                {
                    playerEnteredTile(bot, nextTile);
                }
            }

            return nextTile;
        }

        public static int GetPsychopathLevel(int turnNumber)
        {

            // 1 = regular
            // 3 = fierce
            // 5 = wrathful
            // 7 = loathful
            // 9 = soulless
            // 11 = ruthless
            // 13 = eternal

            // regular psychopath
            if (turnNumber < 300)
            {
                return 1;
            }

            var rand = new Random(Guid.NewGuid().GetHashCode());
            var roll = rand.NextDouble();

            if (turnNumber >= 300 && turnNumber < 600)
            {
                // 15% chance to roll a fierce (lvl 3)
                if (roll < .15D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 600 && turnNumber < 900)
            {
                if (roll < .04D)
                {
                    return 5;
                }
                else if (roll < .15D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 900 && turnNumber < 1500)
            {
                if (roll < .03D)
                {
                    return 7;
                }
                else if (roll < .1D)
                {
                    return 5;
                }
                else if (roll < .25D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 1500 && turnNumber < 2400)
            {
                if (roll < .015D)
                {
                    return 9;
                }
                else if (roll < .03D)
                {
                    return 7;
                }
                else if (roll < .13D)
                {
                    return 5;
                }
                else if (roll < .35D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 2400 && turnNumber < 3600)
            {
                if (roll < .015D)
                {
                    return 11;
                }
                else if (roll < .025D)
                {
                    return 9;
                }
                else if (roll < .06D)
                {
                    return 7;
                }
                else if (roll < .17D)
                {
                    return 5;
                }
                else if (roll < .37D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 3600)
            {
                if (roll < .01D)
                {
                    return 13;
                }
                else if (roll < .02D)
                {
                    return 11;
                }
                else if (roll < .035D)
                {
                    return 9;
                }
                else if (roll < .07D)
                {
                    return 7;
                }
                else if (roll < .21D)
                {
                    return 5;
                }
                else if (roll < .45)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }

            return 1;

        }

        public static void EquipDefeatedPlayer(Player owner, Player defeatedPlayer)
        {
            var playerIsBot = owner.BotId <= AIStatics.PsychopathBotId;

            if (owner.BotId == AIStatics.PsychopathBotId && EffectProcedures.PlayerHasActiveEffect(owner.Id, JokeShopProcedures.PSYCHOTIC_EFFECT))
            {
                // Do not apply to temporary psychos to avoid circumventing inventory rules
                playerIsBot = false;
            }

            if (playerIsBot)
            {
                // have the bot equip any new item they are carrying (psychos take off duplicates later in world update)

                var items = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = owner.Id });
                var item = items.FirstOrDefault(i => i.FormerPlayer?.Id == defeatedPlayer.Id);

                if (item != null)
                {
                    ItemProcedures.EquipItem(item.Id, true);
                }

                var rune = items.Where(i => i.ItemSource.ItemType == PvPStatics.ItemType_Rune &&
                                            i.ItemSource.RuneLevel <= defeatedPlayer.Level)
                                .OrderByDescending(i => i.ItemSource.RuneLevel)
                                .FirstOrDefault();

                if (rune != null)
                {
                    DomainRegistry.Repository.Execute(new EmbedRune { ItemId = item.Id, PlayerId = owner.Id, RuneId = rune.Id });

                    IPlayerRepository playerRepo = new EFPlayerRepository();
                    var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == owner.Id);
                    newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
                    playerRepo.SavePlayer(newMe);
                }
            }
        }

        public static void DealBossDamage(Player boss, Player attacker, bool humanAttacker, int attackCount)
        {
            IBossDamageRepository repo = new EFBossDamageRepository();

            var damage = repo.BossDamages.FirstOrDefault(bf => bf.PlayerId == attacker.Id && bf.BossBotId == boss.BotId);

            if (damage == null)
            {
                damage = new BossDamage
                {
                    PlayerId = attacker.Id,
                    BossBotId = boss.BotId,
                    Timestamp = DateTime.UtcNow,
                };
            }

            if (humanAttacker)
            {
                damage.PlayerAttacksOnBoss += attackCount;

            }
            else
            {
                damage.BossAttacksOnPlayer += attackCount;
            }

            // calculate a unique score to add, weighted a little in favor of higher level human attackers / victims
            damage.TotalPoints += (float)attackCount * (.75F + .25F * (float)attacker.Level);
            damage.Timestamp = DateTime.UtcNow;

            repo.SaveBossDamage(damage);

        }

        public static List<BossDamage> GetTopAttackers(int bossBotId, int amount)
        {
            IBossDamageRepository repo = new EFBossDamageRepository();
            return repo.BossDamages.Where(b => b.BossBotId == bossBotId && b.PlayerAttacksOnBoss > 0).OrderByDescending(b => b.TotalPoints).Take(amount).ToList();
        }

        private static bool CanMove(WorldDetail world, Player bot)
        {
            if (world.Boss_MotorcycleGang == AIStatics.ACTIVE && bot.FormSourceId == BossProcedures_MotorcycleGang.BikerFollowerFormSourceId)
            {
                return false;
            }

            return true;
        }

        private static bool CanAttack(WorldDetail world, Player bot, Player victim)
        {

            if (victim.BotId == AIStatics.MotorcycleGangLeaderBotId)
            {
                return false;
            }

            if (world.Boss_MotorcycleGang == AIStatics.ACTIVE &&
                bot.FormSourceId == BossProcedures_MotorcycleGang.BikerFollowerFormSourceId &&
                victim.FormSourceId == BossProcedures_MotorcycleGang.BikerFollowerFormSourceId &&
                victim.BotId == AIStatics.PsychopathBotId
                )
            {
                return false;
            }

            return true;
        }

    }

}

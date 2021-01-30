using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;


/*
 * Oh no! A terrible curse has befallen the once vampire lord!
 * Now, he's cursed with holiday cheer! 
 * She now suffers in silence, punishing those that are naughty. 
 * Reskins can be TFs with the right spin! 
 * 
 * I admit this is really hacky, but this is a holiday boss reskin.
 * -Fritz
 */
namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Valentine
    {

        private const string ValentineFirstName = "Lady";
        private const string ValentineLastName = "Krampus";

        public const int ValentineFormSourceId = 739; //Krampus

        public const int BoySpellSourceID = 1064; //Gingerbread Man
        public const int GirlSpellSourceID = 1053; //Candy Cane Girl
        public const int TreeSpellSourceID = 1054; //Christmas Tree

        public const int BloodyCurseSpellSourceId = 1264; //Smooch
        private const int BloodyKissEffectSourceId = 197; //Smooch

        public const int ValentinesPresenceSpellSourceId = 1261; //Snatch
        private const int ValentinesPresenceEffectSourceId = 196; //Snatch

        public const int QueensPantiesItemSourceId = 599; //Snowflake

        private const int DayNightInterval = 12;

        public const string DayStance = "daystance";
        public const string NightStance = "nightstance";

        // NIGHT -- Joyful Caroler
        public const int NightVampireMaleSpellSourceId = 1267;
        public const int NightVampireMaleFormSourceId = 1020;

        // NIGHT -- Cheerful Caroler
        public const int NightVampireFemaleSpellSourceId = 1266;
        public const int NightVampireFemaleFormSourceId = 1019;

        // DAY -- Joyful Caroler
        public const int DayVampireMaleSpellSourceId = 1267;
        public const int DayVampireMaleFormSourceId = 1020;

        // DAY -- Cheerful Caroler
        public const int DayVampireFemaleSpellSourceId = 1266;
        public const int DayVampireFemaleFormSourceId = 1019;

        public static void SpawnValentine()
        {
            var valentine = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.ValentineBotId });

            if (valentine == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = ValentineFirstName,
                    LastName = ValentineLastName,
                    Location = GetStanceLocation(),
                    Gender = PvPStatics.GenderMale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = ValentineFormSourceId,
                    Money = 1000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 30,
                    BotId = AIStatics.ValentineBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var valentineEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                valentineEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(valentineEF));
                playerRepo.SavePlayer(valentineEF);

                // give Valentine his reward item drop
                var createItemCmd = new CreateItem
                {
                    dbLocationName = "",
                    EquippedThisTurn = false,
                    IsEquipped = false,
                    IsPermanent = false,
                    Level = 6,
                    OwnerId = id,
                    PvPEnabled = -1,
                    ItemSourceId = ItemStatics.GetStaticItem(QueensPantiesItemSourceId).Id
                };

                DomainRegistry.Repository.Execute(createItemCmd);

                // save his aiDirective, just for the sake of knowing his spawn turn
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                var directive = new AIDirective
                {
                    OwnerId = id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                };

                aiRepo.SaveAIDirective(directive);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.VAMPIRE_RUNE, PlayerId = valentineEF.Id });
                }


            }
        }

        public static void CounterAttack(Player human, Player valentine)
        {

            AIProcedures.DealBossDamage(valentine, human, true, 1);

            // if Valentine's willpower is down to zero, have him hand over the panties and vanish.
            if (valentine.Health <= 0)
            {

                var victoryMessage = "'Fa-la-la-la-la la la la-la!' you recite again, pushing Lady Krampus closer to her limits. The Krampus, worn and weary from your incessant singing, frowns and shakes her head as you continue with your attack upon her. 'I grow tired of this entire charade! This isn't any fun!' she scowls and puts hands over her ears, stepping aside as if to escape your jingling. 'You win! I can't stand this! I didn't even want to stay in this accursed town, anyways!' You can feel a smile creep across your lips, happy in your attempts to erode their will. Still holding her hands upon the sides of her head, she moves to the back of the cabin and reaches into a corner. 'All I wanted to do was punish naughty boys and girls and make the holiday better, but my efforts are clearly out-classed! I feel you would do better in my place!' She continued to mutter to herself as she digs through the rubble, only relenting once they have fetched a lock-box from the debris. 'You seem to be a particularly greedy type, so I have a gift for you. Take it and do as you like with it. I'm done with this place.' She places it into your hands and in an instant, she disappears in a cloud of snow. In their parting, the ringing of caroling in your own brain seems to finally cease to free you from its curse. Shaking your head to come back to your senses, you look down at the item left for you. You cautiously open the unlocked box, finding a simple crystalline item. Much like a snowflake, its form looks rather delicate and fragile.";

                PlayerLogProcedures.AddPlayerLog(human.Id, victoryMessage, true);

                EndEvent(human.Id);
            }

            // Valentine is fine, do counterattack
            else
            {

                // regular counterattacks, not berserk
                if (valentine.Health > valentine.MaxHealth / 4)
                {
                    //Gingerbread Boys and Candy Cane Girls
                    if (human.Gender == PvPStatics.GenderMale)
                    {
                        AttackProcedures.Attack(valentine, human, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(valentine, human, BoySpellSourceID);
                        AttackProcedures.Attack(valentine, human, BoySpellSourceID);
                        AIProcedures.DealBossDamage(valentine, human, false, 3);
                    }
                    else
                    {
                        AttackProcedures.Attack(valentine, human, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(valentine, human, GirlSpellSourceID);
                        AttackProcedures.Attack(valentine, human, GirlSpellSourceID);
                        AIProcedures.DealBossDamage(valentine, human, false, 3);
                    }

                    // give this player the vampire curse if they do not yet have it
                    if (!EffectProcedures.PlayerHasEffect(human, BloodyKissEffectSourceId))
                    {
                        AttackProcedures.Attack(valentine, human, BloodyCurseSpellSourceId);
                        AIProcedures.DealBossDamage(valentine, human, false, 1);
                    }

                    // give this player the immobility curse if they do not yet have it
                    if (!EffectProcedures.PlayerHasEffect(human, ValentinesPresenceEffectSourceId))
                    {
                        AttackProcedures.Attack(valentine, human, ValentinesPresenceSpellSourceId);
                        AIProcedures.DealBossDamage(valentine, human, false, 1);
                    }
                }

                // berserk mode counterattack
                else
                {

                    // counterattack three against original attacker, and don't bother trying to turn them into a tree.
                    if (human.Gender == PvPStatics.GenderMale)
                    {
                        AttackProcedures.Attack(valentine, human, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(valentine, human, BoySpellSourceID);
                        AttackProcedures.Attack(valentine, human, BoySpellSourceID);
                        AttackProcedures.Attack(valentine, human, BoySpellSourceID);
                        AIProcedures.DealBossDamage(valentine, human, false, 4);
                    }
                    else
                    {
                        AttackProcedures.Attack(valentine, human, PvPStatics.Spell_WeakenId);
                        AttackProcedures.Attack(valentine, human, GirlSpellSourceID);
                        AttackProcedures.Attack(valentine, human, GirlSpellSourceID);
                        AttackProcedures.Attack(valentine, human, GirlSpellSourceID);
                        AIProcedures.DealBossDamage(valentine, human, false, 4);
                    }

                    // give this player the vampire curse if they do not yet have it
                    if (!EffectProcedures.PlayerHasEffect(human, BloodyKissEffectSourceId))
                    {
                        AttackProcedures.Attack(valentine, human, BloodyCurseSpellSourceId);
                        AIProcedures.DealBossDamage(valentine, human, false, 1);
                    }

                    // give this player the immobility curse if they do not yet have it
                    if (!EffectProcedures.PlayerHasEffect(human, ValentinesPresenceEffectSourceId))
                    {
                        AttackProcedures.Attack(valentine, human, ValentinesPresenceSpellSourceId);
                        AIProcedures.DealBossDamage(valentine, human, false, 1);
                    }

                    // attack everyone else with 1 cast for Holiday cheer.
                    var playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

                    playersHere = playersHere.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                    !PlayerProcedures.PlayerIsOffline(p) &&
                    p.Level >= 3 &&
                    p.BotId == AIStatics.ActivePlayerBotId &&
                    p.Id != valentine.Id &&
                    p.Id != human.Id &&
                    p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();

                    //People need to be more festive! 
                    foreach (var p in playersHere)
                    {
                        AttackProcedures.Attack(valentine, p, TreeSpellSourceID);
                        AIProcedures.DealBossDamage(valentine, p, false, 1);
                    }
                }
            }
        }

        public static void RunValentineActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);

            // if valentine is not in the right place have him move to the other location
            var locationToBe = GetStanceLocation();
            if (valentine.dbLocationName != locationToBe)
            {
                AIProcedures.MoveTo(valentine, locationToBe, 100000);
                valentine.dbLocationName = locationToBe;
                playerRepo.SavePlayer(valentine);
            }

            // get all of the players in the room
            var playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

            playersHere = playersHere.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                !PlayerProcedures.PlayerIsOffline(p) &&
                p.Level >= 3 &&
                p.BotId == AIStatics.ActivePlayerBotId &&
                p.Id != valentine.Id &&
                p.InDuel <= 0 &&
                p.InQuest <= 0).ToList();

            var turnNo = PvPWorldStatProcedures.GetWorldTurnNumber();

            if (valentine.Mana < valentine.MaxMana / 3)
            {
                var valentineBuffs = ItemProcedures.GetPlayerBuffs(valentine);
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = valentine.Id, Buffs = valentineBuffs, NoValidate = true });
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = valentine.Id, Buffs = valentineBuffs, NoValidate = true });
            }

            foreach (var p in playersHere)
            {

                // give this player the vampire curse if they do not yet have it
                if (!EffectProcedures.PlayerHasEffect(p, BloodyKissEffectSourceId))
                {
                    AttackProcedures.Attack(valentine, p, BloodyCurseSpellSourceId);
                    AIProcedures.DealBossDamage(valentine, p, false, 1);
                }

                // give this player the immobility curse if they do not yet have it
                if (!EffectProcedures.PlayerHasEffect(p, ValentinesPresenceEffectSourceId))
                {
                    AttackProcedures.Attack(valentine, p, ValentinesPresenceSpellSourceId);
                    AIProcedures.DealBossDamage(valentine, p, false, 1);
                }

            }

            // have Valentine equip his two strongest swords
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<Item> valentineSwords = itemRepo.Items.Where(i => i.OwnerId == valentine.Id && i.ItemSourceId != QueensPantiesItemSourceId).OrderByDescending(i => i.Level);
            var swordsToSave = new List<Item>();

            var counter = 1;
            foreach (var sword in valentineSwords)
            {
                if (!sword.IsEquipped && counter < 3)
                {
                    sword.IsEquipped = true;
                    swordsToSave.Add(sword);
                }
                else if (sword.IsEquipped && counter >= 3)
                {
                    sword.IsEquipped = false;
                    swordsToSave.Add(sword);
                }
                counter++;
            }

            foreach (var sword in swordsToSave)
            {
                itemRepo.SaveItem(sword);
            }

        }

        public static void TalkToAndCastSpell(Player player, Player valentine)
        {
            var stance = GetStance();

            // Player should not be able to attack Krampus while another boss is active.
            // This check is also made here in the event someone attempts to use a bookmark to attack the Krampus.
            if (stance == BossProcedures_Valentine.DayStance && !PvPWorldStatProcedures.IsAnyBossActive())
            {
                if (player.FormSourceId != DayVampireFemaleFormSourceId && player.FormSourceId != DayVampireMaleFormSourceId)
                {
                    if (player.Gender == PvPStatics.GenderMale)
                    {
                        AttackProcedures.Attack(valentine, player, DayVampireFemaleSpellSourceId);
                    }
                    else
                    {
                        AttackProcedures.Attack(valentine, player, DayVampireMaleSpellSourceId);
                    }
                }
            }
            /* Only have the one stance.
            else if (stance == BossProcedures_Valentine.NightStance)
            {
                if (player.FormSourceId != NightVampireFemaleFormSourceId && player.FormSourceId != NightVampireMaleFormSourceId)
                {
                    if (player.Gender == PvPStatics.GenderMale)
                    {
                        AttackProcedures.Attack(valentine, player, NightVampireFemaleSpellSourceId);
                    }
                    else
                    {
                        AttackProcedures.Attack(valentine, player, NightVampireMaleSpellSourceId);
                    }
                }
            }
            */
        }

        public static void EndEvent(int newOwnerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            var valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);


            var panties = itemRepo.Items.FirstOrDefault(i => i.ItemSourceId == QueensPantiesItemSourceId);
            panties.OwnerId = newOwnerId;
            itemRepo.SaveItem(panties);

            DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = valentine.Id, IgnoreRunes = true });
            DomainRegistry.Repository.Execute(new DeletePlayer { PlayerId = valentine.Id });

            PvPWorldStatProcedures.Boss_EndValentine();

            // find the players who dealt the most damage and award them with XP
            var damages = AIProcedures.GetTopAttackers(valentine.BotId, 15);

            // top player gets 500 XP, each player down the line receives 25 fewer
            var l = 0;
            var maxReward = 500;

            for (var r = 0; r < damages.Count; r++)
            {
                var damage = damages.ElementAt(r);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                if (victor == null)
                {
                    continue;
                }
                var reward = maxReward - (l * 30);
                victor.XP += reward;
                l++;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your valiant (maybe foolish?) efforts in challenging " + valentine.GetFullName() + " you receieve " + reward + " XP from your risky struggle!</b>", true);

                playerRepo.SavePlayer(victor);

                // top three get runes
                if (r <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.VAMPIRE_RUNE, PlayerId = victor.Id });
                }

            }

        }

        public static string GetStance()
        {
            var turnNum = PvPWorldStatProcedures.GetWorldTurnNumber();

            /* We don't really need the Krampus to change stances.
            if (turnNum % (DayNightInterval * 2) < DayNightInterval)
            {
                return DayStance;
            }
            else
            {
                return NightStance;
            }
            */

            return DayStance;
        }

        public static string GetStanceLocation()
        {
            var stance = GetStance();
            if (stance == DayStance)
            {
                //Normal castle_training
                return "forest_cabin";
            }
            else
            {
                return "castle_tower";
            }
        }

        public static bool IsAttackableInForm(Player attacker)
        {
            var stance = GetStance();

            // Day stance:  Only day vampires can attack Valentine
            if (stance == DayStance)
            {
                if (attacker.FormSourceId == DayVampireMaleFormSourceId || attacker.FormSourceId == DayVampireFemaleFormSourceId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Night stance:  Only night vampires can attack Valentine - don't need to change anything, because I'm lazy and the Krampus should only be in day stance.
            else
            {
                if (attacker.FormSourceId == NightVampireMaleFormSourceId || attacker.FormSourceId == NightVampireFemaleFormSourceId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }  
        }



        public static string GetWrongFormText()
        {
            return "As you approach, the Krampus leers over you, their look seeming to cut right to your soul. \"Have you come for another kiss, or would you like to go into my sack?\" Their gaze looks incredibly malicious, as if they're thinking the best way to gobble you up. Perhaps the Krampus is just waiting to deal with another naughty spell-casters. \"Won't you join me, spreading holiday cheer?\" she asks you, the sounds of caroling ringing in your ears.";

        }

    }
}
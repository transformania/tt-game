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

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Valentine
    {

        private const string ValentineFirstName = "Lord 'Teaserael'";
        private const string ValentineLastName = "Valentine";

        public const string ValentineFormDbName = "form_First_Lord_of_the_Valentine_Castle_Valentine's_Family";
        public const string SwordSpell = "skill_The_Dance_of_Blades_Ashley_Valentine";

        public const string BloodyCurseSpell = "skill_A_Bloody_Kiss_Lilith";
        private const string BloodyKissEffect = "effect_A_Bloody_Kiss_Lilith";

        public const string ValentinesPresenceSpell = "skill_Valentine's_Presence_Lilith";
        private const string ValentinesPresenceEffect = "effect_Valentine’s_Presence_Lilith";

        public const string QueensPanties = "item_Queen_Valentine’s_Panties_Ashley_Valentine";

        private const int DayNightInterval = 12;

        public const string DayStance = "daystance";
        public const string NightStance = "nightstance";

        // NIGHT -- male
        public const string NightVampireMaleSpell = "skill_Wisdom_of_the_Nightkin._Leia_Valentine";
        public const string NightVampireMaleForm = "form_Child_of_the_Night_Leia_Valentine";

        // NIGHT -- female
        public const string NightVampireFemaleSpell = "skill_Elegance_of_the_Nightkin_Leia_Valentine";
        public const string NightVampireFemaleForm = "form_Disciple_of_the_Night_Leia_Valentine";

        // DAY -- male
        public const string DayVampireMaleSpell = "skill_Strength_of_the_Nightkin_Leia_Valentine";
        public const string DayVampireMaleForm = "form_Vampire_Fighter_Leia_Valentine";

        // DAY -- female
        public const string DayVampireFemaleSpell = "skill_Prowess_of_the_Nightkin_Leia_Valentine";
        public const string DayVampireFemaleForm = "form_Vampire_Duelist_Leia_Valentine";

        private const int ValentineFormId = 207;

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
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = ValentineFormDbName,
                    FormSourceId = ValentineFormId,
                    Money = 1000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 10,
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
                    dbName = QueensPanties,
                    EquippedThisTurn = false,
                    IsEquipped = false,
                    IsPermanent = false,
                    Level = 11,
                    OwnerId = id,
                    PvPEnabled = -1,
                    ItemSourceId = ItemStatics.GetStaticItem(QueensPanties).Id
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

                var victoryMessage = "'It's over!' - you yell in a thrill, already feeling the excitement of your victory over this sly, old fox as you lunge at him at point blank range, your palm tightly clenching a spell to finish it off. However, something is wrong. Grin, that paints itself on his lips, states about anything but an imminent defeat. His movements change, becoming more fuid, unreadable, as he steps to the side - or teleports? you don't even notice - and sticks his leg out, providing you with wonderful opportunity to trip over it. Opportunity that you, of course, took, falling over onto the floor, barely managing to not smash your face on it. Immediately after two blades tickle your neck, and the his voice agrees with your previous statement: - 'Indeed, it's over.' - suddenly the sharp steel by your throat vanishes into thin air as the man laughs heartily: - 'I admire your passion, young one.' - as you get up, you look at him and see a sincere smile on his face, as he continues: - 'Mind calling it a draw for today? It won't work good for my reputation if other's would know how you've beaten me.' - he winks. - 'And here's a little prize for your effort. Something very special, That I most certainly do not just trying to get rid of before i got caught...' - he reaches into his pocket, and gives you... a pair of panties. Before you can object or question this 'gift', he explains: - 'Thise are not just any panties. They belong to the Queen herself... so if i were you I'd keep them hidden to avoid being tuurned into a matching bra...' - before he could finish, a loud, furious woman's voice echoes through the room: - 'Israel Victis Valentine!!!.. Care to explain yourself?!!' - the mans face goes noticeably paler than it was before as he whispers to you: - 'Run! Run, I'll distract her!' - as you are snraking out through the other door, you can hear his voice, growing distant: - 'Oh, dear, i did expect you to wake up so soon...'";

                PlayerLogProcedures.AddPlayerLog(human.Id, victoryMessage, true);

                EndEvent(human.Id);
            }

            // Valentine is fine, do counterattack
            else
            {

                // regular counterattacks, not berserk
                if (valentine.Health > valentine.MaxHealth / 4)
                {
                    AttackProcedures.Attack(valentine, human, SwordSpell);
                    AttackProcedures.Attack(valentine, human, SwordSpell);
                    AIProcedures.DealBossDamage(valentine, human, false, 2);
                }

                // berserk mode counterattack
                else
                {

                    // counterattack twice against original attacker
                    AttackProcedures.Attack(valentine, human, SwordSpell);
                    AttackProcedures.Attack(valentine, human, SwordSpell);
                    AIProcedures.DealBossDamage(valentine, human, false, 1);

                    // attack everyone else with 1 cast
                    var playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

                    playersHere = playersHere.Where(p => p.Mobility == PvPStatics.MobilityFull && 
                    !PlayerProcedures.PlayerIsOffline(p) &&
                    p.Level >= 3 && 
                    p.BotId == AIStatics.ActivePlayerBotId && 
                    p.Id != valentine.Id && 
                    p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();

                    foreach (var p in playersHere)
                    {
                        AttackProcedures.Attack(valentine, p, SwordSpell);
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
                AIProcedures.MoveTo(valentine, locationToBe, 99999);
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

            foreach (var p in playersHere)
            {
                
                // give this player the vampire curse if they do not yet have it
                if (!EffectProcedures.PlayerHasEffect(p, BloodyKissEffect))
                {
                    AttackProcedures.Attack(valentine, p, BloodyCurseSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 1);
                }

                // give this player the immobility curse if they do not yet have it
                if (!EffectProcedures.PlayerHasEffect(p, ValentinesPresenceEffect))
                {
                    AttackProcedures.Attack(valentine, p, ValentinesPresenceSpell);
                }

            }

            // have Valentine equip his two strongest swords
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<Item> valentineSwords = itemRepo.Items.Where(i => i.OwnerId == valentine.Id && i.dbName != QueensPanties).OrderByDescending(i => i.Level);
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


            if (stance == BossProcedures_Valentine.DayStance)
            {
                if (player.Form != DayVampireFemaleForm && player.Form != DayVampireMaleForm)
                {
                    if (player.Gender == PvPStatics.GenderMale)
                    {
                        AttackProcedures.Attack(valentine, player, DayVampireFemaleSpell);
                    }
                    else
                    {
                        AttackProcedures.Attack(valentine, player, DayVampireMaleSpell);
                    }
                }
            }
            else if (stance == BossProcedures_Valentine.NightStance)
            {
                if (player.Form != NightVampireFemaleForm && player.Form != NightVampireMaleForm)
                {
                    if (player.Gender == PvPStatics.GenderMale)
                    {
                        AttackProcedures.Attack(valentine, player, NightVampireFemaleSpell);
                    }
                    else
                    {
                        AttackProcedures.Attack(valentine, player, NightVampireMaleSpell);
                    }
                }
            }
        }

        public static void EndEvent(int newOwnerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            var valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);
            

            var panties = itemRepo.Items.FirstOrDefault(i => i.dbName == QueensPanties);
            panties.OwnerId = newOwnerId;
            itemRepo.SaveItem(panties);

            DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = valentine.Id });
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

            if (turnNum % (DayNightInterval * 2) < DayNightInterval)
            {
                return DayStance;
            }
            else
            {
                return NightStance;
            }

        }

        public static string GetStanceLocation()
        {
            var stance = GetStance();
            if (stance == DayStance)
            {
                return "castle_training";
            }
            else
            {
                return "castle_tower";
            }
        }

        public static bool IsAttackableInForm(Player attacker, Player valentine)
        {
            var stance = GetStance();

            // Day stance:  Only day vampires can attack Valentine
            if (stance == DayStance)
            {
                if (attacker.Form == DayVampireMaleForm || attacker.Form == DayVampireFemaleForm)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Night stance:  Only night vampires can attack Valentine
            else
            {
                if (attacker.Form == NightVampireMaleForm || attacker.Form == NightVampireFemaleForm)
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
            return "Lord Valentine takes in your form with a studious and suggestive look. \"I'm afraid you are not yet ready for this hour’s training, child. Perhaps a few adjustments here...\" He caresses your cheek with a finger. \"...and here.\" His next touch leads down your neck with a chilling tingle that stops at your cleavage. In his trail there is marble white flesh, chilled yet so sensitive you can feel the Lords breath washing down upon it. When did he get so close...?";
        }

    }
}
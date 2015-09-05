using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_Valentine
    {

        // -5 Lord Valentine

        private const string ValentineFirstName = "Lord 'Teaserael'";
        private const string ValentineLastName = "Valentine";

        public const string ValentineFormDbName = "form_First_Lord_of_the_Valentine_Castle_Valentine's_Family";
        public const string SwordSpell = "skill_The_Dance_of_Blades_Ashley_Valentine";

        public const string FemaleVampSpell = "skill_Mistress_of_the_night_Foxpower93";
        public const string MaleVampSpell = "skill_Dark_Baptism_Blood_Knight";


        private const string MaleVampFormDbName = "form_Vampire_Lord_Blood_Knight";
        private const string FemaleVampFormDbName = "form_Vampire_Lord_Blood_Knight";

        public const string BloodyCurseSpell = "skill_A_Bloody_Curse";
        private const string BloodyKissEffect = "effect_A_Bloody_Kiss_Lilith";

        public const string ValentinesPresenceSpell = "skill_Valentine's_Presence_Lilith";
        private const string ValentinesPresenceEffect = "effect_Valentine’s_Presence_Lilith";


        public const string QueensPanties = "item_Queen_Valentine’s_Panties_Ashley_Valentine";

        private const int DayNightInterval = 2;

        public const string DayStance = "daystance";
        public const string NightStance = "nightstance";

        // NIGHT -- male
        public const string NightVampireMaleSpell = "skill_Wisdom_of_the_Nightkin._Leia_Valentine";
        public const string NightVampireMaleForm = "form_Child_of_the_Night_Leia_Valentine";

        // NIGHT -- female
        public const string NightVampireFemaleSpell = "skill_Elegance_of_the_Nightkin_Leia_Valentine";
        public const string NightVampireFemaleForm = "form_Disciple_of_the_night_Leia_Valentine";

        // DAY -- male
        public const string DayVampireMaleSpell = "skill_Strength_of_the_Nightkin_Leia_Valentine";
        public const string DayVampireMaleForm = "form_Vampire_Fighter_Leia_Valentine";

        // DAY -- female
        public const string DayVampireFemaleSpell = "skill_Prowess_of_the_Nightkin_Leia_Valentine";
        public const string DayVampireFemaleForm = "form_Vampire_Duelist_Leia_Valentine";

        public static void SpawnValentine()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);

            if (valentine == null)
            {
                valentine = new Player()
                {
                    FirstName = ValentineFirstName,
                    LastName = ValentineLastName,
                    ActionPoints = 120,
                    dbLocationName = GetStanceLocation(),
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "male",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = ValentineFormDbName,
                    Money = 1000,
                    Mobility = "full",
                    Level = 10,
                    MembershipId = AIStatics.ValentineBotId.ToString(),
                    BotId = AIStatics.ValentineBotId,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(valentine);

                valentine = PlayerProcedures.ReadjustMaxes(valentine, ItemProcedures.GetPlayerBuffsSQL(valentine));

                playerRepo.SavePlayer(valentine);

                // give Valentine his skills
                valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);

                // give Valentine the underwear that he drops
                IItemRepository itemRepo = new EFItemRepository();

                Item panties = new Item
                {
                    dbLocationName = "",
                    dbName = QueensPanties,
                    EquippedThisTurn = false,
                    IsEquipped = false,
                    IsPermanent = false,
                    Level = 11,
                    OwnerId = valentine.Id,
                    TimeDropped = DateTime.UtcNow,
                    PvPEnabled = -1,
                    VictimName = "",
                    LastSouledTimestamp = DateTime.UtcNow.AddYears(-1),
                };

                itemRepo.SaveItem(panties);

                // save his aiDirective, just for the sake of knowing his spawn turn
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective directive = new AIDirective
                {
                    OwnerId = valentine.Id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                };

                aiRepo.SaveAIDirective(directive);


            }
        }

        public static void CounterAttack(Player human, Player valentine)
        {

            AIProcedures.DealBossDamage(valentine, human, true, 1);

            // if Valentine's willpower is down to zero, have him hand over the panties and vanish.
            if (valentine.Health <= 0)
            {

                string victoryMessage = "'It's over!' - you yell in a thrill, already feeling the excitement of your victory over this sly, old fox as you lunge at him at point blank range, your palm tightly clenching a spell to finish it off. However, something is wrong. Grin, that paints itself on his lips, states about anything but an imminent defeat. His movements change, becoming more fuid, unreadable, as he steps to the side - or teleports? you don't even notice - and sticks his leg out, providing you with wonderful opportunity to trip over it. Opportunity that you, of course, took, falling over onto the floor, barely managing to not smash your face on it. Immediately after two blades tickle your neck, and the his voice agrees with your previous statement: - 'Indeed, it's over.' - suddenly the sharp steel by your throat vanishes into thin air as the man laughs heartily: - 'I admire your passion, young one.' - as you get up, you look at him and see a sincere smile on his face, as he continues: - 'Mind calling it a draw for today? It won't work good for my reputation if other's would know how you've beaten me.' - he winks. - 'And here's a little prize for your effort. Something very special, That I most certainly do not just trying to get rid of before i got caught...' - he reaches into his pocket, and gives you... a pair of panties. Before you can object or question this 'gift', he explains: - 'Thise are not just any panties. They belong to the Queen herself... so if i were you I'd keep them hidden to avoid being tuurned into a matching bra...' - before he could finish, a loud, furious woman's voice echoes through the room: - 'Israel Victis Valentine!!!.. Care to explain yourself?!!' - the mans face goes noticeably paler than it was before as he whispers to you: - 'Run! Run, I'll distract her!' - as you are snraking out through the other door, you can hear his voice, growing distant: - 'Oh, dear, i did expect you to wake up so soon...'";

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
                    if (EffectProcedures.PlayerHasEffect(human, ValentinesPresenceEffect) == false)
                    {
                        AttackProcedures.Attack(valentine, human, ValentinesPresenceSpell);
                    }
                }

                // berserk mode counterattack
                else
                {

                    List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

                    playersHere = playersHere.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.Level >= 3 && p.BotId == AIStatics.ActivePlayerBotId && p.Id != valentine.Id).ToList();

                    foreach (Player p in playersHere.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.Level >= 3))
                    {
                        AttackProcedures.Attack(valentine, p, SwordSpell);
                        AttackProcedures.Attack(valentine, p, SwordSpell);
                        AttackProcedures.Attack(valentine, p, SwordSpell);
                        AIProcedures.DealBossDamage(valentine, p, false, 3);
                    }
                }
            }
        }

        public static void RunValentineActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);

            // if valentine is not in the right place have him move to the other location
            string locationToBe = GetStanceLocation();
            if (valentine.dbLocationName != locationToBe)
            {
                AIProcedures.MoveTo(valentine, locationToBe, 99999);
                valentine.dbLocationName = locationToBe;
                playerRepo.SavePlayer(valentine);
            }

            // get all of the players in the room
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

            playersHere = playersHere.Where(p => p.Mobility == "full" &&
                PlayerProcedures.PlayerIsOffline(p) == false &&
                p.Level >= 3 &&
                p.BotId == AIStatics.ActivePlayerBotId &&
                p.Id != valentine.Id &&
                p.InDuel <= 0).ToList();

            int turnNo = PvPWorldStatProcedures.GetWorldTurnNumber();

            foreach (Player p in playersHere)
            {
                // turn male nonvamps into male vamps, and male vamps into sword
                if (p.Gender == "male" && p.Form != MaleVampFormDbName)
                {
                    AttackProcedures.Attack(valentine, p, MaleVampSpell);
                    AttackProcedures.Attack(valentine, p, MaleVampSpell);
                    AttackProcedures.Attack(valentine, p, MaleVampSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 3);
                }
                else if (p.Gender == "male" && p.Form == MaleVampFormDbName)
                {
                    AttackProcedures.Attack(valentine, p, SwordSpell);
                    AttackProcedures.Attack(valentine, p, SwordSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 2);
                }


                // turn female nonvamps into female vamps, and female vamps into sword
                else if (p.Gender == "female" && p.Form != FemaleVampSpell)
                {
                    AttackProcedures.Attack(valentine, p, FemaleVampSpell);
                    AttackProcedures.Attack(valentine, p, FemaleVampSpell);
                    AttackProcedures.Attack(valentine, p, FemaleVampSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 3);
                }
                else if (p.Gender == "female" && p.Form == FemaleVampSpell)
                {
                    AttackProcedures.Attack(valentine, p, SwordSpell);
                    AttackProcedures.Attack(valentine, p, SwordSpell);
                    AttackProcedures.Attack(valentine, p, SwordSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 3);
                }

                // give this player the vampire curse if they do not yet have it
                if (EffectProcedures.PlayerHasEffect(p, BloodyKissEffect) == false)
                {
                    AttackProcedures.Attack(valentine, p, BloodyCurseSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 1);
                }

                Random rand = new Random();
                double roll = rand.NextDouble();

                if (turnNo % 3 == 0 && EffectProcedures.PlayerHasEffect(p, ValentinesPresenceEffect) == false)
                {
                    AttackProcedures.Attack(valentine, p, ValentinesPresenceSpell);
                }


            }

            // have Valentine equip his two strongest swords
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<Item> valentineSwords = itemRepo.Items.Where(i => i.OwnerId == valentine.Id && i.dbName != QueensPanties).OrderByDescending(i => i.Level);
            List<Item> swordsToSave = new List<Item>();

            int counter = 1;
            foreach (Item sword in valentineSwords)
            {
                if (sword.IsEquipped == false && counter < 3)
                {
                    sword.IsEquipped = true;
                    swordsToSave.Add(sword);
                }
                else if (sword.IsEquipped == true && counter >= 3)
                {
                    sword.IsEquipped = false;
                    swordsToSave.Add(sword);
                }
                counter++;
            }

            foreach (Item sword in swordsToSave)
            {
                itemRepo.SaveItem(sword);
            }

        }

        public static void EndEvent(int newOwnerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            Player valentine = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.ValentineBotId);
            playerRepo.DeletePlayer(valentine.Id);

            Item panties = itemRepo.Items.FirstOrDefault(i => i.dbName == QueensPanties);
            panties.OwnerId = newOwnerId;
            itemRepo.SaveItem(panties);

            ItemProcedures.DropAllItems(valentine);

            PvPWorldStatProcedures.Boss_EndValentine();

            // find the players who dealt the most damage and award them with XP
            List<BossDamage> damages = AIProcedures.GetTopAttackers(valentine.BotId, 15);

            // top player gets 500 XP, each player down the line receives 25 fewer
            int l = 0;
            int maxReward = 500;

            foreach (BossDamage damage in damages)
            {
                Player victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                int reward = maxReward - (l * 30);
                victor.XP += reward;
                l++;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your valiant (maybe foolish?) efforts in challenging " + valentine.GetFullName() + " you receieve " + reward + " XP from your risky struggle!</b>", true);

                playerRepo.SavePlayer(victor);

            }

        }

        public static string GetStance()
        {
            int turnNum = PvPWorldStatProcedures.GetWorldTurnNumber();

            if (turnNum % (DayNightInterval*2) < DayNightInterval)
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
            string stance = GetStance();
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
            string stance = GetStance();

            // Day stance:  Only night vampires can attack Valentine
            if (stance == DayStance)
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

            // Night stance:  Only day vampires can attack Valentine
            else
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
        }

    }
}
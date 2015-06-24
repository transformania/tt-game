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

        public static void SpawnValentine()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player valentine = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lord 'Teaserael'" && f.LastName == "Valentine");

            if (valentine == null)
            {
                valentine = new Player()
                {
                    FirstName = "Lord 'Teaserael'",
                    LastName = "Valentine",
                    ActionPoints = 120,
                    dbLocationName = "castle_armory",
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
                    Form = "form_First_Lord_of_the_Valentine_Castle_Valentine's_Family",
                    IsPetToId = -1,
                    Money = 1000,
                    Mobility = "full",
                    Level = 10,
                    MembershipId = -5,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(valentine);

                valentine = PlayerProcedures.ReadjustMaxes(valentine, ItemProcedures.GetPlayerBuffsSQL(valentine));

                playerRepo.SavePlayer(valentine);

                // give Valentine his skills
                valentine = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lord 'Teaserael'" && f.LastName == "Valentine");
                DbStaticSkill skillToAdd = SkillStatics.GetStaticSkill("skill_The_Dance_of_Blades_Ashley_Valentine");
                DbStaticSkill skillToAdd2 = SkillStatics.GetStaticSkill("skill_Mistress_of_the_night_Foxpower93");
                DbStaticSkill skillToAdd3 = SkillStatics.GetStaticSkill("skill_Dark_Baptism_Blood_Knight");
                DbStaticSkill skillToAdd4 = SkillStatics.GetStaticSkill("skill_A_Bloody_Curse");
                DbStaticSkill skillToAdd5 = SkillStatics.GetStaticSkill("skill_Valentine's_Presence_Lilith");
                SkillProcedures.GiveSkillToPlayer(valentine.Id, skillToAdd);
                SkillProcedures.GiveSkillToPlayer(valentine.Id, skillToAdd2);
                SkillProcedures.GiveSkillToPlayer(valentine.Id, skillToAdd3);
                SkillProcedures.GiveSkillToPlayer(valentine.Id, skillToAdd4);
                SkillProcedures.GiveSkillToPlayer(valentine.Id, skillToAdd5);

                // give Valentine the underwear that he drops
                IItemRepository itemRepo = new EFItemRepository();

                Item panties = new Item
                {
                    dbLocationName = "",
                    dbName = "item_Queen_Valentine’s_Panties_Ashley_Valentine",
                    EquippedThisTurn = false,
                    IsEquipped = false,
                    IsPermanent = false,
                    Level = 11,
                    OwnerId = valentine.Id,
                    TimeDropped = DateTime.UtcNow,
                    PvPEnabled = -1,
                    VictimName = "",
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
                IItemRepository itemRepo = new EFItemRepository();
                Item panties = itemRepo.Items.FirstOrDefault(i => i.dbName == "item_Queen_Valentine’s_Panties_Ashley_Valentine");
                panties.OwnerId = human.Id;
                itemRepo.SaveItem(panties);

                ItemProcedures.DropAllItems(valentine);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                playerRepo.DeletePlayer(valentine.Id);
                PvPWorldStatProcedures.Boss_EndValentine();

                string victoryMessage = "'It's over!' - you yell in a thrill, already feeling the excitement of your victory over this sly, old fox as you lunge at him at point blank range, your palm tightly clenching a spell to finish it off. However, something is wrong. Grin, that paints itself on his lips, states about anything but an imminent defeat. His movements change, becoming more fuid, unreadable, as he steps to the side - or teleports? you don't even notice - and sticks his leg out, providing you with wonderful opportunity to trip over it. Opportunity that you, of course, took, falling over onto the floor, barely managing to not smash your face on it. Immediately after two blades tickle your neck, and the his voice agrees with your previous statement: - 'Indeed, it's over.' - suddenly the sharp steel by your throat vanishes into thin air as the man laughs heartily: - 'I admire your passion, young one.' - as you get up, you look at him and see a sincere smile on his face, as he continues: - 'Mind calling it a draw for today? It won't work good for my reputation if other's would know how you've beaten me.' - he winks. - 'And here's a little prize for your effort. Something very special, That I most certainly do not just trying to get rid of before i got caught...' - he reaches into his pocket, and gives you... a pair of panties. Before you can object or question this 'gift', he explains: - 'Thise are not just any panties. They belong to the Queen herself... so if i were you I'd keep them hidden to avoid being tuurned into a matching bra...' - before he could finish, a loud, furious woman's voice echoes through the room: - 'Israel Victis Valentine!!!.. Care to explain yourself?!!' - the mans face goes noticeably paler than it was before as he whispers to you: - 'Run! Run, I'll distract her!' - as you are snraking out through the other door, you can hear his voice, growing distant: - 'Oh, dear, i did expect you to wake up so soon...'";

                PlayerLogProcedures.AddPlayerLog(human.Id, victoryMessage, true);
            }

            // Valentine is fine, do counterattack
            else {

                // regular counterattacks, not berserk
                if (valentine.Health > valentine.MaxHealth / 4)
                {
                    SkillViewModel2 danceOfBlades = SkillProcedures.GetSkillViewModel("skill_The_Dance_of_Blades_Ashley_Valentine", valentine.Id);
                    AttackProcedures.Attack(valentine, human, danceOfBlades);
                    AttackProcedures.Attack(valentine, human, danceOfBlades);
                    AIProcedures.DealBossDamage(valentine, human, false, 2);
                    if (EffectProcedures.PlayerHasEffect(human, "Valentine's_Presence_Lilith") == false)
                    {
                        SkillViewModel2 valentinesPresence = SkillProcedures.GetSkillViewModel("skill_Valentine's_Presence_Lilith", valentine.Id);
                        AttackProcedures.Attack(valentine, human, valentinesPresence);
                    }
                }

                // berserk mode counterattack
                else
                {

                    SkillViewModel2 danceOfBlades = SkillProcedures.GetSkillViewModel("skill_The_Dance_of_Blades_Ashley_Valentine", valentine.Id);
                    List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

                    playersHere = playersHere.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.Level >= 3 && p.MembershipId > 0 && p.Id != valentine.Id).ToList();

                    foreach (Player p in playersHere.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.Level >= 3))
                    {
                        AttackProcedures.Attack(valentine, p, danceOfBlades);
                        AttackProcedures.Attack(valentine, p, danceOfBlades);
                        AttackProcedures.Attack(valentine, p, danceOfBlades);
                        AIProcedures.DealBossDamage(valentine, p, false, 3);
                    }
                }
            }
        }

        public static void RunValentineActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player valentine = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lord 'Teaserael'" && f.LastName == "Valentine");

            // get all of the players in the room
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(valentine.dbLocationName).ToList();

            playersHere = playersHere.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.Level >= 3 && p.MembershipId > 0 && p.Id != valentine.Id).ToList();

            int turnNo = PvPWorldStatProcedures.GetWorldTurnNumber();

            foreach (Player p in playersHere)
            {
                // turn male nonvamps into male vamps, and male vamps into sword
                if (p.Gender == "male" && p.Form != "form_Vampire_Lord_Blood_Knight")
                {
                    SkillViewModel2 maleVampSpell = SkillProcedures.GetSkillViewModel("skill_Dark_Baptism_Blood_Knight", valentine.Id);
                    AttackProcedures.Attack(valentine, p, maleVampSpell);
                    AttackProcedures.Attack(valentine, p, maleVampSpell);
                    AttackProcedures.Attack(valentine, p, maleVampSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 3);
                }
                else if (p.Gender == "male" && p.Form == "form_Vampire_Lord_Blood_Knight")
                {
                    SkillViewModel2 danceOfBlades = SkillProcedures.GetSkillViewModel("skill_The_Dance_of_Blades_Ashley_Valentine", valentine.Id);
                    AttackProcedures.Attack(valentine, p, danceOfBlades);
                    AttackProcedures.Attack(valentine, p, danceOfBlades);
                    AIProcedures.DealBossDamage(valentine, p, false, 2);
                }


                // turn female nonvamps into female vamps, and female vamps into sword
                else if (p.Gender == "female" && p.Form != "form_Seductive_vampire_Foxpower93")
                {
                    SkillViewModel2 femaleVampSpell = SkillProcedures.GetSkillViewModel("skill_Mistress_of_the_night_Foxpower93", valentine.Id);
                    AttackProcedures.Attack(valentine, p, femaleVampSpell);
                    AttackProcedures.Attack(valentine, p, femaleVampSpell);
                    AttackProcedures.Attack(valentine, p, femaleVampSpell);
                    AIProcedures.DealBossDamage(valentine, p, false, 3);
                }
                else if (p.Gender == "female" && p.Form == "form_Seductive_vampire_Foxpower93")
                {
                    SkillViewModel2 danceOfBlades = SkillProcedures.GetSkillViewModel("skill_The_Dance_of_Blades_Ashley_Valentine", valentine.Id);
                    AttackProcedures.Attack(valentine, p, danceOfBlades);
                    AttackProcedures.Attack(valentine, p, danceOfBlades);
                    AttackProcedures.Attack(valentine, p, danceOfBlades);
                    AIProcedures.DealBossDamage(valentine, p, false, 3);
                }

                // give this player the vampire curse if they do not yet have it
                if (EffectProcedures.PlayerHasEffect(p, "skill_a_bloody_kiss_Lilith") == false)
                {
                    // TODO:  HAVE VALENTINE ACTUALLY CAST THIS AS A SPELL
                    //EffectProcedures.GivePerkToPlayer("skill_a_bloody_kiss_Lilith", p);
                    SkillViewModel2 bloodyKiss = SkillProcedures.GetSkillViewModel("skill_A_Bloody_Curse", valentine.Id);
                    AttackProcedures.Attack(valentine, p, bloodyKiss);
                    AIProcedures.DealBossDamage(valentine, p, false, 1);
                }

                Random rand = new Random();
                double roll = rand.NextDouble();

                if (turnNo % 3 == 0 && EffectProcedures.PlayerHasEffect(p, "Valentine's_Presence_Lilith") == false)
                {
                   SkillViewModel2 valentinesPresence = SkillProcedures.GetSkillViewModel("skill_Valentine's_Presence_Lilith", valentine.Id);
                   AttackProcedures.Attack(valentine, p, valentinesPresence);
                }


            }

            // have Valentine equip his two strongest swords
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<Item> valentineSwords = itemRepo.Items.Where(i => i.OwnerId == valentine.Id && i.dbName != "item_Queen_Valentine’s_Panties_Ashley_Valentine").OrderByDescending(i => i.Level);
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

    }
}
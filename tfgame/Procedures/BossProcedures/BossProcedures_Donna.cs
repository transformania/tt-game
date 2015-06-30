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
    
    public class BossProcedures_Donna
    {

        private const int DonnaSpellCount = 5;
        private const string Spell1 = "skill_Donna's_Bitch_LexamTheGemFox";
        private const string Spell2 = "skill_Donna's_Cow_LexamtheGemFox";
        private const string Spell3 = "skill_Donna's_Pig_LexamtheGemFox";
        private const string Spell4 = "skill_Donna's_Mare_LexamtheGemFox";
        private const string Spell5 = "skill_Donna's_Chicken_LexamtheGemFox";

        public static void SpawnDonna()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player donna = playerRepo.Players.FirstOrDefault(f => f.FirstName == "'Aunt' Donna" && f.LastName == "Milton");

            if (donna == null)
            {
                donna = new Player()
                {
                    FirstName = "'Aunt' Donna",
                    LastName = "Milton",
                    ActionPoints = 120,
                    dbLocationName = "ranch_bedroom",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_Mythical_Sorceress_LexamTheGemFox",
                    //IsPetToId = -1,
                    Money = 1000,
                    Mobility = "full",
                    Level = 20,
                    MembershipId = -4,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(donna);

                donna = PlayerProcedures.ReadjustMaxes(donna, ItemProcedures.GetPlayerBuffsSQL(donna));

                playerRepo.SavePlayer(donna);

            }



        }

        public static void RunDonnaActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IServerLogRepository serverLogRepo = new EFServerLogRepository();

            int worldTurnNumber = PvPWorldStatProcedures.GetWorldTurnNumber() - 1;
            ServerLog log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == worldTurnNumber);

            Player donna = playerRepo.Players.FirstOrDefault(p => p.MembershipId == -4);

            if (donna.Mobility != "full")
            {
                EndEvent(donna);
            }

            else if (donna.Mobility == "full")
            {

                donna.Form = "form_Mythical_Sorceress_LexamTheGemFox";

                BuffBox donnasBuffs = ItemProcedures.GetPlayerBuffsSQL(donna);

                // have donna meditate and cleanse if needed
                if (donna.Health < donna.MaxHealth / 6)
                {
                    PlayerProcedures.Cleanse(donna, donnasBuffs);
                }
                if (donna.Mana < donna.MaxMana)
                {
                    PlayerProcedures.Meditate(donna, donnasBuffs);
                }

                AIDirective directive = AIDirectiveProcedures.GetAIDirective(donna.Id);

                if (directive.State == "attack" || directive.State == "idle")
                {



                    Player target = playerRepo.Players.FirstOrDefault(p => p.Id == directive.TargetPlayerId);



                    // if Donna's target goes offline, is inanimate, or in the dungeon, have her teleport back to the ranch
                    if (target == null || target.Mobility != "full" || PlayerProcedures.PlayerIsOffline(target) || target.IsInDungeon() == true)
                    {

                        if (donna.dbLocationName != "ranch_bedroom")
                        {
                            LocationLogProcedures.AddLocationLog(donna.dbLocationName, donna.FirstName + " " + donna.LastName + " vanished from here in a flash of smoke.");
                            donna.dbLocationName = "ranch_bedroom";
                            LocationLogProcedures.AddLocationLog(donna.dbLocationName, donna.FirstName + " " + donna.LastName + " appeared here in a flash of smoke.");
                            playerRepo.SavePlayer(donna);
                        }


                        AIDirectiveProcedures.SetAIDirective_Idle(donna.Id);

                    }

                    // Donna has a valid target; go chase it down and attack.  Donna does not look for new targets.
                    else
                    {
                        string newplace = AIProcedures.MoveTo(donna, target.dbLocationName, 9);
                        donna.dbLocationName = newplace;
                        playerRepo.SavePlayer(donna);

                        if (target.dbLocationName == newplace)
                        {

                            Random rand = new Random();
                            double roll = rand.NextDouble() * 3 + 2;
                            for (int i = 0; i < roll; i++)
                            {
                                AttackProcedures.Attack(donna, target, ChooseSpell(PvPStatics.LastGameTurn));
                                
                            }
                            AIProcedures.DealBossDamage(donna, target, false, (int)roll);
                        }
                        else
                        {
                            
                        }

                    }
                }
                else
                {
                    
                }

                // have Donna equip all the pets she owns
                IItemRepository itemRepo = new EFItemRepository();
                IEnumerable<Item> donnasItems = itemRepo.Items.Where(i => i.OwnerId == donna.Id && i.IsEquipped == false && i.Level > 3);
                List<Item> itemsToEquip = new List<Item>();
                foreach (Item i in donnasItems)
                {
                    itemsToEquip.Add(i);
                }
                foreach (Item i in itemsToEquip)
                {
                    i.IsEquipped = true;
                    i.dbLocationName = donna.dbLocationName;
                    itemRepo.SaveItem(i);
                }

                List<ItemViewModel> donnasPlayerPets = ItemProcedures.GetAllPlayerItems(donna.Id).ToList();

                //List<Player> donnasPlayerPets = playerRepo.Players.Where(p => p.IsPetToId == donna.Id).ToList();
                //List<Player> donnasPlayerPetsToSave = new List<Player>();
                //foreach (Player p in donnasPlayerPets)
                //{
                //    p.dbLocationName = donna.dbLocationName;
                //    donnasPlayerPetsToSave.Add(p);
                //}
                //foreach (Player p in donnasPlayerPetsToSave)
                //{
                //    playerRepo.SavePlayer(p);
                //}

                // have Donna release her weakest pet every so often
                if (worldTurnNumber % 6 == 0 && donnasPlayerPets.Count() > 0)
                {
                    IEnumerable<Item> weakest = itemRepo.Items.Where(i => i.OwnerId == donna.Id).OrderBy(i => i.Level);
                    Item weakestItem = weakest.First();
                    ItemProcedures.DropItem(weakestItem.Id, donna.dbLocationName);
                    LocationLogProcedures.AddLocationLog(donna.dbLocationName, "Donna released one of her weaker pets, " + weakestItem.GetFullName() + ", here.");
                    Player luckyVictim = PlayerProcedures.GetPlayerWithExactName(weakestItem.VictimName);
                    PlayerLogProcedures.AddPlayerLog(luckyVictim.Id, "Donna has released you, allowing you to wander about or be tamed by a new owner.", true);
                }
                serverLogRepo.SaveServerLog(log);

            }

        }

        public static void DonnaCounterattack(Player personAttacking, Player donna)
        {
            AIProcedures.DealBossDamage(donna, personAttacking, true, 1);

            Random rand = new Random();
            double roll = rand.NextDouble() * 4;

            for (int i = 0; i < roll; i++)
            {
                AttackProcedures.Attack(donna, personAttacking, ChooseSpell(PvPStatics.LastGameTurn));
            }

            AIProcedures.DealBossDamage(donna, personAttacking, false, (int)roll);

            // if Donna is weak enough start having her mega-attack anyone in the room
            if (donna.Health < donna.MaxHealth / 5)
            {
                List<Player> PlayersHere = PlayerProcedures.GetPlayersAtLocation(donna.dbLocationName).ToList();

                foreach (Player p in PlayersHere)
                {
                    if (p.MembershipId > 0 && p.Level > 3 && p.Mobility == "full" && !PlayerProcedures.PlayerIsOffline(p) && p.Id != personAttacking.Id)
                    {
                        AttackProcedures.Attack(donna, p, ChooseSpell(PvPStatics.LastGameTurn));
                        AIProcedures.DealBossDamage(donna, p, false, 1);
                    }
                }


            }


            //AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);
            AIDirective directive = AIDirectiveProcedures.GetAIDirective(donna.Id);

            // if Donna has no target or by a random chance, make her target this attacker
            if (directive.TargetPlayerId == -1 || directive.State == "idle" || roll < 1)
            {
                AIDirectiveProcedures.SetAIDirective_Attack(donna.Id, personAttacking.Id);
            }
        }

        public static string ChooseSpell(int turnNumber)
        {

            int mod = turnNumber % (3*DonnaSpellCount);

            if (mod >= 0 && mod < 3)
            {
                return Spell1;
            }
            else if (mod >= 4 && mod < 6)
            {
                return Spell2;
            }
            else if (mod >= 7 && mod < 9)
            {
                return Spell3;
            }
            else if (mod >= 10 && mod < 12)
            {
                return Spell4;
            }
            else if (mod >= 13 && mod < 15)
            {
                return Spell5;
            }
            else
            {
                return Spell1;
            }

        }

        public static void EndEvent(Player donna)
        {
            PvPWorldStatProcedures.Boss_EndDonna();
            List<BossDamage> damages = AIProcedures.GetTopAttackers(-4, 20);
            IPlayerRepository playerRepo = new EFPlayerRepository();

            // top player gets 800 XP, each player down the line receives 35 fewer
            int i = 0;
            int maxReward = 800;

            foreach (BossDamage damage in damages)
            {
                Player victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                int reward = maxReward - (i * 40);
                victor.XP += reward;
                i++;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + donna.GetFullName() + ", you earn " + reward + " XP from your spells cast against the mythical sorceress.</b>", true);

                playerRepo.SavePlayer(victor);

            }
        }

    }
}
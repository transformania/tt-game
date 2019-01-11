using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    
    public class BossProcedures_Donna
    {

        private const string FirstName = "'Aunt' Donna";
        private const string LastName = "Milton";
        private const int DonnaSpellCount = 5;
        public const string DonnaDbForm = "form_Mythical_Sorceress_LexamTheGemFox";

        public const int Spell1 = 465;
        public const int Spell2 = 595;
        public const int Spell3 = 596;
        public const int Spell4 = 597;
        public const int Spell5 = 649;

        private const int DonnaFormId = 287;

        public static void SpawnDonna()
        {
            var donna = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId {BotId = AIStatics.DonnaBotId});

            if (donna == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Location = "ranch_bedroom",
                    Gender = PvPStatics.GenderFemale,
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = DonnaDbForm,
                    FormSourceId = DonnaFormId,
                    Money = 1000,
                    Level = 20,
                    BotId = AIStatics.DonnaBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var donnaEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                donnaEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(donnaEF));
                playerRepo.SavePlayer(donnaEF);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.DONNA_RUNE, PlayerId = donnaEF.Id });
                }

            }
        }

        public static void RunDonnaActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            var worldTurnNumber = PvPWorldStatProcedures.GetWorldTurnNumber() - 1;

            var donna = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.DonnaBotId);

            if (donna.Mobility != PvPStatics.MobilityFull)
            {
                EndEvent(donna);
            }

            else if (donna.Mobility == PvPStatics.MobilityFull)
            {

                var donnasBuffs = ItemProcedures.GetPlayerBuffs(donna);

                // have donna meditate
                if (donna.Mana < donna.MaxMana)
                {
                    DomainRegistry.Repository.Execute(new Meditate { PlayerId = donna.Id, Buffs = donnasBuffs, NoValidate = true});
                }

                var directive = AIDirectiveProcedures.GetAIDirective(donna.Id);

                if (directive.State == "attack" || directive.State == "idle")
                {

                    var target = playerRepo.Players.FirstOrDefault(p => p.Id == directive.TargetPlayerId);

                    // if Donna's target goes offline, is inanimate, or in the dungeon, have her teleport back to the ranch
                    if (target == null || 
                        target.Mobility != PvPStatics.MobilityFull ||
                        PlayerProcedures.PlayerIsOffline(target) || 
                        target.IsInDungeon() ||
                        target.InDuel > 0 ||
                        target.InQuest > 0)
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
                        var newplace = AIProcedures.MoveTo(donna, target.dbLocationName, 9);
                        donna.dbLocationName = newplace;
                        playerRepo.SavePlayer(donna);

                        if (target.dbLocationName == newplace)
                        {

                            var rand = new Random();
                            var roll = rand.NextDouble() * 3 + 2;
                            for (var i = 0; i < roll; i++)
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
                IEnumerable<Item> donnasItems = itemRepo.Items.Where(i => i.OwnerId == donna.Id && !i.IsEquipped && i.Level > 3);
                var itemsToEquip = new List<Item>();
                foreach (var i in donnasItems)
                {
                    itemsToEquip.Add(i);
                }
                foreach (var i in itemsToEquip)
                {
                    i.IsEquipped = true;
                    i.dbLocationName = donna.dbLocationName;
                    itemRepo.SaveItem(i);
                }

                var donnasPlayerPets = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer {OwnerId = donna.Id}).OrderBy(i => i.Level).ToList();

                // have Donna release her weakest pet every so often
                if (worldTurnNumber % 6 == 0 && donnasPlayerPets.Any())
                {
                    var weakestItem = donnasPlayerPets.First();
                    ItemProcedures.DropItem(weakestItem.Id);
                    LocationLogProcedures.AddLocationLog(donna.dbLocationName, "Donna released one of her weaker pets, " + weakestItem.FormerPlayer.FullName + ", here.");
                    var luckyVictim = PlayerProcedures.GetPlayerWithExactName(weakestItem.FormerPlayer.FullName);
                    PlayerLogProcedures.AddPlayerLog(luckyVictim.Id, "Donna has released you, allowing you to wander about or be tamed by a new owner.", true);
                }

            }

        }

        public static void DonnaCounterattack(Player personAttacking, Player donna)
        {
            AIProcedures.DealBossDamage(donna, personAttacking, true, 1);

            var rand = new Random();
            var roll = rand.NextDouble() * 4;

            for (var i = 0; i < roll; i++)
            {
                AttackProcedures.Attack(donna, personAttacking, ChooseSpell(PvPStatics.LastGameTurn));
            }

            AIProcedures.DealBossDamage(donna, personAttacking, false, (int)roll);

            // if Donna is weak enough start having her mega-attack anyone in the room
            if (donna.Health < donna.MaxHealth / 5)
            {
                var PlayersHere = PlayerProcedures.GetPlayersAtLocation(donna.dbLocationName).ToList();

                foreach (var p in PlayersHere)
                {
                    if (p.BotId == AIStatics.ActivePlayerBotId &&
                        p.Level > 3 && 
                        p.Mobility == PvPStatics.MobilityFull && 
                        !PlayerProcedures.PlayerIsOffline(p) &&
                        p.Id != personAttacking.Id &&
                        p.InDuel <= 0 &&
                        p.InQuest <= 0)
                    {
                        AttackProcedures.Attack(donna, p, ChooseSpell(PvPStatics.LastGameTurn));
                        AIProcedures.DealBossDamage(donna, p, false, 1);
                    }
                }


            }


            //AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);
            var directive = AIDirectiveProcedures.GetAIDirective(donna.Id);

            // if Donna has no target or by a random chance, make her target this attacker
            if (directive.TargetPlayerId == -1 || directive.State == "idle" || roll < 1)
            {
                AIDirectiveProcedures.SetAIDirective_Attack(donna.Id, personAttacking.Id);
            }
        }

        public static int ChooseSpell(int turnNumber)
        {

            var mod = turnNumber % (3*DonnaSpellCount);

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
            var damages = AIProcedures.GetTopAttackers(-4, 20);
            IPlayerRepository playerRepo = new EFPlayerRepository();

            // top player gets 800 XP, each player down the line receives 35 fewer
            var i = 0;
            var maxReward = 800;

            for (var r = 0; r < damages.Count; r++)
            {
                var damage = damages.ElementAt(r);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                var reward = maxReward - (i * 40);
                victor.XP += reward;
                i++;

                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + donna.GetFullName() + ", you earn " + reward + " XP from your spells cast against the mythical sorceress.</b>", true);

                playerRepo.SavePlayer(victor);

                // top three get runes
                if (r <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.DONNA_RUNE, PlayerId = victor.Id });
                }

            }
        }

    }
}
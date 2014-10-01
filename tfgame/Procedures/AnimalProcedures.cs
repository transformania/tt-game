using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;
using tfgame.Statics;

namespace tfgame.Procedures
{
    public static class AnimalProcedures
    {
        public static string DoAnimalAction(string actionName, int attackerId, int victimId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player attacker = playerRepo.Players.FirstOrDefault(p => p.Id == attackerId);
            Player victim = playerRepo.Players.FirstOrDefault(p => p.Id == victimId);

            Player attackerOwner = playerRepo.Players.FirstOrDefault(p => p.Id == attacker.IsPetToId);
            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == attacker.dbLocationName);


            PlayerFormViewModel attackerPlus = PlayerProcedures.GetPlayerFormViewModel(attackerId);
            PlayerFormViewModel victimPlus = PlayerProcedures.GetPlayerFormViewModel(victimId);

            string victimMessage = "";
            string attackerMessage = "";
            string locationMessage = "";

            string victimPronoun;
            if (victim.Gender == "male")
            {
                victimPronoun = "his";
            }
            else
            {
                victimPronoun = "her";
            }

            if (actionName == "snarl")
            {
                victim.Mana -= 1;

                if (victim.Mana < 0)
                {
                    victim.Mana = 0;
                }

                if (attackerOwner != null)
                {
                    victimMessage = attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.FirstName + " " + attackerOwner.LastName + ", snarls at you, slightly lowering your mana.";
                }
                else
                {
                    victimMessage = attacker.FirstName + " " + attacker.LastName + ", a feral " + attackerPlus.Form.FriendlyName + " snarls at you, slightly lowering your mana.";
                }

                attackerMessage = "You snarl at " + victim.FirstName + " " + victim.LastName + ", lowering " + victimPronoun + " mana slightly.";

                locationMessage = attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + ", snarled at " + victim.FirstName + " " + victim.LastName + ".";

            }
            else if (actionName == "lick")
            {
                victim.Health += 1;

                if (victim.Health > victim.MaxHealth)
                {
                    victim.Health = victim.MaxHealth;
                }

                if (attackerOwner != null)
                {
                    victimMessage = "<span style='color: blue'>" + attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.FirstName + " " + attackerOwner.LastName + ", gently licks you, slightly raising your willpower.</span>";
                }
                else
                {
                    victimMessage = "<span style='color: blue'>" + attacker.FirstName + " " + attacker.LastName + ", a feral " + attackerPlus.Form.FriendlyName + " gently licks you, slightly raising your willpower.";
                }

                attackerMessage = "You gently lick " + victim.FirstName + " " + victim.LastName + ", raising " + victimPronoun + " willpower slightly.";

                locationMessage = attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + ", licked " + victim.FirstName + " " + victim.LastName + ".";


            }
            
            else if (actionName == "nuzzle")
            {
                victim.Mana += 1;

                if (victim.Mana > victim.MaxMana)
                {
                    victim.Mana = victim.MaxMana;
                }

                if (attackerOwner != null)
                {
                    victimMessage = "<span style='color: blue'>" + attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.FirstName + " " + attackerOwner.LastName + ", gently nuzzles you, slightly raising your mana.";
                }
                else
                {
                    victimMessage = "<span style='color: blue'>" + attacker.FirstName + " " + attacker.LastName + ", a feral " + attackerPlus.Form.FriendlyName + " gently nuzzles you, slightly raising your mana.";
                }

                attackerMessage = "You gently nuzzle " + victim.FirstName + " " + victim.LastName + ", raising " + victimPronoun + " mana slightly.";

                locationMessage = attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + ", nuzzles " + victim.FirstName + " " + victim.LastName + ".";

            }

            if (actionName == "headbutt")
            {
                victim.Health -= 1;

                if (victim.Health < 0)
                {
                    victim.Health = 0;
                }

                if (attackerOwner != null)
                {
                    victimMessage = attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.FirstName + " " + attackerOwner.LastName + ", lightly headbutts you, slightly lowering your willpower.";
                }
                else
                {
                    victimMessage = attacker.FirstName + " " + attacker.LastName + ", a feral " + attackerPlus.Form.FriendlyName + " lightly headbutts you, slightly lowering your willpower.";
                }

                attackerMessage = "You headbutt " + victim.FirstName + " " + victim.LastName + ", lowering " + victimPronoun + " willpower slightly.";

                locationMessage = attacker.FirstName + " " + attacker.LastName + ", a " + attackerPlus.Form.FriendlyName + ", headbutted " + victim.FirstName + " " + victim.LastName + ".";

            }


       
            PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);
            PlayerLogProcedures.AddPlayerLog(attacker.Id, attackerMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationMessage);

            attacker.TimesAttackingThisUpdate++;
            playerRepo.SavePlayer(victim);
            playerRepo.SavePlayer(attacker);

            return attackerMessage;

        }

        public static void ChangeOwner(Item_VM animalItem, int ownerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player animal = playerRepo.Players.FirstOrDefault(p => p.FirstName + " " + p.LastName == animalItem.VictimName);

            animal.IsPetToId = ownerId;
            playerRepo.SavePlayer(animal);

        }

        public static void ChangeOwner(Item animalItem, int ownerId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player animal = playerRepo.Players.FirstOrDefault(p => p.FirstName + " " + p.LastName == animalItem.VictimName);

            animal.IsPetToId = ownerId;
            playerRepo.SavePlayer(animal);

        }

        //public static Location GetAnimalLocation(Player me, Player owner)
        //{
        //    if (me.IsPetToId != -1)
        //    {
        //        return LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == owner.dbLocationName);
        //    }
        //    else
        //    {
        //        return LocationsStatics.GetLocation.First(l => l.dbName == me.dbLocationName);
        //    }
        //}

        //public static Location GetAnimalLocation(Player me)
        //{
        //    IPlayerRepository playerRepo = new EFPlayerRepository();
        //    Player owner = playerRepo.Players.FirstOrDefault(p => p.Id == me.IsPetToId);
        //    return GetAnimalLocation(me, owner);
        //}


    }
}
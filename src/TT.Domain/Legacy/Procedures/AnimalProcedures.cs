﻿using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Statics;

namespace TT.Domain.Procedures
{
    public static class AnimalProcedures
    {
        public static string DoAnimalAction(string actionName, int animalPlayerId, int victimId, string message = "")
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var animalPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == animalPlayerId);

            var animalItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = animalPlayerId });

            var victim = playerRepo.Players.FirstOrDefault(p => p.Id == victimId);

            Player attackerOwner = null;

            if (animalItem.Owner != null) { 
                attackerOwner = playerRepo.Players.FirstOrDefault(p => p.Id == animalItem.Owner.Id);
            }

            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == animalPlayer.dbLocationName);


            var attackerPlus = PlayerProcedures.GetPlayerFormViewModel(animalPlayerId);

            if (attackerPlus == null)
            {
                return "";
            }

            var victimMessage = "";
            var attackerMessage = "";
            var locationMessage = "";

            string victimPronoun;
            if (victim.Gender == PvPStatics.GenderMale)
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
                    victimMessage = "<span class='petActionBad'>" + animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.GetFullName() + ", snarls at you, slightly lowering your mana.</span>";
                }
                else
                {
                    victimMessage = "<span class='petActionBad'>" + animalPlayer.GetFullName() + ", a feral " + attackerPlus.Form.FriendlyName + " snarls at you, slightly lowering your mana.</span>";
                }

                attackerMessage = "You snarl at " + victim.GetFullName() + ", lowering " + victimPronoun + " mana slightly.";

                locationMessage = animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + ", snarled at " + victim.GetFullName() + ".";

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
                    victimMessage = "<span class='petActionGood'>" + animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.FirstName + " " + attackerOwner.LastName + ", gently licks you, slightly raising your willpower.</span>";
                }
                else
                {
                    victimMessage = "<span class='petActionGood'>" + animalPlayer.GetFullName() + ", a feral " + attackerPlus.Form.FriendlyName + " gently licks you, slightly raising your willpower.</span>";
                }

                attackerMessage = "You gently lick " + victim.GetFullName() + ", raising " + victimPronoun + " willpower slightly.";

                locationMessage = animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + ", licked " + victim.FirstName + " " + victim.LastName + ".";


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
                    victimMessage = "<span class='petActionGood'>" + animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.FirstName + " " + attackerOwner.LastName + ", gently nuzzles you, slightly raising your mana.</span>";
                }
                else
                {
                    victimMessage = "<span class='petActionGood'>" + animalPlayer.GetFullName() + ", a feral " + attackerPlus.Form.FriendlyName + " gently nuzzles you, slightly raising your mana.</span>";
                }

                attackerMessage = "You gently nuzzle " + victim.GetFullName() + ", raising " + victimPronoun + " mana slightly.";

                locationMessage = animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + ", nuzzles " + victim.FirstName + " " + victim.LastName + ".";

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
                    victimMessage = "<span class='petActionBad'>" + animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + " kept as a pet by " + attackerOwner.GetFullName() + ", lightly headbutts you, slightly lowering your willpower.</span>";
                }
                else
                {
                    victimMessage = "<span class='petActionBad'>" + animalPlayer.GetFullName() + ", a feral " + attackerPlus.Form.FriendlyName + " lightly headbutts you, slightly lowering your willpower.</span>";
                }

                attackerMessage = "You headbutt " + victim.FirstName + " " + victim.LastName + ", lowering " + victimPronoun + " willpower slightly.";

                locationMessage = animalPlayer.GetFullName() + ", a " + attackerPlus.Form.FriendlyName + ", headbutted " + victim.FirstName + " " + victim.LastName + ".";

            }

            // Message stuff
            if (!message.IsNullOrEmpty() && !BlacklistProcedures.PlayersHaveBlacklistedEachOther(animalPlayer, victim, "any"))
            {
                if (message.Length <= 150)
                {
                    victimMessage = victimMessage + "<br><b>" + animalPlayer.GetFullName() + " " + message + "</b>";
                    attackerMessage = "You sent an emote to your owner: <b>" + animalPlayer.GetFullName() + " " + message + "</b><br>" + attackerMessage;
                }
            }

            // Notify a victim when a pet interacts with them.
            DomainRegistry.AttackNotificationBroker.Notify(victim.Id, victimMessage);
            DomainRegistry.AttackNotificationBroker.Notify(victim.Id, "A pet, <a href='/pvp/lookatplayer/" + animalPlayer.Id + "'>" + animalPlayer.GetFullName() + "</a>, just interacted with you!");

            PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);
            PlayerLogProcedures.AddPlayerLog(animalPlayer.Id, attackerMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationMessage);

            animalPlayer.TimesAttackingThisUpdate++;
            playerRepo.SavePlayer(victim);
            playerRepo.SavePlayer(animalPlayer);

            return attackerMessage;

        }



    }
}
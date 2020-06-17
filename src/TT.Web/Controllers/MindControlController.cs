using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class MindControlController : Controller
    {
        public virtual ActionResult MindControlList()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            MindControlProcedures.ClearPlayerMindControlFlagIfOn(me);

            ViewBag.MyId = me.Id;

            var output = MindControlProcedures.GetAllMindControlVMsWithPlayer(me);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.MindControl.Views.MindControlList, output);
        }

        public virtual ActionResult MoveVictim(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            var errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__MovementFormSourceId);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction(MVC.MindControl.MindControlList());
            }

            ViewBag.Victim = victim;

            if (victim.IsInDungeon())
            {
                var output = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region == "dungeon");
                return View(MVC.MindControl.Views.MoveVictim, output);
            }
            else
            {
                var output = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region != "dungeon");
                return View(MVC.MindControl.Views.MoveVictim, output);
            }
        }

        public virtual ActionResult StripVictim(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            var errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__StripFormSourceId);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction(MVC.MindControl.MindControlList());
            }

            var victimItems = ItemProcedures.GetAllPlayerItems(victim.Id).ToList();

            if (victimItems.Any())
            {
                double max = victimItems.Count();
                var rand = new Random();
                var num = rand.NextDouble();

                var index = Convert.ToInt32(Math.Floor(num * max));
                var itemToDrop = victimItems.ElementAt(index);

                MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__StripFormSourceId);

                var attackerMessage = "";

                if (itemToDrop.Item.ItemType != PvPStatics.ItemType_Pet)
                {
                    attackerMessage = "You commanded " + victim.GetFullName() + " to drop something.  They let go of a " + itemToDrop.Item.FriendlyName + " that they were carrying.";
                }
                else
                {
                    attackerMessage = "You commanded " + victim.GetFullName() + " to drop something.  They released their pet " + itemToDrop.Item.FriendlyName + " that they had tamed.";
                }

                PlayerLogProcedures.AddPlayerLog(me.Id, attackerMessage, false);
                TempData["Result"] = attackerMessage;

                var victimMessage = "";

                if (itemToDrop.Item.ItemType != PvPStatics.ItemType_Pet)
                {
                    victimMessage = me.GetFullName() + " commanded you to to drop something. You had no choice but to go of a " + itemToDrop.Item.FriendlyName + " that you were carrying.";
                }
                else
                {
                    victimMessage = me.GetFullName() + " commanded you to drop something. You had no choice but to release your pet " + itemToDrop.Item.FriendlyName + " that you had tamed.";
                }

                PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);

                ItemProcedures.DropItem(itemToDrop.dbItem.Id);

                var locationLogMessage = victim.GetFullName() + " was forced to drop their <b>" + itemToDrop.Item.FriendlyName + "</b> by someone mind controlling them.";
                LocationLogProcedures.AddLocationLog(victim.dbLocationName, locationLogMessage);

                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__MindControlCommandsIssued, 1);
            }
            else
            {
                TempData["Error"] = "It seems " + victim.GetFullName() + " was not carrying or wearing anything to drop!";
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult DeMeditateVictim(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            var errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__MeditateFormSourceId);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction(MVC.MindControl.MindControlList());
            }

            var buffs = ItemProcedures.GetPlayerBuffs(victim);
            var result = PlayerProcedures.DeMeditate(victim, me, buffs);

            MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__MeditateFormSourceId);


            TempData["Result"] = "You force " + victim.GetFullName() + " to meditate while filling their mind with nonsense instead of relaxation, lowering their mana instead of increasing it!";
            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__MindControlCommandsIssued, 1);

            return RedirectToAction(MVC.PvP.Play());
        }
    }
}
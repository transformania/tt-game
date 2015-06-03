using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Controllers
{
     [InitializeSimpleMembership]
    public class ItemController : Controller
    {
        //
        // GET: /Item/
         [Authorize]
        public ActionResult SelfCast()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();

            // assert player owns at least one of the type of item needed
            ItemViewModel itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.dbName == "item_consumable_selfcaster");
            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction("Play", "PvP");
            }

            IEnumerable<SkillViewModel2> output = SkillProcedures.GetSkillViewModelsOwnedByPlayer(me.Id).Where(m => m.MobilityType == "full");
            return View(output);
        }

         [Authorize]
        public ActionResult SelfCastSend(string spell)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            
            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player owns at least one of the type of item needed
            ItemViewModel itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.dbName == "item_consumable_selfcaster");
            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player does own this skill
            SkillViewModel2 skill = SkillProcedures.GetSkillViewModel(spell, me.Id);
            if (skill == null)
            {
                TempData["Error"] = "You do not own this spell.";
                return RedirectToAction("Play", "PvP");
            }

            // assert desired form is animate
            if (skill.MobilityType != "full") {
                TempData["Error"] = "The target form must be an animate form.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not already in the form of the spell
            if (me.Form == skill.Skill.FormdbName)
            {
                TempData["Error"] = "You are already in the target form of that spell, so doing this would do you no good.";
                return RedirectToAction("Play", "PvP");
            }

            PlayerProcedures.InstantChangeToForm(me, skill.Skill.FormdbName);
            ItemProcedures.DeleteItemOfName(me, itemToUse.dbItem.dbName);

            DbStaticForm form = FormStatics.GetForm(skill.Skill.FormdbName);
            TempData["Result"] = "You use a " + itemToUse.Item.FriendlyName + ", your spell bouncing through the device for a second before getting flung back at you and hitting you square in the chest, instantly transforming you into a " + form.FriendlyName + "!";

            return RedirectToAction("Play", "PvP");
        }

         [Authorize]
        public ActionResult RemoveCurse()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player owns at least one of the type of item needed
            ItemViewModel itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.dbName == "item_consumable_curselifter" || i.dbItem.dbName == "item_Butt_Plug_Hanna");
            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction("Play", "PvP");
            }

            IEnumerable<EffectViewModel2> output = EffectProcedures.GetPlayerEffects2(me.Id).Where(e => e.Effect.IsRemovable == true && e.dbEffect.Duration > 0).ToList();
            ViewBag.itemToUseId = itemToUse.dbItem.Id;

            return View(output);
        }

         [Authorize]
        public ActionResult RemoveCurseSend(string curse, int id)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();
            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player owns this item
            ItemViewModel itemToUse = ItemProcedures.GetItemViewModel(id);
            if (itemToUse == null || itemToUse.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the item can remove curses and is not any old item
            if (itemToUse.dbItem.dbName != "item_consumable_curselifter" && itemToUse.dbItem.dbName != "item_Butt_Plug_Hanna")
            {
                TempData["Error"] = "This item cannot remove curses.";
                return RedirectToAction("Play", "PvP");
            }

            DbStaticEffect curseToRemove = EffectStatics.GetEffect(curse);

            // assert this curse is removable
            if (curseToRemove.IsRemovable == false)
            {
                TempData["Error"] = "This curse is too strong to be lifted.";
                return RedirectToAction("Play", "PvP");
            }

            // success; remove the curse
            EffectProcedures.SetPerkDurationToZero(curseToRemove.dbName, me);

            // if the item is a consumable type, delete it.  Otherwise reset its cooldown
            if (itemToUse.Item.ItemType == "consumable")
            {
                ItemProcedures.DeleteItem(itemToUse.dbItem.Id);
            }
           // else if (itemToUse.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable)
            else
            {
                ItemProcedures.ResetUseCooldown(itemToUse);
            }
            

            TempData["Result"] = "You have successfully removed the curse <b>" + curseToRemove.FriendlyName + "</b> from your body!";
            return RedirectToAction("Play","PvP");
        }

         //[Authorize]
         public ActionResult ReadSkillBook(int id)
         {
             Player me = PlayerProcedures.GetPlayerFromMembership();

             // assert player is animate
             if (me.Mobility != "full")
             {
                 TempData["Error"] = "This curse is too strong to be lifted.";
                 return RedirectToAction("Play", "PvP");
             }

             ItemViewModel book = ItemProcedures.GetItemViewModel(id);

             // assert player owns this book
             if (book.dbItem.OwnerId != me.Id)
             {
                 TempData["Error"] = "You do not own this book.";
                 return RedirectToAction("Play", "PvP");
             }

             // assert player hasn't already read this book
             if (ItemProcedures.PlayerHasReadBook(me, book.dbItem.dbName) == true)
             {
                 TempData["Error"] = "You have already absorbed the knowledge from this book and can learn nothing more from it.";
                 TempData["SubError"] = "Perhaps a friend could use this tome more than you right now.";
                 return RedirectToAction("Play", "PvP");
             }

             ItemProcedures.DeleteItem(book.dbItem.Id);
             ItemProcedures.AddBookReading(me, book.dbItem.dbName);
             PlayerProcedures.GiveXP(me.Id, 35);

             new Thread(() =>
                  StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LoreBooksRead, 1)
              ).Start();

             TempData["Result"] = "You read your copy of " + book.Item.FriendlyName + ", absorbing its knowledge for 35 XP.  The tome slips into thin air so it can provide its knowledge to another mage in a different time and place.";
             return RedirectToAction("Play", "PvP");

         }

         public ActionResult ShowItemDetails(int id)
         {
             ItemViewModel output = ItemProcedures.GetItemViewModel(id);
             return PartialView("partial/ItemDetails", output);
         }

         public ActionResult ShowStatsTable()
         {
             return View();
         }


	}
}
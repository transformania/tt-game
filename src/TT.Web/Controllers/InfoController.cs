﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{

    public class InfoController : Controller
    {
         public ActionResult FAQ()
         {
             return View();
         }

         public ActionResult HowToPlay()
         {
             return View();
         }

         public ActionResult Rules()
         {
             return View();
         }

         public ActionResult GameNews_Archive()
         {
             return View();
         }

         public ActionResult GameNews_PlannedFeatures()
         {
             return View();
         }

         public ActionResult GameNews()
         {
             return View();
         }

         public ActionResult RecentRPClassifieds()
         {
            IEnumerable<RPClassifiedAd> output =  RPClassifiedAdsProcedures.GetClassifiedAds();
            return View(output);
         }

         public ActionResult AllLocations()
         {
             List<Location> output = LocationsStatics.LocationList.GetLocation.Where(s => s.dbName.Contains("_dungeon") == false).ToList();
             return PartialView(output);
         }

         public ActionResult AllForms()
         {
             List<DbStaticForm> output = PlayerProcedures.GetAllDbStaticForms().ToList();
             return PartialView(output);
         }

        public ActionResult ServerPopulation()
        {
            IServerLogRepository repo = new EFServerLogRepository();

            //  IEnumerable<PopulationTurnTuple> output = repo.ServerLogs.Select(


            IEnumerable<PopulationTurnTuple> output = from q in repo.ServerLogs  select new PopulationTurnTuple { Turn = q.TurnNumber, Population = q.Population };

            return View(output);
        }

        public JsonResult ServerPopulationJson()
        {
            IServerLogRepository repo = new EFServerLogRepository();
            IEnumerable<PopulationTurnTuple> output = from q in repo.ServerLogs select new PopulationTurnTuple { Turn = q.TurnNumber, Population = q.Population };
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GearTool()
        {
            return View();
        }

        /// <summary>
        /// Return basic information about all items and pet item type, name, graphic, and buffs in JSON format
        /// </summary>
        /// <returns></returns>
        public ActionResult Gear()
        {
            IDbStaticItemRepository repo = new EFDbStaticItemRepository();

            var output = from c in repo.DbStaticItems
                         select new
                         {
                             FriendlyName = c.FriendlyName,
                             ItemType = c.ItemType,
                             PortraitUrl = c.PortraitUrl,
                             Discipline = c.Discipline,
                             Perception = c.Perception,
                             Charisma = c.Charisma,
                             Fortitude = c.Fortitude,
                             Agility = c.Agility,
                             Allure = c.Allure,
                             Magicka = c.Magicka,
                             Succour = c.Succour,
                             Luck = c.Luck
                         };

            return Json(output, JsonRequestBehavior.AllowGet);
        }

    }
}
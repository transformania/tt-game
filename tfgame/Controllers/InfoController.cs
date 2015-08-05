using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Controllers
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;

namespace tfgame.Controllers
{

     [InitializeSimpleMembership]
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
    }
}
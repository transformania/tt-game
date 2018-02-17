using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain;
using System;
using TT.Domain.ClassifiedAds.Queries;
using TT.Domain.Identity.Queries;
using TT.Domain.Items.Queries;
using TT.Domain.Players.Queries;

namespace TT.Web.Controllers
{

    public partial class InfoController : Controller
    {
        public virtual ActionResult FAQ()
        {
            return View(MVC.Info.Views.FAQ);
        }

        public virtual ActionResult HowToPlay()
        {
            return View(MVC.Info.Views.HowToPlay);
        }

        public virtual ActionResult Rules()
        {
            return View(MVC.Info.Views.Rules);
        }


        public virtual ActionResult GameNews_PlannedFeatures()
        {
            return View(MVC.Info.Views.GameNews_PlannedFeatures);
        }

        public virtual ActionResult GameNews()
        {
            INewsPostRepository repo = new EFNewsPostRepository();

            var output = new NewsPageViewModel
            {
                ArtistBios = DomainRegistry.Repository.Find(new GetArtistBios()),
                NewsPosts = repo.NewsPosts.Where(n => n.ViewState == 1).OrderByDescending(n => n.Timestamp) // 1 == Live
            };

            return View(MVC.Info.Views.GameNews, output);
        }

        public virtual ActionResult GameNews_Archive()
        {
            INewsPostRepository repo = new EFNewsPostRepository();
            var output = repo.NewsPosts.Where(n => n.ViewState == 2).OrderByDescending(n => n.Timestamp); ; // 2 == Archived
            return View(MVC.Info.Views.GameNews_Archive, output);
        }

        public virtual ActionResult RecentRPClassifieds()
        {
            var output = DomainRegistry.Repository.Find(new GetRPClassifiedAds() { CutOff = TimeSpan.FromDays(3) });
            return View(MVC.Info.Views.RecentRPClassifieds, output);
        }

        public virtual ActionResult AllLocations()
        {
            var output = LocationsStatics.LocationList.GetLocation.Where(s => s.Region!="dungeon").ToList();
            return PartialView(MVC.Info.Views.AllLocations, output);
        }

        public virtual ActionResult AllForms()
        {
            var output = PlayerProcedures.GetAllDbStaticForms().ToList();
            return PartialView(MVC.Info.Views.AllForms, output);
        }

        public virtual ActionResult AllItems()
        {
            var cmd = new GetItemSources();
            var output = DomainRegistry.Repository.Find(cmd);
            return PartialView(MVC.Info.Views.AllItems, output);
        }

        public virtual ActionResult ServerPopulation()
        {
            IServerLogRepository repo = new EFServerLogRepository();

            //  IEnumerable<PopulationTurnTuple> output = repo.ServerLogs.Select(


            IEnumerable<PopulationTurnTuple> output = from q in repo.ServerLogs select new PopulationTurnTuple { Turn = q.TurnNumber, Population = q.Population };

            return View(MVC.Info.Views.ServerPopulation, output);
        }

        public virtual JsonResult ServerPopulationJson()
        {
            IServerLogRepository repo = new EFServerLogRepository();
            IEnumerable<PopulationTurnTuple> output = from q in repo.ServerLogs select new PopulationTurnTuple { Turn = q.TurnNumber, Population = q.Population };
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GearTool()
        {
            return View(MVC.Info.Views.GearTool);
        }

        /// <summary>
        /// Return basic information about all items and pet item type, name, graphic, and buffs in JSON format
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Gear()
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

        /// <summary>
        /// Requests data on all the starting forms required for a new player to make a character selection
        /// </summary>
        /// <returns>data on all the starting forms required for a new player to make a character selection</returns>
        public virtual ActionResult StartingForms()
        {
            var output = DomainRegistry.Repository.Find(new GetBaseForms());
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult Donate()
        {
            return View(MVC.Info.Views.Donate);
        }

    }
}
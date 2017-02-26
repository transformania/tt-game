using System;
using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Assets.Commands;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Queries;
using TT.Domain.Statics;
using TT.Domain.ViewModels.Assets;

namespace TT.Web.Controllers.Admin
{
    [Authorize(Roles = PvPStatics.Permissions_Admin)]
    public partial class TomeController : Controller
    {
        // GET: Lore
        public virtual ActionResult List()
        {

            var cmd = new GetTomes();
            var tomes = DomainRegistry.Repository.Find(cmd);

            SetMessages();
            return View(MVC.Admin.Views.Tomes.List, tomes);
        }

        public virtual ActionResult Edit(int Id)
        {
            var cmd = new GetTome { TomeId = Id };
            TomeDetail detail = DomainRegistry.Repository.FindSingle(cmd);

            var output = new UpdateTomeViewModel(detail);

            SetMessages();
            return View(MVC.Admin.Views.Tomes.Edit, output);
        }

        [ValidateInput(false)]
        public virtual ActionResult EditSend(UpdateTome cmd)
        {
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "Tome Id " + cmd.TomeId + " saved successfully.";
            return RedirectToAction(MVC.Tome.List());
        }

        public virtual ActionResult Create()
        {
            var output = new CreateTomeViewModel();
            return View(MVC.Admin.Views.Tomes.Create, output);
        }

        [ValidateInput(false)]
        public virtual ActionResult CreateSend(CreateTome cmd)
        {
            var tomeId = DomainRegistry.Repository.Execute(cmd);
            var tome = DomainRegistry.Repository.FindSingle(new GetTome { TomeId = tomeId });

            TempData["Result"] = "Tome for " + tome.BaseItem.FriendlyName + " created successfully.";
            return RedirectToAction(MVC.Tome.List());
        }

        public virtual ActionResult Delete(int id)
        {
            var cmd = new DeleteTome { TomeId = id };
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "Tome Id " + id + " deleted successfully.";
            return RedirectToAction(MVC.Tome.List());
        }

        private void SetMessages()
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
        }
    }
}
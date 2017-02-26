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
    public partial class RestockItemController : Controller
    {
        // GET: Lore
        public virtual ActionResult List()
        {

            var cmd = new GetRestockItems();
            var restockItems = DomainRegistry.Repository.Find(cmd);

            SetMessages();
            return View(MVC.Admin.Views.RestockItems.List, restockItems);
        }

        public virtual ActionResult Edit(int Id)
        {
            var cmd = new GetRestockItem { RestockItemId = Id };
            RestockItemDetail restockItemDetail = DomainRegistry.Repository.FindSingle(cmd);

            var output = new UpdateRestockItemViewModel(restockItemDetail);

            SetMessages();
            return View(MVC.Admin.Views.RestockItems.Edit, output);
        }

        [ValidateInput(false)]
        public virtual ActionResult EditSend(UpdateRestockItem cmd)
        {
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "RestockItem Id " + cmd.RestockItemId + " saved successfully.";
            return RedirectToAction(MVC.RestockItem.List());
        }

        public virtual ActionResult Create()
        {
            var output = new CreateRestockItemViewModel();
            return View(MVC.Admin.Views.RestockItems.Create, output);
        }

        [ValidateInput(false)]
        public virtual ActionResult CreateSend(CreateRestockItem cmd)
        {
            var restockItemId = DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "RestockItem created successfully.";
            return RedirectToAction(MVC.RestockItem.List());
        }

        public virtual ActionResult Delete(int id)
        {
            var cmd = new DeleteRestockItem { RestockItemId = id };
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "RestockItem Id " + id + " deleted successfully.";
            return RedirectToAction(MVC.RestockItem.List());
        }

        private void SetMessages()
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
        }
    }
}
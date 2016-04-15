using System;
using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Commands.Assets;
using TT.Domain.Statics;
using TT.Domain.Queries.Assets;
using TT.Domain.DTOs.Assets;
using TT.Domain.ViewModels.Assets;

namespace TT.Web.Controllers.Admin
{
    [Authorize(Roles = PvPStatics.Permissions_Admin)]
    public class RestockItemController : Controller
    {
        // GET: Lore
        public ActionResult List()
        {

            var cmd = new GetRestockItems();
            var restockItems = DomainRegistry.Repository.Find(cmd);

            SetMessages();
            return View("~/Views/Admin/RestockItems/List.cshtml", restockItems);
        }

        public ActionResult Edit(int Id)
        {
            var cmd = new GetRestockItem{ RestockItemId = Id };
            RestockItemDetail restockItemDetail = DomainRegistry.Repository.FindSingle(cmd);

            var output = new UpdateRestockItemViewModel(restockItemDetail);
           
            SetMessages();
            return View("~/Views/Admin/RestockItems/Edit.cshtml", output);
        }

        [ValidateInput(false)]
        public ActionResult EditSend(UpdateRestockItem cmd)
        {
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "RestockItem Id " + cmd.RestockItemId + " saved successfully.";
            return RedirectToAction("List");
        }

        public ActionResult Create()
        {
            var output = new CreateRestockItemViewModel();
            return View("~/Views/Admin/RestockItems/Create.cshtml", output);
        }

        [ValidateInput(false)]
        public ActionResult CreateSend(CreateRestockItem cmd)
        {
            var restockItemId = DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "RestockItem created successfully.";
            return RedirectToAction("List");
        }

        public ActionResult Delete(int id)
        {
            var cmd = new DeleteRestockItem{ RestockItemId = id };
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "RestockItem Id " + id + " deleted successfully.";
            return RedirectToAction("List");
        }

        private void SetMessages()
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
        }
    }
}
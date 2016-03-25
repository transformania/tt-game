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
    public class TomeController : Controller
    {
        // GET: Lore
        public ActionResult List()
        {

            var cmd = new GetTomes();
            var tomes = DomainRegistry.Repository.Find(cmd);

            SetMessages();
            return View("~/Views/Admin/Tomes/List.cshtml", tomes);
        }

        public ActionResult Edit(int Id)
        {
            var cmd = new GetTome(Id);
            TomeDetail detail = DomainRegistry.Repository.Find(cmd);
            var output = new UpdateTome
            {
                Id = detail.Id,
                BaseItemId = detail.BaseItem.Id,
                Text = detail.Text
            };

            SetMessages();
            return View("~/Views/Admin/Tomes/Edit.cshtml", output);
        }

        [ValidateInput(false)]
        public ActionResult EditSend(UpdateTome cmd)
        {

            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "Tome Id " + cmd.Id + " saved successfully.";
            return RedirectToAction("List");
        }

        public ActionResult Create()
        {
            var output = new CreateTomeViewModel();
            return View("~/Views/Admin/Tomes/Create.cshtml", output);
        }

        [ValidateInput(false)]
        public ActionResult CreateSend(CreateTomeViewModel input)
        {
            //var cmd = new CreateTome { Text = input.TomeDetail.Text, BaseItemId = input.TomeDetail.BaseItem.Id };
            var cmd = new CreateTome();
            cmd.Text = input.TomeDetail.Text;
            cmd.BaseItemId = input.TomeDetail.BaseItem.Id;

            var tome = DomainRegistry.Repository.Execute(cmd);
            TempData["Result"] = "Tome for " + tome.BaseItem.FriendlyName + " created successfully.";
            return RedirectToAction("List");
        }

        [ValidateInput(false)]
        public ActionResult Delete(int Id)
        {
            var cmd = new DeleteTome(Id);
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "Tome Id " + Id + " deleted successfully.";
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
using System;
using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Commands.Assets;
using TT.Domain.Statics;
using TT.Domain.Queries.Assets;
using TT.Domain.DTOs.Assets;

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

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

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

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

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
            var output = new CreateTome();
            output.Text = "<span class='booktitle'>TITLE</span><br><br>" + Environment.NewLine + Environment.NewLine + "<span class='bookauthor'>By AUTHOR</span><br><br>";

            return View("~/Views/Admin/Tomes/Create.cshtml", output);
        }

        [ValidateInput(false)]
        public ActionResult CreateSend(CreateTome cmd)
        {
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
    }
}
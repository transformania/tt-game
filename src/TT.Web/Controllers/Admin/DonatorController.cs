using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Commands.Identity;
using TT.Domain.DTOs.Identity;
using TT.Domain.Queries.Identity;
using TT.Domain.Statics;

namespace TT.Web.Controllers.Admin
{
    [Authorize(Roles = PvPStatics.Permissions_Admin)]
    public class DonatorController : Controller
    {

        public ActionResult List()
        {

            var output = DomainRegistry.Repository.Find(new GetUserDonators());
            SetMessages();

            return View("~/Views/Admin/Donators/List.cshtml", output);
        }

        public ActionResult Edit(int id)
        {

            var output = DomainRegistry.Repository.FindSingle(new GetUserDonator {Id = id});

            return View("~/Views/Admin/Donators/Edit.cshtml", output);
        }

        public ActionResult EditSend(UpdateDonator input)
        {

            try
            {
                DomainRegistry.Repository.Execute(input);
                TempData["Result"] = "Donator update successful.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.InnerException;
            }
            
            return RedirectToAction("List");
        }

        public ActionResult Delete()
        {

            var output = DomainRegistry.Repository.Find(new GetUserDonators());

            return View("~/Views/Admin/Donators/List.cshtml", output);
        }

        public ActionResult Create()
        {

            return View("~/Views/Admin/Donators/Create.cshtml");
        }

        public ActionResult CreateSend(CreateDonator input)
        {

            try
            {
                DomainRegistry.Repository.Execute(input);
                TempData["Result"] = "Donator created successful.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.InnerException;
            }

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
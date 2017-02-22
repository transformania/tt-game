using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Commands.Identity;
using TT.Domain.Commands.Players;
using TT.Domain.DTOs.Identity;
using TT.Domain.Queries.Identity;
using TT.Domain.Statics;

namespace TT.Web.Controllers.Admin
{
    [Authorize(Roles = PvPStatics.Permissions_Admin)]
    public partial class DonatorController : Controller
    {

        public virtual ActionResult List(int minimumTier = 1)
        {

            var output = DomainRegistry.Repository.Find(new GetUserDonators { MinimumTier = minimumTier });
            SetMessages();

            return View(MVC.Admin.Views.Donators.List, output);
        }

        public virtual ActionResult Edit(int id)
        {

            var output = DomainRegistry.Repository.FindSingle(new GetUserDonator { Id = id });

            return View(MVC.Admin.Views.Donators.Edit, output);
        }

        public virtual ActionResult EditSend(UpdateDonator input)
        {

            try
            {
                DomainRegistry.Repository.Execute(input);
                DomainRegistry.Repository.Execute(new ChangeDonatorTier { UserId = input.UserId, Tier = input.Tier });
                TempData["Result"] = "Donator update successful.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
            }

            return RedirectToAction(MVC.Donator.List());
        }

        public virtual ActionResult Delete()
        {

            var output = DomainRegistry.Repository.Find(new GetUserDonators());

            return View(MVC.Admin.Views.Donators.List, output);
        }

        public virtual ActionResult Create()
        {

            return View(MVC.Admin.Views.Donators.Create);
        }

        public virtual ActionResult CreateSend(CreateDonator input)
        {

            try
            {
                DomainRegistry.Repository.Execute(input);
                DomainRegistry.Repository.Execute(new ChangeDonatorTier { UserId = input.UserId, Tier = input.Tier });
                TempData["Result"] = "Donator created successful.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
            }

            return RedirectToAction(MVC.Donator.List());
        }

        private void SetMessages()
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
        }


    }
}
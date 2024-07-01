using Microsoft.AspNetCore.Mvc;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Server.Features.Identity;
using TT.Server.Features.Shared;

namespace TT.Server.Features.Character;

[Route("/character")]
public class CharacterController(ApplicationUserManager userManager) : LoggedInController(userManager)
{
    [HttpGet("restart")]
    public IActionResult Restart()
    {
        var me = PlayerProcedures.GetPlayerFromMembership(UserId);

        if (me != null && me.Mobility == PvPStatics.MobilityFull)
        {
            TempData["Error"] = "You are still animate.";
            return RedirectToAction("Play", "Play");
        }

        ViewBag.IsRerolling = false;
        ViewBag.OldFirstName = "";
        ViewBag.OldLastName = "";
        ViewBag.OldFormSourceId = 2;

        if (me != null)
        {
            ViewBag.IsRerolling = true;
            ViewBag.OldFirstName = me.FirstName;
            ViewBag.OldLastName = me.LastName.Split(' ')[0];
            ViewBag.OldFormSourceId = me.OriginalFormSourceId;
        }

        // find the reserved name if there is one
        if (me == null)
        {

            var reservedName = PlayerProcedures.GetPlayerReservedName(UserId);
            if (reservedName != null)
            {
                ViewBag.OldFirstName = reservedName.FullName.Split(' ')[0];
                ViewBag.OldLastName = reservedName.FullName.Split(' ')[1];
            }
        }

        return View(nameof(NewCharacter));
    }

    [HttpPost("newcharacter")]
    public IActionResult NewCharacter(NewCharacterViewModel newCharacterViewModel)
    {
        ViewBag.IsRerolling = false;

        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Your character was not created.  You can only use letters and your first and last names must be between 2 and 30 letters long.";
            return View();
        }

        if (newCharacterViewModel.StartGameMode != 0 && newCharacterViewModel.StartGameMode != 1 && newCharacterViewModel.StartGameMode != 2)
        {
            ViewBag.ErrorMessage = "That is not a valid game mode.";
            return View();
        }

        var staticForm = FormStatics.GetForm(newCharacterViewModel.FormSourceId);

        switch (staticForm.FriendlyName)
        {
            case "Regular Girl":
                newCharacterViewModel.Gender = PvPStatics.GenderFemale;
                break;
            case "Regular Guy":
                newCharacterViewModel.Gender = PvPStatics.GenderMale;
                break;
            default:
                ViewBag.ErrorMessage = "That is not a valid starting form.";
                return View();
        }

        // assert that the first name is not reserved by the system
        var fnamecheck = TrustStatics.NameIsReserved(newCharacterViewModel.FirstName);
        if (!fnamecheck.IsNullOrEmpty())
        {
            ViewBag.ErrorMessage = "You can't use the first name '" + newCharacterViewModel.FirstName + "'.  It is reserved.";
            return View();
        }

        // assert that the last name is not reserved by the system
        var lnamecheck = TrustStatics.NameIsReserved(newCharacterViewModel.LastName);
        if (!lnamecheck.IsNullOrEmpty())
        {
            ViewBag.ErrorMessage = "You can't use the last name '" + newCharacterViewModel.LastName + "'.  It is reserved or else not allowed.";
            return View();
        }

        // assert player does not currently have an animate character
        var me = PlayerProcedures.GetPlayerFromMembership(UserId);
        if (me is { Mobility: PvPStatics.MobilityFull })
        {
            ViewBag.ErrorMessage = "You cannot create a new character right now.  You already have a fully animate character already, " + me.GetFullName() + ".";
            return View();
        }

        // assert player does not have more than 1 account already

        var iAmWhitelisted = User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist);

        if (!iAmWhitelisted && newCharacterViewModel.InanimateForm == null && PlayerProcedures.IsMyIPInUseAndAnimate(UserIp))
        {
            ViewBag.ErrorMessage = "Your character was not created.  It looks like your IP address, <b>" + UserIp +
                                   "</b> already has 1 animate character in this world, and the current limit is 1. ";
            return View();
        }

        if (me != null && me.Covenant > 0 && newCharacterViewModel.InanimateForm == null)
        {
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (CovenantProcedures.GetPlayerCountInCovenant(myCov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount)
            {
                TempData["Error"] = "The maximum number of animate players in your covenant has already been reached.";
                TempData["SubError"] = "You will not be able to reroll as an animate character until there is room in the covenant or you leave it.";
                return RedirectToAction("Play", "Play");
            }
        }

        var result = PlayerProcedures.SaveNewPlayer(newCharacterViewModel, UserId);

        if (result != "saved")
        {
            ViewBag.ErrorMessage = "Your character was not created.  Reason:  " + result;
            ViewBag.IsRerolling = true;
            return View();
        }

        PlayerProcedures.LogIP(UserIp, UserId);

        return RedirectToAction("Play", "Play");
    }

    [HttpGet("startingforms")]
    public IActionResult StartingForms()
    {
        var output = DomainRegistry.Repository.Find(new GetBaseForms());
        return Json(output);
    }
}
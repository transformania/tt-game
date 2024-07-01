using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TT.Domain;
using TT.Domain.Legacy.Procedures;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Server.Features.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TT.Server.Features.Account
{
    [Authorize]
    public class AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        : Controller
    {
        [HttpGet("")]
        [HttpGet("/account/login")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("/account/login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            var user = DomainRegistry.Repository.FindSingle(new GetUserFromUsername { Username = model.UserName });
            var signInUser = await signInManager.UserManager.FindByNameAsync(model.UserName);

            if (result == SignInResult.Success)
            {
                return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Play", "Play");
            }

            ModelState.AddModelError("",
                result == SignInResult.LockedOut
                    ? $"Your account has been disabled until the following date:<br>" +
                      $"{signInUser?.LockoutEnd} UTC.<br><br>" +
                      $"Reason: {user.AccountLockoutMessage}" + "<br><br>" +
                      $"Please contact the moderation team via Discord or email support@transformaniatime.com for assistance if you want to make an appeal."
                    : "Invalid username or password.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost("/account/logoff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Manage));

        }

        [HttpGet("/account/register")]
        [AllowAnonymous]
        public virtual IActionResult Register()
        {
            return View();
        }

        [HttpPost("/account/register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User { UserName = model.UserName?.Trim(), Email = model.Email?.Trim(), CreateDate = DateTime.Now };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Restart", "Character");
            }

            AddErrors(result);

            return View(model);
        }

        //
        // GET: /Account/Manage

        [HttpGet("/account/manage")]
        public async Task<IActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your e-mail address has been changed."
                : "";
            ViewBag.HasLocalPassword = await HasPassword();
            ViewBag.ReturnUrl = Url.Action(nameof(Manage)) ?? string.Empty;

            return View();
        }

        [HttpGet("/account/change-email")]
        public async Task<IActionResult> ChangeEmail()
        {
            var model = new LocalEmailModel
            {
                Email = (await GetUser()).Email
            };
            return View(model);
        }

        [HttpPost("/account/change-email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(LocalEmailModel model)
        {
            var user = await GetUser();
            if (!ModelState.IsValid) return View(model);

            if (await userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                var result = await userManager.SetEmailAsync(user, model.Email?.Trim());
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Manage), new { message = ManageMessageId.ChangeEmailSuccess });
                }
                else
                {
                    AddErrors(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Bad password");
            }

            return View(nameof(ChangeEmail), model);
        }

        [HttpGet("/account/delete-account")]
        public IActionResult DeleteAccount()
        {
            return View(nameof(DeleteAccount));
        }

        [HttpPost("/account/confirm-delete-account")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirm()
        {
            var user = await GetUser();
            var player = PlayerProcedures.GetPlayerFromMembership(user.Id);
            try
            {
                DeleteAccountProcedures.DeleteAccount(user.Id, player.Id);
                await signInManager.SignOutAsync();
            }
            catch (Exception e)
            {
                TempData["Error"] = $"FAILED to delete account.  Reason: {e}.";
                return RedirectToAction(nameof(Manage));
            }
            return RedirectToAction(nameof(Register));
        }

        [HttpGet("/terms-of-service")]
        [AllowAnonymous]
        public IActionResult TermsOfService()
        {
            return View();
        }

        [HttpGet("/privacy-policy")]
        [AllowAnonymous]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpPost("/account/manage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(LocalPasswordModel model)
        {
            var hasPassword = await HasPassword();
            var user = await GetUser();

            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action(nameof(Manage));

            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction(nameof(Manage), new { message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Manage), new { message = ManageMessageId.SetPasswordSuccess});
                    }

                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ChangeEmailSuccess,
            Error
        }

        #region Identity Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private async Task<bool> HasPassword()
        {
            var user = await GetUser();
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private async Task<User> GetUser() => User?.Identity?.Name != null ? await userManager.FindByNameAsync(User.Identity.Name) : null;
        #endregion
    }
}

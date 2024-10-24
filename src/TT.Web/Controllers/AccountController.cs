using System;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TT.Web.Models;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using TT.Domain;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Queries;
using TT.Domain.Legacy.Procedures;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Players.Queries;
using TT.Web.Services;
using TT.Domain.Statics;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        private ApplicationUserManager userManager;
        private ApplicationSignInManager signInManager;
        private readonly IAuthenticationManager authenticationManager;
        private readonly bool useCaptcha;

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IAuthenticationManager authenticationManager, IFeatureService featureService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.authenticationManager = authenticationManager;
            useCaptcha = featureService.IsFeatureEnabled(Features.UseCaptcha);
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Account.Views.Login, new LoginModel { UseCaptcha = useCaptcha });
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Login(LoginModel model, string returnUrl)
        {
            if (useCaptcha)
            {
                var recaptchaHelper = this.GetRecaptchaVerificationHelper();
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Captcha answer cannot be empty.");
                    return View(MVC.Account.Views.Login, model);
                }
                var recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha answer.");
                }
            }

            if (ModelState.IsValid)
            {
                var result = signInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                var user = DomainRegistry.Repository.FindSingle(new GetUserFromUsername { Username = model.UserName });

                if (result == SignInStatus.Success)
                {
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("",
                    result == SignInStatus.LockedOut
                        ? $"Your account has been disabled until the following date:<br>" +
                        $"{signInManager.UserManager.FindByName(model.UserName).LockoutEndDateUtc} UTC.<br><br>" +
                        $"Reason: {user.AccountLockoutMessage}" + "<br><br>" +
                        $"Please contact the moderation team via Discord or email support@transformaniatime.com for assistance if you want to make an appeal."
                        : "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(MVC.Account.Views.Login, model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            authenticationManager.SignOut();
            return RedirectToAction(MVC.PvP.Play());

        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            return View(MVC.Account.Views.Register, new RegisterModel { UseCaptcha = useCaptcha });
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Register(RegisterModel model)
        {

            if (useCaptcha)
            {
                var recaptchaHelper = this.GetRecaptchaVerificationHelper();
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Captcha answer cannot be empty.");
                    return View(MVC.Account.Views.Register, model);
                }
                var recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha answer.");
                }
            }


            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.UserName?.Trim(), Email = model.Email?.Trim(), CreateDate = DateTime.Now };
                var result = userManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    signInManager.SignIn(user, isPersistent: false, rememberBrowser:false);
                    return RedirectToAction(MVC.PvP.Play());
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(MVC.Account.Views.Register, model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Disassociate(string provider, string providerUserId)
        {
            ManageMessageId? message = null;
            var owner = userManager.Find(new UserLoginInfo(provider, providerUserId));
            if (owner != null && owner.UserName == User.Identity.Name)
            {
                var result = userManager.RemoveLogin(owner.Id, new UserLoginInfo(provider, providerUserId));
                if (result.Succeeded)
                {
                    message = ManageMessageId.RemoveLoginSuccess;
                }
                else
                {
                    message = ManageMessageId.Error;
                }
            }
            return RedirectToAction(MVC.Account.Manage(message));
        }

        //
        // GET: /Account/Manage

        public virtual ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your e-mail address has been changed."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action(MVC.Account.Manage());

            // Fucking viewbags.
            ViewBag.Email = GetUser().Email;
            ViewBag.Approved = GetUser().Approved;
            
            return View(MVC.Account.Views.Manage);
        }

        //
        // GET: /Account/ChangeEmail

        public virtual ActionResult ChangeEmail()
        {
            var Email = new LocalEmailModel
            {
                Email = GetUser().Email
            };
            return View(MVC.Account.Views.ChangeEmail, Email);
        }

        //
        // GET: /Account/ChangeEmail

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangeEmail(LocalEmailModel model)
        {
            var user = GetUser();
            if (ModelState.IsValid)
            {
                if (userManager.CheckPassword(user, model.OldPassword))
                {

                    if (!model.Verify.IsNullOrEmpty() && userManager.ConfirmEmail(user.Id, model.Verify).Succeeded)
                    {
                        DomainRegistry.Repository.Execute(new SetApproved
                        {
                            UserId = user.Id,
                            Approved = true
                        });

                        TempData["Result"] = $"You have successfully verified your account.";
                        TempData["SubError"] = $"You should now be able to create a character.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    var result = userManager.SetEmail(User.Identity.GetUserId(), model.Email?.Trim());
                    if (result.Succeeded)
                    {
                        user = userManager.FindById(User.Identity.GetUserId());
                        if (user != null)
                        {
                            signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                        }

                        return RedirectToAction(MVC.Account.Manage(ManageMessageId.ChangeEmailSuccess));
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
            }
            // If we got this far, something failed, redisplay form
            return View(MVC.Account.Views.ChangeEmail, model);
        }

        public virtual ActionResult DeleteAccount()
        {
            return View(MVC.Account.Views.DeleteAccount);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual ActionResult DeleteAccountConfirm()
        {
            var myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            try
            {
                DeleteAccountProcedures.DeleteAccount(myMembershipId, me.Id);
                authenticationManager.SignOut();
            }
            catch (Exception e)
            {
                TempData["Error"] = $"FAILED to delete account.  Reason: {e}.";
                return RedirectToAction(MVC.PvP.Play());
            }
            return RedirectToAction(MVC.Account.Register());
        }

        [AllowAnonymous]
        public virtual ActionResult TermsOfService()
        {
            return View(MVC.Account.Views.TermsOfService);
        }

        [AllowAnonymous]
        public virtual ActionResult PrivacyPolicy()
        {
            return View(MVC.Account.Views.PrivacyPolicy);
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Manage(LocalPasswordModel model)
        {
            var hasPassword = HasPassword();
            var user = GetUser();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action(MVC.Account.Manage());
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var result = userManager.ChangePassword(user.Id, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user = userManager.FindById(User.Identity.GetUserId());
                        if (user != null)
                        {
                            signInManager.SignIn(user, isPersistent: false,rememberBrowser:false);
                        }

                        return RedirectToAction(MVC.Account.Manage(ManageMessageId.ChangePasswordSuccess));
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
                    var result = userManager.AddPassword(user.Id, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(MVC.Account.Manage(ManageMessageId.SetPasswordSuccess));
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(MVC.Account.Views.Manage, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SubmitCaptcha()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (useCaptcha && DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired { UserId = me.MembershipId }))
            {
                var recaptchaHelper = this.GetRecaptchaVerificationHelper();
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    TempData["Error"] = "You must correctly answer the captcha in order to do this.";
                    return RedirectToAction(MVC.PvP.Play());
                }
                var recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    DomainRegistry.Repository.Execute(new UpdateCaptchaEntry
                    {
                        UserId = me.MembershipId,
                        AddFailAttempt = true,
                        AddPassAttempt = false
                    });

                    TempData["Error"] = "Captcha incorrect.  Please try again.";
                    return RedirectToAction(MVC.PvP.Play());
                }
                else if (recaptchaResult == RecaptchaVerificationResult.Success)
                {
                    DomainRegistry.Repository.Execute(new UpdateCaptchaEntry
                    {
                        UserId = me.MembershipId,
                        AddFailAttempt = false,
                        AddPassAttempt = true
                    });
                }
            }
            TempData["Result"] = "Captcha successfully submitted!  You will not be prompted to do this again for a while.";
            return RedirectToAction(MVC.PvP.Play());
        }

        [Authorize]
        public virtual async Task<ActionResult> SendTestEmail()
        {
            var myMembershipId = User.Identity.GetUserId();

            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var recipient = GetUser().Email;

            if (recipient == null) 
            {
                TempData["Error"] = "It doesn't look like you have an email set up for your account.";
                TempData["SubError"] = "Please, for everyone's sanity, set that.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USWest1);

            var data = @"{""username"": ""Admin Debug""}";

            var sendRequest = new SendTemplatedEmailRequest
            {
                Source = "support@transformaniatime.com",
                Destination = new Destination { ToAddresses = new List<string> { recipient } },
                Template = "my-email-template",
                TemplateData = data,
            };

            await client.SendTemplatedEmailAsync(sendRequest);

            return RedirectToAction(MVC.PvP.Play());
        }

        [Authorize]
        public virtual async Task<ActionResult> SendVerificationEmail()
        {
            var recipient = GetUser()?.Email;

            if (recipient == null)
            {
                TempData["Error"] = "To verify your account, please set your email.";
                TempData["SubError"] = "Click on your account name in the top-right corner to access account-wide settings.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USWest1);

            var data = @"{
                        ""username"": """ + GetUser().UserName + @""",
                        ""verifycode"": """ + userManager.GenerateEmailConfirmationToken(GetUser().Id) + @"""
                        }";

            var sendRequest = new SendTemplatedEmailRequest
            {
                Source = "support@transformaniatime.com",
                Destination = new Destination { ToAddresses = new List<string> { recipient } },
                Template = "my-email-template",
                TemplateData = data,
            };

            await client.SendTemplatedEmailAsync(sendRequest);

            return RedirectToAction(MVC.PvP.Play());
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(MVC.PvP.Play());
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ChangeEmailSuccess,
            Error
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion


        #region Identity Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = GetUser();
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private User GetUser()
        {
            if (User != null && User.Identity != null)
            {
                return userManager.FindByName(User.Identity.Name);
            }

            return null;
        }
        #endregion
    }
}

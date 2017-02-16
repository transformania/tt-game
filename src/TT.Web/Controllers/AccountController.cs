using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FeatureSwitch;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TT.Web.Models;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using TT.Domain;
using TT.Domain.Commands.Identity;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Queries.Identity;

namespace TT.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        private Microsoft.Owin.Security.IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (FeatureContext.IsEnabled<UseCaptcha>())
            {
                RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Captcha answer cannot be empty.");
                    return View(model);
                }
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha answer.");
                }
            }

            if (ModelState.IsValid)
            {
                var result = SignInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

                if (result == SignInStatus.Success)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Play", "PvP");

        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {

            if (FeatureContext.IsEnabled<UseCaptcha>())
            {
                RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Captcha answer cannot be empty.");
                    return View(model);
                }
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha answer.");
                }
            }
            

            if (ModelState.IsValid)
            {
                var user = new User() { UserName = model.UserName, Email=model.Email, CreateDate=DateTime.Now };
                var result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    SignInManager.SignIn(user, isPersistent: false, rememberBrowser:false);
                    return RedirectToAction("Play", "PvP");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            ManageMessageId? message = null;
            var owner = UserManager.Find(new UserLoginInfo(provider, providerUserId));
            if (owner != null && owner.UserName == User.Identity.Name)
            {
                IdentityResult result = UserManager.RemoveLogin(owner.Id, new UserLoginInfo(provider, providerUserId));
                if (result.Succeeded)
                {
                    message = ManageMessageId.RemoveLoginSuccess;
                }
                else
                {
                    message = ManageMessageId.Error;
                }
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your e-mail address has been changed."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // GET: /Account/ChangeEmail

        public ActionResult ChangeEmail()
        {
            LocalEmailModel Email = new LocalEmailModel
            {
                Email = GetUser().Email
            };
            return View(Email);
        }

        //
        // GET: /Account/ChangeEmail

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(LocalEmailModel model)
        {
            var user = GetUser();
            if (ModelState.IsValid)
            {
                if (UserManager.CheckPassword(user, model.OldPassword))
                {
                    IdentityResult result = UserManager.SetEmail(User.Identity.GetUserId(), model.Email);
                    if (result.Succeeded)
                    {
                        user = UserManager.FindById(User.Identity.GetUserId());
                        if (user != null)
                        {
                            SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                        }

                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangeEmailSuccess });
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
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult TermsOfService()
        {
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasPassword = HasPassword();
            var user = GetUser();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = UserManager.ChangePassword(user.Id, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user = UserManager.FindById(User.Identity.GetUserId());
                        if (user != null)
                        {
                            SignInManager.SignIn(user, isPersistent: false,rememberBrowser:false);
                        }

                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
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
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = UserManager.AddPassword(user.Id, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCaptcha()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (FeatureContext.IsEnabled<UseCaptcha>() && DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired { UserId = me.MembershipId }))
            {
                RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    TempData["Error"] = "You must correctly answer the captcha in order to do this.";
                    return RedirectToAction("Play", "PvP");
                }
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    DomainRegistry.Repository.Execute(new UpdateCaptchaEntry
                    {
                        UserId = me.MembershipId,
                        AddFailAttempt = true,
                        AddPassAttempt = false
                    });

                    TempData["Error"] = "Captcha incorrect.  Please try again.";
                    return RedirectToAction("Play", "PvP");
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
            return RedirectToAction("Play", "PvP");
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
                return RedirectToAction("Play", "PvP");
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
                return UserManager.FindByName(User.Identity.Name);
            }

            return null;
        }
        #endregion
    }
}

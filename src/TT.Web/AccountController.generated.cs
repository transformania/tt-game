// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using TT.Web.Controllers.Generated;
namespace TT.Web.Controllers
{
    public partial class AccountController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected AccountController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Login()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Login);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Disassociate()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Disassociate);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Manage()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Manage);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public AccountController Actions { get { return MVC.Account; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "account";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "account";
        [GeneratedCode("T4MVC", "2.0")]
        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Login = ("Login").ToLowerInvariant();
            public readonly string LogOff = ("LogOff").ToLowerInvariant();
            public readonly string Register = ("Register").ToLowerInvariant();
            public readonly string Disassociate = ("Disassociate").ToLowerInvariant();
            public readonly string Manage = ("Manage").ToLowerInvariant();
            public readonly string ChangeEmail = ("ChangeEmail").ToLowerInvariant();
            public readonly string TermsOfService = ("TermsOfService").ToLowerInvariant();
            public readonly string SubmitCaptcha = ("SubmitCaptcha").ToLowerInvariant();
        }


        static readonly ActionParamsClass_Login s_params_Login = new ActionParamsClass_Login();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Login LoginParams { get { return s_params_Login; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Login
        {
            public readonly string returnUrl = ("returnUrl").ToLowerInvariant();
            public readonly string model = ("model").ToLowerInvariant();
        }
        static readonly ActionParamsClass_Register s_params_Register = new ActionParamsClass_Register();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Register RegisterParams { get { return s_params_Register; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Register
        {
            public readonly string model = ("model").ToLowerInvariant();
        }
        static readonly ActionParamsClass_Disassociate s_params_Disassociate = new ActionParamsClass_Disassociate();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Disassociate DisassociateParams { get { return s_params_Disassociate; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Disassociate
        {
            public readonly string provider = ("provider").ToLowerInvariant();
            public readonly string providerUserId = ("providerUserId").ToLowerInvariant();
        }
        static readonly ActionParamsClass_Manage s_params_Manage = new ActionParamsClass_Manage();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Manage ManageParams { get { return s_params_Manage; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Manage
        {
            public readonly string message = ("message").ToLowerInvariant();
            public readonly string model = ("model").ToLowerInvariant();
        }
        static readonly ActionParamsClass_ChangeEmail s_params_ChangeEmail = new ActionParamsClass_ChangeEmail();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ChangeEmail ChangeEmailParams { get { return s_params_ChangeEmail; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ChangeEmail
        {
            public readonly string model = ("model").ToLowerInvariant();
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _ChangePasswordPartial = "_ChangePasswordPartial";
                public readonly string _SetPasswordPartial = "_SetPasswordPartial";
                public readonly string ChangeEmail = "ChangeEmail";
                public readonly string Login = "Login";
                public readonly string Manage = "Manage";
                public readonly string Register = "Register";
                public readonly string TermsOfService = "TermsOfService";
            }
            public readonly string _ChangePasswordPartial = "~/Views/Account/_ChangePasswordPartial.cshtml";
            public readonly string _SetPasswordPartial = "~/Views/Account/_SetPasswordPartial.cshtml";
            public readonly string ChangeEmail = "~/Views/Account/ChangeEmail.cshtml";
            public readonly string Login = "~/Views/Account/Login.cshtml";
            public readonly string Manage = "~/Views/Account/Manage.cshtml";
            public readonly string Register = "~/Views/Account/Register.cshtml";
            public readonly string TermsOfService = "~/Views/Account/TermsOfService.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_AccountController : TT.Web.Controllers.AccountController
    {
        public T4MVC_AccountController() : base(Dummy.Instance) { }

        [NonAction]
        partial void LoginOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string returnUrl);

        [NonAction]
        public override System.Web.Mvc.ActionResult Login(string returnUrl)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Login);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "returnUrl", returnUrl);
            LoginOverride(callInfo, returnUrl);
            return callInfo;
        }

        [NonAction]
        partial void LoginOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, TT.Web.Models.LoginModel model, string returnUrl);

        [NonAction]
        public override System.Web.Mvc.ActionResult Login(TT.Web.Models.LoginModel model, string returnUrl)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Login);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "returnUrl", returnUrl);
            LoginOverride(callInfo, model, returnUrl);
            return callInfo;
        }

        [NonAction]
        partial void LogOffOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult LogOff()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LogOff);
            LogOffOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void RegisterOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Register()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Register);
            RegisterOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void RegisterOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, TT.Web.Models.RegisterModel model);

        [NonAction]
        public override System.Web.Mvc.ActionResult Register(TT.Web.Models.RegisterModel model)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Register);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            RegisterOverride(callInfo, model);
            return callInfo;
        }

        [NonAction]
        partial void DisassociateOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, string provider, string providerUserId);

        [NonAction]
        public override System.Web.Mvc.ActionResult Disassociate(string provider, string providerUserId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Disassociate);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "provider", provider);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "providerUserId", providerUserId);
            DisassociateOverride(callInfo, provider, providerUserId);
            return callInfo;
        }

        [NonAction]
        partial void ManageOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, TT.Web.Controllers.AccountController.ManageMessageId? message);

        [NonAction]
        public override System.Web.Mvc.ActionResult Manage(TT.Web.Controllers.AccountController.ManageMessageId? message)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Manage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "message", message);
            ManageOverride(callInfo, message);
            return callInfo;
        }

        [NonAction]
        partial void ChangeEmailOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult ChangeEmail()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ChangeEmail);
            ChangeEmailOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void ChangeEmailOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, TT.Web.Models.LocalEmailModel model);

        [NonAction]
        public override System.Web.Mvc.ActionResult ChangeEmail(TT.Web.Models.LocalEmailModel model)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.ChangeEmail);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            ChangeEmailOverride(callInfo, model);
            return callInfo;
        }

        [NonAction]
        partial void TermsOfServiceOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult TermsOfService()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.TermsOfService);
            TermsOfServiceOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void ManageOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, TT.Web.Models.LocalPasswordModel model);

        [NonAction]
        public override System.Web.Mvc.ActionResult Manage(TT.Web.Models.LocalPasswordModel model)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Manage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            ManageOverride(callInfo, model);
            return callInfo;
        }

        [NonAction]
        partial void SubmitCaptchaOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult SubmitCaptcha()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SubmitCaptcha);
            SubmitCaptchaOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
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
    public partial class MindControlController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public MindControlController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected MindControlController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult MoveVictim()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoveVictim);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult StripVictim()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.StripVictim);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult DeMeditateVictim()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.DeMeditateVictim);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public MindControlController Actions { get { return MVC.MindControl; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "mindcontrol";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "mindcontrol";
        [GeneratedCode("T4MVC", "2.0")]
        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string MindControlList = ("MindControlList").ToLowerInvariant();
            public readonly string MoveVictim = ("MoveVictim").ToLowerInvariant();
            public readonly string StripVictim = ("StripVictim").ToLowerInvariant();
            public readonly string DeMeditateVictim = ("DeMeditateVictim").ToLowerInvariant();
        }


        static readonly ActionParamsClass_MoveVictim s_params_MoveVictim = new ActionParamsClass_MoveVictim();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MoveVictim MoveVictimParams { get { return s_params_MoveVictim; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MoveVictim
        {
            public readonly string id = ("id").ToLowerInvariant();
        }
        static readonly ActionParamsClass_StripVictim s_params_StripVictim = new ActionParamsClass_StripVictim();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_StripVictim StripVictimParams { get { return s_params_StripVictim; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_StripVictim
        {
            public readonly string id = ("id").ToLowerInvariant();
        }
        static readonly ActionParamsClass_DeMeditateVictim s_params_DeMeditateVictim = new ActionParamsClass_DeMeditateVictim();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_DeMeditateVictim DeMeditateVictimParams { get { return s_params_DeMeditateVictim; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_DeMeditateVictim
        {
            public readonly string id = ("id").ToLowerInvariant();
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
                public readonly string MindControlList = "MindControlList";
                public readonly string MoveVictim = "MoveVictim";
            }
            public readonly string MindControlList = "~/Views/MindControl/MindControlList.cshtml";
            public readonly string MoveVictim = "~/Views/MindControl/MoveVictim.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_MindControlController : TT.Web.Controllers.MindControlController
    {
        public T4MVC_MindControlController() : base(Dummy.Instance) { }

        [NonAction]
        partial void MindControlListOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult MindControlList()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MindControlList);
            MindControlListOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void MoveVictimOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int id);

        [NonAction]
        public override System.Web.Mvc.ActionResult MoveVictim(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.MoveVictim);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            MoveVictimOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void StripVictimOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int id);

        [NonAction]
        public override System.Web.Mvc.ActionResult StripVictim(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.StripVictim);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            StripVictimOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void DeMeditateVictimOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int id);

        [NonAction]
        public override System.Web.Mvc.ActionResult DeMeditateVictim(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.DeMeditateVictim);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            DeMeditateVictimOverride(callInfo, id);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TT.Web.Attributes;

namespace TT.Web.Controllers
{
    [InheritedRoute("{controller}/{action=Index}")]
    public abstract class BaseController : Controller
    {
        // GET: {controller}
        public abstract ActionResult Index();
    }
}
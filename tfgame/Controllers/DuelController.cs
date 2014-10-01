using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.Filters;

namespace tfgame.Controllers
{
     [InitializeSimpleMembership]
    public class DuelController : Controller
    {
        
        [Authorize]
        public ActionResult Duel()
        {
            return View();
        }
	}
}
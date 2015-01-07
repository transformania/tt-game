using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.Filters;

namespace tfgame.Controllers
{
    public class NPCController : Controller
    {

        // This controller should handle all interactions with NPC characters, ie Lindella, Wuf, and Jewdewfae, and any others


        [InitializeSimpleMembership]
        public ActionResult Index()
        {
            return View();
        }
	}
}
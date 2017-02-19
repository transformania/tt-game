using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Web.Controllers.API
{
    public class WorldUpdateController : ApiControllerBase
    {
        [AllowAnonymous]
        public IHttpActionResult Post()
        {
            var allowedIps = new List<string> {"127.0.0.1", "::1"};
            var owinContext = Request.GetOwinContext();
            if (owinContext == null)
                return BadRequest();

            if (!allowedIps.Contains(owinContext.Request.RemoteIpAddress))
                return Unauthorized();

            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var world = worldStatRepo.PvPWorldStats.First();

            // Don't do a turn update if the round is over or we're still in an update
            if (world.TurnNumber >= PvPStatics.RoundDuration || world.WorldIsUpdating) return BadRequest();

            world.TurnNumber++;
            world.WorldIsUpdating = true;
            world.LastUpdateTimestamp = DateTime.UtcNow;

            // save changes to database
            worldStatRepo.SavePvPWorldStat(world);

            try
            {
                WorldUpdateProcedures.UpdateWorld();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Ok();
        }
    }
}
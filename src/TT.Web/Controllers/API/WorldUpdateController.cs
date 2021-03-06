﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Exceptions;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.World.Commands;

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

            if (world.TurnNumber == world.RoundDuration && !world.ChaosMode)
            {
                var round = Int32.Parse(PvPStatics.AlphaRound.Split(' ')[2]);
                var errorSavingLeaderboards = false;
                try
                {
                    DomainRegistry.Repository.Execute(new SavePvPLeaderboards { RoundNumber = round }); 
                }
                catch (DomainException)
                {
                    errorSavingLeaderboards = true;
                }

                try
                {
                    DomainRegistry.Repository.Execute(new SaveXpLeaderboards { RoundNumber = round });
                }
                catch (DomainException)
                {
                    errorSavingLeaderboards = true;
                }

                try
                {
                    DomainRegistry.Repository.Execute(new SaveItemLeaderboards { RoundNumber = round });
                }
                catch (DomainException)
                {
                    errorSavingLeaderboards = true;
                }

                if (errorSavingLeaderboards)
                {
                    return InternalServerError();
                }

                // TODO: Set the turn number to 0 and enable chaos mode once we are confident that leaderboards are saving properly, including achievements and badges
                return Ok();
            }

            // Don't do a turn update if the round is over or we're still in an update
            if (world.TurnNumber >= PvPStatics.RoundDuration || world.WorldIsUpdating) return BadRequest();

            // Don't do a turn update if it hasn't been long enough yet
            var gracePeriodSeconds = 10; // account for delays in fetching the URL
            var secondsElapsed = DateTime.UtcNow.Subtract(world.LastUpdateTimestamp).TotalSeconds + gracePeriodSeconds;
            if (secondsElapsed < TurnTimesStatics.GetTurnLengthInSeconds())
            {
                return BadRequest();
            }

            world.TurnNumber++;
            world.WorldIsUpdating = true;
            world.LastUpdateTimestamp = DateTime.UtcNow;

            // save changes to database
            worldStatRepo.SavePvPWorldStat(world);

            try
            {
                JokeShopProcedures.SetJokeShopActive(world.JokeShop);
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
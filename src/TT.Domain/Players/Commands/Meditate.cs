using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.Commands
{
    public class Meditate : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        public BuffBox Buffs { get; set; }
        public bool NoValidate { get; set; }

        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.PlayerLogs)
                    .Include(p => p.User)
                    .Include(p => p.User.Stats)
                    .Include(p => p.Effects)
                    .Include(p => p.VictimMindControls)
                    .SingleOrDefault(cr => cr.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID '{PlayerId}' could not be found");

                if (!NoValidate)
                {
                    if (player.ActionPoints < PvPStatics.MeditateCost)
                        throw new DomainException("You don't have enough action points to meditate!");

                    if (player.Mobility != PvPStatics.MobilityFull)
                        throw new DomainException("You must be animate in order to meditate!");

                    if (player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
                        throw new DomainException("You have cleansed and meditated the maximum number of times this update.");

                    var mcs = player.VictimMindControls.Select(m => m.FormSourceId);

                    if (mcs.Contains(MindControlStatics.MindControl__MeditateFormSourceId))
                        throw new DomainException("You try to meditate but find you cannot!  The moment you try and focus, your head swims with nonsensical thoughts implanted by someone partially mind controlling you!");

                }

                result = player.Meditate(Buffs);

                // log statistics only for human players
                if (player.BotId == AIStatics.ActivePlayerBotId)
                {
                    player.User.AddStat(StatsProcedures.Stat__TimesMeditated, 1);
                }

                ctx.Update(player);

                // TODO: Perform this as part of the Meditate method on the Player entity once known how
                var locationLog = LocationLog.Create(player.Location, $"<span class='playerMediatingNotification'>{player.GetFullName()} meditated here.</span>");
                ctx.Add(locationLog);

                ctx.Commit();
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("Player ID is required!");

            if (Buffs == null)
                throw new DomainException("Buffs are required!");
        }

    }
}


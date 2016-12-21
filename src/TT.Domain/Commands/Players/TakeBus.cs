using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Commands.Players
{
    public class TakeBus : DomainCommand<string>
    {
        public int playerId { get; set; }
        public string destination { get; set; }

        public override string Execute(IDataContext context)
        {
            var output = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>()
                    .Include(p => p.Effects)
                    .Include(p => p.User)
                    .Include(p => p.User.Stats)
                .SingleOrDefault(p => p.Id == playerId);

                if (player == null)
                    throw new DomainException($"Player with Id '{playerId}' could not be found");

                if (player.Mobility != PvPStatics.MobilityFull)
                    throw new DomainException("You must be animate in order to take the bus.");

                var originLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == player.Location);

                if (!LocationsStatics.BusStops.Contains(originLocation.dbName))
                    throw new DomainException("You aren't at a valid bus stop.");

                var destinationLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == destination);
                if (!LocationsStatics.BusStops.Contains(destinationLocation.dbName))
                    throw new DomainException("Your destination is not a valid bus stop.");

                if (originLocation.dbName == destinationLocation.dbName)
                    throw new DomainException("You can't take the bus to the location you're already at.");

                if (DateTime.UtcNow.Subtract(player.GetLastCombatTimestamp()).TotalMinutes<15)
                    throw new DomainException("You have been in combat too recently to take a bus.");

                if (player.InDuel > 0)
                    throw new DomainException("You cannot take the bus whilst in a duel.");

                if (player.InQuest > 0)
                    throw new DomainException("You cannot take the bus whilst in a quest.");

                var distance = LocationsStatics.GetDistanceBetweenLocations(player.Location, destination);
                var ticketPrice = LocationsStatics.GetTicketPriceBetweenLocations(player.Location, destination);

                if (player.ActionPoints < 3)
                    throw new DomainException("You don't have enough AP to take the bus.");

                if (player.Money < ticketPrice)
                    throw new DomainException("You can't afford this bus ticket!");

                if (player.Effects.FirstOrDefault(e => e.dbName == MindControlStatics.MindControl__Movement) != null)
                    throw new DomainException("You can't ride the bus while under the Forced March! mind control spell.");

                var buffs = ItemProcedures.GetPlayerBuffs(player.Id);

                if (buffs.MoveActionPointDiscount() < -120)
                    throw new DomainException("You can't ride the bus while immobilized.");

                output = $"You took the bus from <b>{originLocation.Name}</b> to <b>{destinationLocation.Name}</b> for <b>{ticketPrice}</b> Arpeyjis.";
                player.SetLocation(destination);
                player.AddLog(output, false);
                player.ChangeMoney(-ticketPrice);
                player.ChangeActionPoints(-3);
                player.SetOnlineActivityToNow();

                var originLocationLog = LocationLog.Create(originLocation.dbName, $"{player.GetFullName()} got on a bus headed toward {destinationLocation.Name}.", 0);
                var destinationLocationLog = LocationLog.Create(destinationLocation.dbName, $"{player.GetFullName()} arrived via bus from {originLocation.Name}.", 0);

                // log statistics only for human players
                if (player.BotId == AIStatics.ActivePlayerBotId)
                {
                    player.User.AddStat(StatsProcedures.Stat__BusRides, distance);
                }

                ctx.Add(originLocationLog);
                ctx.Add(destinationLocationLog);
                ctx.Commit();
            };

            ExecuteInternal(context);
            return output;
        }

        protected override void Validate()
        {
            if (destination == null)
                throw new DomainException("Destination is required.");
        }
    }
}

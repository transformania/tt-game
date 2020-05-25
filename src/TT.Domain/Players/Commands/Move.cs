using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.Commands
{
    public class Move : DomainCommand<string>
    {
        public int PlayerId { get; set; }
        public string destination { get; set; }

        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Item)
                    .Include(p => p.User)
                    .Include(p => p.User.Stats)
                    .Include(p => p.VictimMindControls)
                    .SingleOrDefault(cr => cr.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID '{PlayerId}' could not be found");

                if (String.IsNullOrWhiteSpace(destination))
                    throw new DomainException("Destination must be specified");

                if (LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == destination) == null)
                    throw new DomainException($"Location with dbName '{destination}' could not be found");

                if (player.MoveActionPointDiscount < -TurnTimesStatics.GetActionPointReserveLimit())
                    throw new DomainException("You can't move since you have been immobilized!");

                if (player.ActionPoints < PvPStatics.LocationMoveCost)
                    throw new DomainException("You don't have enough action points to move.");

                if (player.Mobility == PvPStatics.MobilityInanimate)
                    throw new DomainException("You can't move because you are currently inanimate!");

                var lastAttackTimeAgo = Math.Abs(Math.Floor(player.LastCombatTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));
                if (lastAttackTimeAgo < TurnTimesStatics.GetNoMovingAfterAttackSeconds())
                    throw new DomainException($"You are resting from a recent attack.  You must wait {Math.Ceiling(TurnTimesStatics.GetNoMovingAfterAttackSeconds() - lastAttackTimeAgo)} more seconds before moving.");

                if (player.Mobility == PvPStatics.MobilityPet && player.Item != null)
                {
                    if (player.Item.Owner != null)
                        throw new DomainException($"You can't move because you are a non-feral pet owned by {player.Item.Owner.GetFullName()}.");
                }

                if (player.IsCarryingTooMuchToMove())
                {
                    var excessItemCount = player.GetCarriedItemCount() - player.GetMaxInventorySize();
                    var pluralize = excessItemCount == 1 ? "item" : "items";
                    throw new DomainException($"You are carrying too much to move.  You need to drop at least {excessItemCount} {pluralize}.");
                }

                if (player.InDuel > 0)
                    throw new DomainException("You must finish your duel before you can move again.");

                if (player.InQuest > 0)
                    throw new DomainException("You must end your quest before you can move again.");

                // TODO:  This might not work yet because it's considering mind controls this player has on others and not themself
                //if (player.CantMoveBecauseOfForcedMarch())
                //        throw new DomainException("You try to move but discover you cannot!  Some other mage has partial control of your mind, disabling your ability to move on your own!");

                // assert this location does have a connection to the next one
                var currentLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == player.Location);
                var nextLocation = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == destination);
                if (currentLocation.Name_North != destination && currentLocation.Name_East != destination && currentLocation.Name_South != destination && currentLocation.Name_West != destination)
                {
                    throw new DomainException($"You cannot move directly from {currentLocation.Name} to {nextLocation.Name}.");
                }

                MoveLogBox logs;

                if (player.Mobility == PvPStatics.MobilityPet)
                {
                    logs = player.MoveToAsAnimal(destination);
                }
                else
                {
                    logs = player.MoveTo(destination);
                }


                var oldLocationLog = LocationLog.Create(currentLocation.dbName, logs.SourceLocationLog, logs.ConcealmentLevel);
                var newLocationLog = LocationLog.Create(nextLocation.dbName, logs.DestinationLocationLog, logs.ConcealmentLevel);
                result = logs.PlayerLog;
                ctx.Add(oldLocationLog);
                ctx.Add(newLocationLog);
                ctx.Update(player);

                ctx.Commit();
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("Player ID is required!");
        }
    }
}

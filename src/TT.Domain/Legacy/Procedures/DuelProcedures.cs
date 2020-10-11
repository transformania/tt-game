using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class DuelProcedures
    {

        public const string PENDING = "pending";
        public const string ACTIVE = "active";
        public const string FINISHED = "finished";
        public const string TIMEOUT = "timeout";
        public const string REJECTED = "rejected";

        public const int TimeoutCurseEffectSourceId = 156;

        public static void SendDuelChallenge(Player challenger, Player target)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IDuelRepository duelRepo = new EFDuelRepository();

            var dbChallenger = playerRepo.Players.FirstOrDefault(p => p.Id == challenger.Id);
            var dbTarget = playerRepo.Players.FirstOrDefault(p => p.Id == target.Id);


            var newDuel = new Duel
            {
                StartTurn = -1,
                ProposalTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                CompletionTurn = -1,
                Status = PENDING,
                LastResetTimestamp = DateTime.UtcNow,
                Combatants = new List<DuelCombatant> {
                    new DuelCombatant {
                        PlayerId = challenger.Id,
                        Team = 1,
                    }, new DuelCombatant {
                        PlayerId = target.Id,
                                            Team = 2,
                    }
                },
            };

            duelRepo.SaveDuel(newDuel);

            // TODO:  make it so target has to accept first
           // dbChallenger.InDuel = newDuel.Id;
           // dbTarget.InDuel = newDuel.Id;

            var messageToTarget = "You have been challenge to a duel by <b>" + challenger.GetFullName() + "</b>!  Will you accept the challenge or show your cowardice?  " +
                "<b><u><a href='/Duel/AcceptChallenge/" + newDuel.Id + "'>Click here to Accept</a></b></u>."
                ;
            PlayerLogProcedures.AddPlayerLog(dbTarget.Id, messageToTarget, true);

            playerRepo.SavePlayer(dbChallenger);
            playerRepo.SavePlayer(dbTarget);

        }

        public static Duel GetDuel(int duelId)
        {
            IDuelRepository duelRepo = new EFDuelRepository();
            return duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);
        }

        public static List<PlayerFormViewModel> GetPlayerViewModelsInDuel(int duelId)
        {
            IDuelRepository duelRepo = new EFDuelRepository();
            var duel = duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);

            var output = new List<PlayerFormViewModel>();

            foreach (var p in duel.Combatants)
            {
                var player = PlayerProcedures.GetPlayerFormViewModel(p.PlayerId);
                if (player != null)
                {
                    output.Add(player);
                }
            }

            return output;
        }

        public static void BeginDuel(int duelId)
        {
            IDuelRepository duelRepo = new EFDuelRepository();
            var duel = duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);
            duel.StartTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            duel.Status = ACTIVE;

            var members = GetPlayerViewModelsInDuel(duelId);
            var memberNames = "";

            foreach (var p in members)
            {
                memberNames += p.Player.GetFullName() + ", ";
            }
        
            foreach (var p in members)
            {
                duel.Combatants.FirstOrDefault(f => f.PlayerId == p.Player.Id).StartFormSourceId = p.Player.FormSourceId;
                PlayerProcedures.EnterDuel(p.Player.Id, duel.Id);
                var playerMessage = "<b>Your duel between " + memberNames + " has begun!</b>";
                PlayerLogProcedures.AddPlayerLog(p.Player.Id, playerMessage, true);
            }

            LocationLogProcedures.AddLocationLog(members.First().Player.dbLocationName, "<b class='playerAttackNotification'>A duel started here.</b>");

            duelRepo.SaveDuel(duel);

        }

        public static bool PlayerIsNotInDuel(Player attacker, Player target)
        {

            var duel = DuelProcedures.GetDuel(attacker.InDuel);

            // attacker is not in any duel, so obviously they are not in this duel
            if (duel == null)
            {
                return true;
            }

            // otherwise scan through participants and see if the target is amongst them
            var notInDuel = true;
            foreach (var d in duel.Combatants)
            {
                if (target.Id == d.PlayerId)
                {
                    notInDuel = false;
                    break;
                }
            }

            if (notInDuel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void EndDuel(int duelId, string endStatus)
        {
            IDuelRepository duelRepo = new EFDuelRepository();
            var duel = duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);
            duel.CompletionTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            duel.Status = endStatus;

            duelRepo.SaveDuel(duel);

            foreach (var d in duel.Combatants)
            {
                PlayerProcedures.EnterDuel(d.PlayerId, 0);

                var message = "";

                if (endStatus==TIMEOUT) {
                    message = "<b class='bad'>Your duel has timed out, ending in a disappointing draw.  You feel as if some frustrated spirits have left you weakened by a curse...</b>";
                    EffectProcedures.GivePerkToPlayer(TimeoutCurseEffectSourceId, d.PlayerId);
                } else {
                    message = "<b>Your duel has ended.</b>";
                }

                PlayerLogProcedures.AddPlayerLog(d.PlayerId, message, true);
            }

        }

        public static void SetLastDuelAttackTimestamp(int duelId)
        {
            IDuelRepository duelRepo = new EFDuelRepository();
            var duel = duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);
            duel.LastResetTimestamp = DateTime.UtcNow;
            duelRepo.SaveDuel(duel);
        }

    }
}
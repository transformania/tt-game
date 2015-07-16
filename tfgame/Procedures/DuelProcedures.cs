using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;

namespace tfgame.Procedures
{
    public static class DuelProcedures
    {

        public const string PENDING = "pending";
        public const string ACTIVE = "active";
        public const string FINISHED = "finished";
        public const string REJECTED = "rejected";

        public static void SendDuelChallenge(Player challenger, Player target)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IDuelRepository duelRepo = new EFDuelRepository();

            Player dbChallenger = playerRepo.Players.FirstOrDefault(p => p.Id == challenger.Id);
            Player dbTarget = playerRepo.Players.FirstOrDefault(p => p.Id == target.Id);


            Duel newDuel = new Duel
            {
                StartTurn = -1,
                ProposalTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                CompletionTurn = -1,
                Status = PENDING,
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

            string messageToTarget = "You have been challenge to a duel by <b>" + challenger.GetFullName() + "</b>!  Will you accept the challenge or show your cowardice?  " +
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
            Duel duel = duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);

            List<PlayerFormViewModel> output = new List<PlayerFormViewModel>();

            foreach (DuelCombatant p in duel.Combatants)
            {
                output.Add(PlayerProcedures.GetPlayerFormViewModel(p.PlayerId));
            }

            return output;
        }

        public static void BeginDuel(int duelId)
        {
            IDuelRepository duelRepo = new EFDuelRepository();
            Duel duel = duelRepo.Duels.FirstOrDefault(d => d.Id == duelId);
            duel.StartTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            duel.Status = ACTIVE;

            List<PlayerFormViewModel> members = GetPlayerViewModelsInDuel(duelId);
            string memberNames = "";

            foreach (PlayerFormViewModel p in members)
            {
                memberNames += p.Player.GetFullName() + ", ";
            }
        
            foreach (PlayerFormViewModel p in members)
            {
                duel.Combatants.FirstOrDefault(f => f.PlayerId == p.Player.Id).StartForm = p.Player.Form;
                PlayerProcedures.EnterDuel(p.Player.Id, duel.Id);
                string playerMessage = "<b>Your duel between " + memberNames + " has begun!</b>";
                PlayerLogProcedures.AddPlayerLog(p.Player.Id, playerMessage, true);
            }

            LocationLogProcedures.AddLocationLog(members.First().Player.dbLocationName, "<b class='playerAttackNotification'>A duel started here.<b>");

            duelRepo.SaveDuel(duel);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class DuelProcedures
    {
        public static void SendDuelChallenge(Player challenger, Player target)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IDuelRepository duelRepo = new EFDuelRepository();

            Player dbChallenger = playerRepo.Players.FirstOrDefault(p => p.Id == challenger.Id);
            Player dbTarget = playerRepo.Players.FirstOrDefault(p => p.Id == target.Id);

          
            Duel newDuel = new Duel();
            newDuel.StartTurn = -1;
            newDuel.ProposalTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            newDuel.CompletionTurn = -1;

            dbChallenger.InDuel = 1;
            dbTarget.InDuel = 1;

            playerRepo.SavePlayer(dbChallenger);
            playerRepo.SavePlayer(dbTarget);

        }
    }
}
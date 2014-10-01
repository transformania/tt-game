using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class PlayerExtraProcedures
    {
        public static PlayerExtra GetPlayerExtra(Player player)
        {
            IPlayerExtraRepository repo = new EFPlayerExtraRepository();
            PlayerExtra output = repo.PlayerExtras.FirstOrDefault(i => i.PlayerId == player.Id);

            if (output == null)
            {
                output = new PlayerExtra
                {
                    ProtectionToggleTurnsRemaining = 0,
                    PlayerId = player.Id,
                };
                repo.SavePlayerExtra(output);
            }
            return output;

        }

        public static void SetNextProtectionToggleTurn(Player player)
        {
            IPlayerExtraRepository repo = new EFPlayerExtraRepository();
            PlayerExtra saveMe = repo.PlayerExtras.FirstOrDefault(i => i.PlayerId == player.Id);
           
            saveMe.ProtectionToggleTurnsRemaining = 9 * player.Level; 
            repo.SavePlayerExtra(saveMe);

        }
    }
}
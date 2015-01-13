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
    public class MindControlProcedures
    {

        public static void AddMindControl(Player attacker, Player victim, string type)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbAttacker = playerRepo.Players.FirstOrDefault(p => p.Id == attacker.Id);
            Player dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

            MindControl mc = mcRepo.MindControls.FirstOrDefault(m => m.VictimId == victim.Id && m.MasterId == attacker.Id);

            if (mc == null)
            {
                mc = new MindControl
                {
                    TurnsRemaining = 6,
                    MasterId = attacker.Id,
                    VictimId = victim.Id,
                    Type = type,
                };
            }
            else
            {
                mc.TurnsRemaining = 6;
            }



            mcRepo.SaveMindControl(mc);

            dbVictim.MindControlIsActive = true;
            playerRepo.SavePlayer(dbVictim);


        }

        public static IEnumerable<MindControl> GetAllMindControlsWithPlayer(Player player)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            return mcRepo.MindControls.Where(m => m.MasterId == player.Id || m.VictimId == player.Id);
        }

        public static IEnumerable<MindControlViewModel> GetAllMindControlVMsWithPlayer(Player player)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            List<MindControl> mcs = mcRepo.MindControls.Where(m => m.MasterId == player.Id || m.VictimId == player.Id).ToList();

            List<MindControlViewModel> output = new List<MindControlViewModel>();

            PlayerFormViewModel basePlayer = PlayerProcedures.GetPlayerFormViewModel(player.Id);

            foreach (MindControl mc in mcs)
            {
                MindControlViewModel addme = new MindControlViewModel
                {
                    MindControl = mc,
                    Victim = PlayerProcedures.GetPlayerFormViewModel(mc.VictimId),
                    TypeFriendlyName = GetMCFriendlyName(mc.Type),
                };
                output.Add(addme);
            }
            return output;

        }

        public static string GetMCFriendlyName(string input)
        {
            if (input == "form_(MC-Movement)_Judoo") {
                return "Forced March";
            }
            else
            {
                return "ERROR:  UNKNOWN";
            }
        }

    }
}
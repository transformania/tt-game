using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
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

            MindControl mc = mcRepo.MindControls.FirstOrDefault(m => m.VictimId == victim.Id && m.MasterId == attacker.Id && m.Type == type);

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
                    Master = PlayerProcedures.GetPlayerFormViewModel(mc.MasterId),
                    TypeFriendlyName = GetMCFriendlyName(mc.Type),
                };
                output.Add(addme);
            }
            return output;

        }

        public static string GetMCFriendlyName(string input)
        {
            if (input == MindControlStatics.MindControl__Movement) {
                return "Forced March";
            }
            else if (input == MindControlStatics.MindControl__Strip)
            {
                return "Take a Load Off!";
            }
            else
            {
                return "ERROR:  UNKNOWN";
            }
        }

        public static IEnumerable<MindControl> GetMindControlsBetweenPlayers(Player master, Player victim)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            return mcRepo.MindControls.Where(m => m.MasterId == master.Id && m.VictimId == victim.Id);
        }

        public static ErrorBox AssertBasicMindControlConditions(Player master, Player victim, string mcType)
        {
            ErrorBox output = new ErrorBox();
            output.HasError = true;

            // assert that world update is not in progress
            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                output.Error = "Animate portion of world update is in progress.";
                output.SubError = "Try again in a few seconds.";
                return output;
            }

            // assert both commander and victim is animate
            if (master.Mobility != "full" || victim.Mobility != "full")
            {
                output.Error = "Both you and your victim must be animate in order to invoke any mind control commands.";
                return output;
            }

            // assert that the victim is not offline
            if (PlayerProcedures.PlayerIsOffline(victim) == true)
            {
                output.Error = "Your victim has gone offline.";
                output.SubError = "You can only issue commands to online players under your mind control.";
                return output;
            }

            // assert that there is indeed a mind control between these two players
            IEnumerable<MindControl> mcs = MindControlProcedures.GetMindControlsBetweenPlayers(master, victim);

            MindControl mc = mcs.FirstOrDefault(m => m.Type == mcType);

            if (mc == null)
            {
                output.Error = "You are not mind controlling this person with this type of mind control.";
                return output;
            }

            // assert that the player is the master of the mind control
            if (mc.MasterId != master.Id)
            {
                output.Error = "You are not mind controlling this person with this type of mind control.";
                return output;
            }

            // assert that the mind control is not expired, if for whatever reason it has not been deleted
            if (mc.TurnsRemaining <= 0)
            {
                output.Error = "This mind control has expired.";
                return output;
            }

            // assert that this mind control has not reached its limit
            if (mc.TimesUsedThisTurn >= GetCommandLimitOfType(mc.Type))
            {
                output.Error = "You've issued this command too many times this turn.";
                output.SubError = "Wait until next turn to issue your next command.";
                return output;
            }

            output.HasError = false;
            return output;

        }

        public static int GetCommandLimitOfType(string type)
        {
            if (type == MindControlStatics.MindControl__Movement)
            {
                return MindControlStatics.MindControl__Movement_Limit;
            }
            else if (type == MindControlStatics.MindControl__Strip)
            {
                return MindControlStatics.MindControl__Strip_Limit;
            }

            return 0;
        }

        public static decimal GetAPCostToMove(BuffBox buffs, string oldLocation, string newLocation)
        {
            Location oldLocationl = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == oldLocation);
            Location newLocationl = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == newLocation);
            decimal output = Math.Abs(oldLocationl.X - newLocationl.X) + Math.Abs(oldLocationl.Y - newLocationl.Y);
            output *= 1-buffs.MoveActionPointDiscount();

            return output;
        }

        public static bool ClearPlayerMindControlFlagIfOn(Player player)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            if (mcRepo.MindControls.Where(m => m.VictimId == player.Id).Count() == 0)
            {
                Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
                dbPlayer.MindControlIsActive = false;
                playerRepo.SavePlayer(dbPlayer);
                return true;
            }
            return false;
        }

        public static bool PlayerIsMindControlledWithType(Player player, string type)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            if (mcRepo.MindControls.Where(p => p.VictimId == player.Id && p.Type == type).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PlayerIsMindControlledWithType(Player player, IEnumerable<MindControl> controls, string type)
        {
            if (controls.Where(p => p.VictimId == player.Id && p.Type == type).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PlayerIsMindControlledWithSomeType(Player player, IEnumerable<MindControl> controls)
        {
            if (controls.Where(p => p.VictimId == player.Id).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void AddCommandUsedToMindControl(Player master, Player victim, string type)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            MindControl mc = mcRepo.MindControls.FirstOrDefault(m => m.MasterId == master.Id && m.VictimId == victim.Id && m.Type == type);
            mc.TimesUsedThisTurn++;
            mcRepo.SaveMindControl(mc);
        }

    }
}
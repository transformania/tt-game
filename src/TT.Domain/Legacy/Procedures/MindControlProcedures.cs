using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public class MindControlProcedures
    {

        public static void AddMindControl(Player attacker, Player victim, int formSourceId)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

            var mc = mcRepo.MindControls.FirstOrDefault(m => m.VictimId == victim.Id && m.MasterId == attacker.Id && m.FormSourceId == formSourceId);

            if (mc == null)
            {
                mc = new MindControl
                {
                    TurnsRemaining = 6,
                    MasterId = attacker.Id,
                    VictimId = victim.Id,
                    FormSourceId = formSourceId
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
            var mcs = mcRepo.MindControls.Where(m => m.MasterId == player.Id || m.VictimId == player.Id).ToList();

            var output = new List<MindControlViewModel>();
            foreach (var mc in mcs)
            {
                var addme = new MindControlViewModel
                {
                    MindControl = mc,
                    Victim = PlayerProcedures.GetPlayerFormViewModel(mc.VictimId),
                    Master = PlayerProcedures.GetPlayerFormViewModel(mc.MasterId),
                    TypeFriendlyName = GetMCFriendlyName(mc.Id),
                };
                output.Add(addme);
            }
            return output;

        }

        public static string GetMCFriendlyName(int id)
        {
            var formRepo = new EFDbStaticFormRepository();
            var form = formRepo.DbStaticForms.SingleOrDefault(f => f.Id == id);

            if (form.Id == MindControlStatics.MindControl__MovementFormSourceId) {
                return "Forced March";
            }
            else if (form.Id == MindControlStatics.MindControl__StripFormSourceId)
            {
                return "Take a Load Off!";
            }
            else if (form.Id == MindControlStatics.MindControl__MeditateFormSourceId)
            {
                return "Am I Bugging You?";
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

        public static ErrorBox AssertBasicMindControlConditions(Player master, Player victim, int formSourceId)
        {
            var output = new ErrorBox();
            output.HasError = true;

            // assert that world update is not in progress
            if (PvPStatics.AnimateUpdateInProgress)
            {
                output.Error = "Animate portion of world update is in progress.";
                output.SubError = "Try again in a few seconds.";
                return output;
            }

            // assert both commander and victim is animate
            if (master.Mobility != PvPStatics.MobilityFull || victim.Mobility != PvPStatics.MobilityFull)
            {
                output.Error = "Both you and your victim must be animate in order to invoke any mind control commands.";
                return output;
            }

            // assert that the victim is not offline
            if (PlayerProcedures.PlayerIsOffline(victim))
            {
                output.Error = "Your victim has gone offline.";
                output.SubError = "You can only issue commands to online players under your mind control.";
                return output;
            }

            // assert that there is indeed a mind control between these two players
            var mcs = MindControlProcedures.GetMindControlsBetweenPlayers(master, victim);

            var mc = mcs.FirstOrDefault(m => m.FormSourceId == formSourceId);

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
            if (mc.TimesUsedThisTurn >= GetCommandLimitOfType(formSourceId))
            {
                output.Error = "You've issued this command too many times this turn.";
                output.SubError = "Wait until next turn to issue your next command.";
                return output;
            }

            // assert that the victim is not mind controlled
            if (victim.InDuel > 0)
            {
                output.Error = "Your victim is in a duel and cannot obey this command.";
                return output;
            }

            // assert that the victim is not mind controlled
            if (victim.InQuest > 0)
            {
                output.Error = "Your victim is in a quest and cannot obey this command.";
                return output;
            }

            output.HasError = false;
            return output;

        }

        public static int GetCommandLimitOfType(int formSourceId)
        {
            if (formSourceId == MindControlStatics.MindControl__MovementFormSourceId)
            {
                return MindControlStatics.MindControl__Movement_Limit;
            }
            else if (formSourceId == MindControlStatics.MindControl__StripFormSourceId)
            {
                return MindControlStatics.MindControl__Strip_Limit;
            }
            else if (formSourceId == MindControlStatics.MindControl__MeditateFormSourceId)
            {
                return MindControlStatics.MindControl__Meditate_Limit;
            }

            return 0;
        }

        public static decimal GetAPCostToMove(BuffBox buffs, string oldLocation, string newLocation)
        {
            var oldLocationl = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == oldLocation);
            var newLocationl = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == newLocation);
            decimal output = Math.Abs(oldLocationl.X - newLocationl.X) + Math.Abs(oldLocationl.Y - newLocationl.Y);
            output *= 1-buffs.MoveActionPointDiscount();

            return output;
        }

        public static bool ClearPlayerMindControlFlagIfOn(Player player)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            if (mcRepo.MindControls.Any(m => m.VictimId == player.Id)) return false;
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            if (dbPlayer == null) return false;
            dbPlayer.MindControlIsActive = false;
            playerRepo.SavePlayer(dbPlayer);
            return true;
        }

        public static bool PlayerIsMindControlledWithType(Player player, int formSourceId)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            return mcRepo.MindControls.Any(p => p.VictimId == player.Id && p.FormSourceId == formSourceId);
        }

        public static bool PlayerIsMindControlledWithType(Player player, IEnumerable<MindControl> controls, int formSourceId)
        {
            return controls.Any(p => p.VictimId == player.Id && p.FormSourceId == formSourceId);
        }

        public static bool PlayerIsMindControlledWithSomeType(Player player, IEnumerable<MindControl> controls)
        {
            return controls.Any(p => p.VictimId == player.Id);
        }

        public static void AddCommandUsedToMindControl(Player master, Player victim, int formSourceId)
        {
            IMindControlRepository mcRepo = new EFMindControlRepository();
            var mc = mcRepo.MindControls.FirstOrDefault(m => m.MasterId == master.Id && m.VictimId == victim.Id && m.FormSourceId == formSourceId);
            mc.TimesUsedThisTurn++;
            mcRepo.SaveMindControl(mc);
        }

    }
}

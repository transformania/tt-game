using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
{
    public class PvPWorldStatProcedures
    {

        public static PvPWorldStat GetWorldStats()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            return worldStatRepo.PvPWorldStats.First();
        }

        public static void StopUpdatingWorld()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.WorldIsUpdating = false;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void UpdateWorldTurnCounter()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();

            PvPWorldStat stat;

            try
            {
                stat = worldStatRepo.PvPWorldStats.First();
     
            }
            catch
            {
                stat = new PvPWorldStat
                {
                    TurnNumber = 0,
                    Boss_DonnaActive = false,
                    Boss_Donna = "unstarted", // "unstarted", "active", or "completed"
                };
            }
            stat.TurnNumber++;
            stat.WorldIsUpdating = true;
            stat.LastUpdateTimestamp = DateTime.UtcNow;

            worldStatRepo.SavePvPWorldStat(stat);

        }

        public static void UpdateWorldTurnCounter_UpdateDone()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.WorldIsUpdating = false;
            stat.LastUpdateTimestamp_Finished = DateTime.UtcNow;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static int GetWorldTurnNumber()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            return worldStatRepo.PvPWorldStats.First().TurnNumber;
        }

        public static DateTime GetLastWorldUpdate()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            return worldStatRepo.PvPWorldStats.First().LastUpdateTimestamp;

        }

        #region Donna
        public static void Boss_StartDonna()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Donna = "active";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndDonna()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Donna = "completed";
            worldStatRepo.SavePvPWorldStat(stat);
        }
        #endregion

        #region Valentine

        public static void Boss_StartValentine()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Valentine = "active";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndValentine()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Valentine = "completed";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        #region Bimbo

        public static void Boss_StartBimbo()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Bimbo = "active";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndBimbo()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Bimbo = "completed";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion


        #region Thieves

        public static void Boss_StartThieves()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Thief = "active";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndThieves()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Thief = "completed";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        #region Mouse Sisters

        public static void Boss_StartSisters()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Sisters = "active";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndSisters()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Sisters = "completed";
            worldStatRepo.SavePvPWorldStat(stat);
        }


        #endregion

        #region Narcissa

        public static void Boss_StartFaeBoss()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Faeboss = "active";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndFaeBoss()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Faeboss = "completed";
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        public static bool IsAnyBossActive()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = worldStatRepo.PvPWorldStats.First();
            return stat.AnyBossIsActive();
        }



    }
}
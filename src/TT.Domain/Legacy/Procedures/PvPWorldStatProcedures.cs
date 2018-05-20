using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.World.Queries;

namespace TT.Domain.Procedures
{
    public class PvPWorldStatProcedures
    {

        private const string COMPLETED = "completed";
        private const string ACTIVE = "active";

        public static PvPWorldStat GetWorldStats()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            return worldStatRepo.PvPWorldStats.First();
        }

        public static void StopUpdatingWorld()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.WorldIsUpdating = false;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void UpdateWorldTurnCounter_UpdateDone()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
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
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Donna = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndDonna()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Donna = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }
        #endregion

        #region Valentine

        public static void Boss_StartValentine()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Valentine = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndValentine()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Valentine = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        #region Bimbo

        public static void Boss_StartBimbo()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Bimbo = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndBimbo()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Bimbo = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion


        #region Thieves

        public static void Boss_StartThieves()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Thief = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndThieves()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Thief = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        #region Mouse Sisters

        public static void Boss_StartSisters()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Sisters = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndSisters()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Sisters = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_StartMotorcycleGang()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_MotorcycleGang = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndMotorcycleGang()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_MotorcycleGang = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        #region Narcissa

        public static void Boss_StartFaeBoss()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Faeboss = ACTIVE;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        public static void Boss_EndFaeBoss()
        {
            IPvPWorldStatRepository worldStatRepo = new EFPvPWorldStatRepository();
            var stat = worldStatRepo.PvPWorldStats.First();
            stat.Boss_Faeboss = COMPLETED;
            worldStatRepo.SavePvPWorldStat(stat);
        }

        #endregion

        public static bool IsAnyBossActive()
        {
            var worldDetails = DomainRegistry.Repository.FindSingle(new GetWorld());
            return worldDetails.AnyBossIsActive();
        }



    }
}
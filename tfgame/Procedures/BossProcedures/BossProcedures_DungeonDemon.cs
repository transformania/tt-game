using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_DungeonDemon
    {
        public static void CounterAttack(Player demon, Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbDemon = playerRepo.Players.FirstOrDefault(f => f.Id == demon.Id);

            if (dbDemon != null && dbDemon.Mobility == "full" && attacker.Mobility == "full")
            {
                AttackProcedures.Attack(dbDemon, attacker, "skill_Lilitu_Yah_Shaddai_Christopher");
                AttackProcedures.Attack(dbDemon, attacker, "skill_Lilitu_Yah_Shaddai_Christopher");
                AttackProcedures.Attack(dbDemon, attacker, "skill_Lilitu_Yah_Shaddai_Christopher");
            }

        }

    }
}
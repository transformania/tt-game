using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            if (dbDemon.Mobility != "full")
            {
                decimal xpGain = 30+5*dbDemon.Level;
                decimal pointsGain = dbDemon.Level * 2;
                PlayerProcedures.GivePlayerPvPScore_NoLoser(attacker, pointsGain);
                string playerLog = "You absorb dark magic from your vanquished opponent, earning " + pointsGain + " points and " + xpGain + " XP.  Unfortunately the demon's new form fades into mist, denying you any other trophies of your conquest.";
                PlayerLogProcedures.AddPlayerLog(attacker.Id, playerLog, true);
                PlayerProcedures.GiveXP(attacker.Id, xpGain);

                Item item = ItemProcedures.GetItemByVictimName(dbDemon.FirstName, dbDemon.LastName);
                ItemProcedures.DeleteItem(item.Id);

                new Thread(() =>
                  StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__DungeonDemonsDefeated, 1)
              ).Start();


            }
            else if (dbDemon != null && dbDemon.Mobility == "full" && attacker.Mobility == "full")
            {
                AttackProcedures.Attack(dbDemon, attacker, "skill_Lilitu_Yah_Shaddai_Christopher");
                AttackProcedures.Attack(dbDemon, attacker, "skill_Lilitu_Yah_Shaddai_Christopher");
                AttackProcedures.Attack(dbDemon, attacker, "skill_Lilitu_Yah_Shaddai_Christopher");
            }

        }

        public static void SpawnArtifactsAndDemons()
        {

        }

    }
}
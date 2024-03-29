﻿using System.Linq;
using System.Threading;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_DungeonDemon
    {
        public static void CounterAttack(Player demon, Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbDemon = playerRepo.Players.FirstOrDefault(f => f.Id == demon.Id);

            if (dbDemon.Mobility != PvPStatics.MobilityFull)
            {
                decimal xpGain = 30+5*dbDemon.Level;
                decimal pointsGain = dbDemon.Level * 2;
                PlayerProcedures.GivePlayerPvPScore_NoLoser(attacker, pointsGain);
                var playerLog = "You absorb dark magic from your vanquished opponent, earning " + pointsGain + " points and " + xpGain + " XP.  Unfortunately the demon's new form fades into mist, denying you any other trophies of your conquest.";
                PlayerLogProcedures.AddPlayerLog(attacker.Id, playerLog, true);
                PlayerProcedures.GiveXP(attacker, xpGain);

                var item = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = dbDemon.Id});
                ItemProcedures.DeleteItem(item.Id);

                DomainRegistry.Repository.Execute(new DeletePlayer {PlayerId = demon.Id});

                StatsProcedures.AddStat(attacker.MembershipId, StatsProcedures.Stat__DungeonDemonsDefeated, 1);
            }
            else if (dbDemon != null && dbDemon.Mobility == PvPStatics.MobilityFull && attacker.Mobility == PvPStatics.MobilityFull)
            {
                var (complete, _) = AttackProcedures.Attack(dbDemon, attacker, PvPStatics.Dungeon_VanquishSpellSourceId);

                if (!complete)
                {
                    (complete, _) = AttackProcedures.Attack(dbDemon, attacker, PvPStatics.Dungeon_VanquishSpellSourceId);
                }

                var dbDemonBuffs = ItemProcedures.GetPlayerBuffs(dbDemon);
                if (dbDemon.Mana < PvPStatics.AttackManaCost * 6)
                {
                    DomainRegistry.Repository.Execute(new Meditate { PlayerId = dbDemon.Id, Buffs = dbDemonBuffs, NoValidate = true });
                }

                if (complete)
                {
                    AIProcedures.EquipDefeatedPlayer(dbDemon, attacker);
                }
            }

        }

    }
}
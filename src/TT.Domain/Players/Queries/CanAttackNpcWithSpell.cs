using System;
using Highway.Data;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Models;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;

namespace TT.Domain.Players.Queries
{
    public class CanAttackNpcWithSpell : DomainQuerySingle<string>
    {
        public DbStaticForm futureForm { get; set; }
        public int spellSourceId { get; set; }
        public Player target { get; set; }
        public Player attacker { get; set; }

        public override string Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                // Donna
                if (target.BotId == AIStatics.DonnaBotId)
                {
                    if (futureForm == null || futureForm.MobilityType == PvPStatics.MobilityFull)
                    {
                        return $"You get the feeling this type of spell won't work against {target.GetFullName()}.  Maybe a different one would do...";
                    }
                }

                // Bimbo Boss
                if (target.BotId == AIStatics.BimboBossBotId)
                {

                    // disallow animate spells
                    if (futureForm == null || futureForm.MobilityType == PvPStatics.MobilityFull)
                    {
                        return $"You get the feeling this type of spell won't work against {target.GetFullName()}.  Maybe a different one would do...";
                    }

                }

                // Thieves Boss
                if (target.BotId == AIStatics.MaleRatBotId || target.BotId == AIStatics.FemaleRatBotId)
                {

                    // only allow inanimate spells
                    if (futureForm == null || futureForm.MobilityType != PvPStatics.MobilityInanimate)
                    {
                        return $"You get the feeling this type of spell won't work against {target.GetFullName()}.  Maybe a different one would do...";
                    }

                }

                // Motorcycle boss
                if (target.BotId == AIStatics.MotorcycleGangLeaderBotId) {

                    if (futureForm == null || !BossProcedures_MotorcycleGang.SpellIsValid(futureForm.MobilityType))
                    {
                        return $"You get the feeling this type of spell won't work against {target.GetFullName()}.  Maybe a different one would do...";
                    }
                }

                // Narcissa
                if (target.BotId == AIStatics.FaebossBotId)
                {
                    var isValid = BossProcedures_FaeBoss.SpellIsValid(spellSourceId, attacker);
                    if (!isValid.Item1)
                    {
                        return isValid.Item2;
                    }
                }

                // Mouse Sisters Boss
                if (target.BotId == AIStatics.MouseNerdBotId || target.BotId == AIStatics.MouseBimboBotId)
                {
                    string result = BossProcedures_Sisters.SpellIsValid(attacker, target, spellSourceId);
                    if (!result.IsNullOrEmpty())
                    {
                        return result;
                    }
                }

                if (AIStatics.IsAMiniboss(target.BotId) && (futureForm == null || (futureForm.MobilityType != PvPStatics.MobilityInanimate && futureForm.MobilityType != PvPStatics.MobilityPet)))
                {
                    return "Your target seems immune from this kind of spell.  Maybe a different one would do...";
                }

                if (target.BotId == AIStatics.MinibossPlushAngelId && (attacker.FormSourceId == 97 || attacker.FormSourceId == 427 || attacker.FormSourceId == 620))
                {
                    return "You two are already the best of friends! Why not try again later?";
                }

                // Dungeon Demons can only be vanquished
                if (target.BotId == AIStatics.DemonBotId && spellSourceId != PvPStatics.Dungeon_VanquishSpellSourceId && spellSourceId != PvPStatics.Spell_WeakenId)
                {
                    return "Only the 'Vanquish' spell and Weaken have any effect on the Dark Demonic Guardians.";
                }

                return "";
            };

            return ExecuteInternal(context);
        }

    }
}

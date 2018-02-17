using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;

namespace TT.Domain.Procedures
{
    public class RerollProcedures {

        public static void AddRerollGeneration(string playerMembershipId)
        {
            IRerollRepository RerollRepo = new EFRerollRepository();
            var playerReroll = RerollRepo.Rerolls.Where(r => r.MembershipId == playerMembershipId).FirstOrDefault();
            if (playerReroll == null)
            {
                // Does not exist, must be a new player. Create new Reroll, assume that it is first generation.
                playerReroll = new Reroll
                {
                    CharacterGeneration = 0,
                    LastCharacterCreation = DateTime.UtcNow,
                    MembershipId = playerMembershipId
                };
            }
            else
            {
                // They exist. Bump up the generation and update the time.
                playerReroll.CharacterGeneration++;
                playerReroll.LastCharacterCreation = DateTime.UtcNow;
            }
            RerollRepo.SaveReroll(playerReroll);
        }

        public static TimeSpan GetTimeUntilReroll(Player player)
        {
            return GetTimeUntilReroll(player.MembershipId);
        }

        public static TimeSpan GetTimeUntilReroll(string playerMembershipId)
        {
            IRerollRepository RerollRepo = new EFRerollRepository();
            var playerReroll = RerollRepo.Rerolls.Where(r => r.MembershipId == playerMembershipId).FirstOrDefault();
            if (playerReroll == null)
            {
                // It's ok to reroll, they are a new first generation
                return TimeSpan.Zero;
            }

            var g = playerReroll.CharacterGeneration;
            if (g >= PvPStatics.RerollTimer.Count()) g = PvPStatics.RerollTimer.Count() - 1;
            var rerollTime = playerReroll.LastCharacterCreation.AddMinutes(PvPStatics.RerollTimer[g]);
            return rerollTime.Subtract(DateTime.UtcNow);
        }
    }
}
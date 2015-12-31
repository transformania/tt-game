using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;
using tfgame.Statics;

namespace tfgame.Procedures
{
    public class RerollProcedures {
        public static bool CheckRerollOk(Player player)
        {
            IRerollRepository RerollRepo = new EFRerollRepository();
            Reroll playerReroll = RerollRepo.Rerolls.Where(r => r.MembershipId == player.MembershipId).FirstOrDefault();
            if (playerReroll == null)
            {
                // It's ok to reroll, they are a new first generation
                return true;
            }

            int g = playerReroll.CharacterGeneration;
            if (g >= PvPStatics.RerollTimer.Count()) g = PvPStatics.RerollTimer.Count()-1;
            DateTime rerollTime = playerReroll.LastCharacterCreation.AddMinutes(PvPStatics.RerollTimer[g]);
            if (rerollTime > DateTime.UtcNow)
            {
                // Reroll time has not passed. It's not ok for the player to Reroll.
                return false;
            }

            // Rerolling is ok!
            return true;
        }

        public static void AddRerollGeneration(Player player)
        {
            AddRerollGeneration(player.MembershipId);
        }

        public static void AddRerollGeneration(string playerMembershipId)
        {
            IRerollRepository RerollRepo = new EFRerollRepository();
            Reroll playerReroll = RerollRepo.Rerolls.Where(r => r.MembershipId == playerMembershipId).FirstOrDefault();
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
            Reroll playerReroll = RerollRepo.Rerolls.Where(r => r.MembershipId == playerMembershipId).FirstOrDefault();
            if (playerReroll == null)
            {
                // It's ok to reroll, they are a new first generation
                return TimeSpan.Zero;
            }

            int g = playerReroll.CharacterGeneration;
            if (g >= PvPStatics.RerollTimer.Count()) g = PvPStatics.RerollTimer.Count() - 1;
            DateTime rerollTime = playerReroll.LastCharacterCreation.AddMinutes(PvPStatics.RerollTimer[g]);
            return rerollTime.Subtract(DateTime.UtcNow);
        }
    }
}
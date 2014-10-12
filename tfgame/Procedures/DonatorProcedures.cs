using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class DonatorProcedures
    {
        public static bool DonatorGetsIcon(Player player)
        {
            if (player.DonatorLevel >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DonatorGetsNickname(Player player)
        {
            if (player.DonatorLevel >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DonatorGetsMessagesRewards(Player player)
        {
            if (player.DonatorLevel >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool BothPlayersAreDonatorsOfTier(Player one, Player two, int tier)
        {
            if (one.DonatorLevel >= tier && two.DonatorLevel >= tier)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool EitherPlayersAreDonatorsOfTier(Player one, Player two, int tier)
        {
            if (one.DonatorLevel >= tier || two.DonatorLevel >= tier)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetNewPlayerDonationRank(string playerFullName)
        {
            IDonatorRepository donatorRepo = new EFDonatorRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            string playerFirstName = playerFullName.Split(' ')[0];
            string playerLastName = playerFullName.Split(' ')[1];

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.FirstName == playerFirstName && p.LastName == playerLastName);

            int rank = 0;

            Donator donatorStatus = donatorRepo.Donators.FirstOrDefault(p => p.OwnerMembershipId == dbPlayer.MembershipId);

            if (donatorStatus == null)
            {
                rank = 0;
                dbPlayer.DonatorLevel = 0;

            }
            else
            {
                dbPlayer.DonatorLevel = donatorStatus.Tier;
            }

            playerRepo.SavePlayer(dbPlayer);

        }
    }
}
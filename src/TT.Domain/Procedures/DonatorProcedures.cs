using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
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

            //add the title on the last name if there is one
            try
            {
                playerLastName += " " + playerFullName.Split(' ')[2];
            }
            catch
            {
                // do nothing, there probably wasn't a title
            }

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.FirstName == playerFirstName && p.LastName == playerLastName);


            Donator donatorStatus = donatorRepo.Donators.FirstOrDefault(p => p.OwnerMembershipId == dbPlayer.MembershipId);

            if (donatorStatus == null)
            {
                dbPlayer.DonatorLevel = 0;

            }
            else
            {
                dbPlayer.DonatorLevel = donatorStatus.Tier;
            }

            playerRepo.SavePlayer(dbPlayer);

        }

        public static void SetNewPlayerDonationRank(int playerId)
        {
            IDonatorRepository donatorRepo = new EFDonatorRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();


            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);


            Donator donatorStatus = donatorRepo.Donators.FirstOrDefault(p => p.OwnerMembershipId == dbPlayer.MembershipId);

            if (donatorStatus == null)
            {
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
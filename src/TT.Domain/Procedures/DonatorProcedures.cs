using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
{
    public static class DonatorProcedures
    {

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
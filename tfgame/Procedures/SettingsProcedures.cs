using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using WebMatrix.WebData;

namespace tfgame.Procedures
{
    public class SettingsProcedures
    {
        public static void SavePlayerBio(PlayerBio bio)
        {
            IPlayerBioRepository playerBioRepo = new EFPlayerBioRepository();

            PlayerBio playerBio = playerBioRepo.PlayerBios.FirstOrDefault(p => p.OwnerMembershipId == WebSecurity.CurrentUserId);

            if (playerBio == null)
            {
                playerBio = new PlayerBio
                {
                    OwnerMembershipId = WebSecurity.CurrentUserId,
                };
            }

            playerBio.PublicVisibility = bio.PublicVisibility;
            playerBio.Timestamp = DateTime.UtcNow;
            playerBio.Text = bio.Text;
            playerBio.WebsiteURL = bio.WebsiteURL;

            playerBioRepo.SavePlayerBio(playerBio);

            // playerBioRe
        }


        public static PlayerBio GetPlayerBioFromMembershipId(int id)
        {
            IPlayerBioRepository playerBioRepo = new EFPlayerBioRepository();
            PlayerBio playerBio = playerBioRepo.PlayerBios.FirstOrDefault(p => p.OwnerMembershipId == id);
            return playerBio;
        }

        public static bool PlayerHasBio(int id)
        {
            IPlayerBioRepository playerBioRepo = new EFPlayerBioRepository();
            PlayerBio playerBio = playerBioRepo.PlayerBios.FirstOrDefault(p => p.OwnerMembershipId == id);
            if (playerBio != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
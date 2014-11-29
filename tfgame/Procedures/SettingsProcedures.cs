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

        public static void SavePoll(PollEntry input, int round, int pollId)
        {
            IPollEntryRepository pollRepo = new EFPollEntriesRepository();
            PollEntry dbPoll = pollRepo.PollEntries.FirstOrDefault(p => p.OwnerMembershipId == WebSecurity.CurrentUserId && p.PollId == pollId);
            if (dbPoll == null)
            {
                dbPoll = new PollEntry();
                dbPoll.OwnerMembershipId = WebSecurity.CurrentUserId;
                dbPoll.PollId = pollId;
            }

            dbPoll.Num1 = input.Num1;
            dbPoll.Num2 = input.Num2;
            dbPoll.Num3 = input.Num3;
            dbPoll.Num4 = input.Num4;
            dbPoll.Num5 = input.Num5;

            dbPoll.String1 = input.String1;
            dbPoll.String2 = input.String2;
            dbPoll.String3 = input.String3;
            dbPoll.String4 = input.String4;
            dbPoll.String5 = input.String5;

            dbPoll.Timestamp = DateTime.UtcNow;

            pollRepo.SavePollEntry(dbPoll);

        }

        public static PollEntry LoadPoll(int pollId)
        {
            IPollEntryRepository pollRepo = new EFPollEntriesRepository();
            PollEntry dbPoll = pollRepo.PollEntries.FirstOrDefault(p => p.OwnerMembershipId == WebSecurity.CurrentUserId && p.PollId == pollId);
            if (dbPoll == null)
            {
                dbPoll = new PollEntry();
            }
            return dbPoll;
        }


    }
}
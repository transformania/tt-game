using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public class SettingsProcedures
    {
        public static void SavePlayerBio(PlayerBio bio, int membershipId)
        {
            IPlayerBioRepository playerBioRepo = new EFPlayerBioRepository();

            PlayerBio playerBio = playerBioRepo.PlayerBios.FirstOrDefault(p => p.OwnerMembershipId == membershipId);

            if (playerBio == null)
            {
                playerBio = new PlayerBio
                {
                    OwnerMembershipId = membershipId,
                };
            }

            playerBio.PublicVisibility = bio.PublicVisibility;
            playerBio.Timestamp = DateTime.UtcNow;
            playerBio.Text = bio.Text;
            playerBio.WebsiteURL = bio.WebsiteURL;

            playerBio.Tags = bio.Tags;

            playerBioRepo.SavePlayerBio(playerBio);

            // playerBioRe
        }

        public static void DeletePlayerBio(int ownerMembershipId)
        {
            IPlayerBioRepository playerBioRepo = new EFPlayerBioRepository();
            PlayerBio myBio = playerBioRepo.PlayerBios.FirstOrDefault(p => p.OwnerMembershipId == ownerMembershipId);
            if (myBio != null) {
                playerBioRepo.DeletePlayerBio(myBio.Id);
            }
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

        public static void SavePoll(PollEntry input, int round, int pollId, int membershipId)
        {
            IPollEntryRepository pollRepo = new EFPollEntriesRepository();
            PollEntry dbPoll = pollRepo.PollEntries.FirstOrDefault(p => p.OwnerMembershipId == membershipId && p.PollId == pollId);
            if (dbPoll == null)
            {
                dbPoll = new PollEntry();
                dbPoll.OwnerMembershipId = membershipId;
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

        public static PollEntry LoadPoll(int pollId, int membershipId)
        {
            IPollEntryRepository pollRepo = new EFPollEntriesRepository();
            PollEntry dbPoll = pollRepo.PollEntries.FirstOrDefault(p => p.OwnerMembershipId == membershipId && p.PollId == pollId);
            if (dbPoll == null)
            {
                dbPoll = new PollEntry();
                dbPoll.PollId = pollId;
            }
            return dbPoll;
        }

        public static IEnumerable<PollEntry> GetAllPollResults(int pollId)
        {
            IPollEntryRepository pollRepo = new EFPollEntriesRepository();
            return pollRepo.PollEntries.Where(p => p.PollId == pollId);
        }

        public static AuthorArtistBio GetAuthorArtistBio(int ownerMembershipId)
        {
            IAuthorArtistBioRepository repo = new EFAuthorArtistBioRepository();
            AuthorArtistBio output = repo.AuthorArtistBios.FirstOrDefault(a => a.OwnerMembershipId == ownerMembershipId);
            if (output != null)
            {
                return output;
            }
            else
            {
                output = new AuthorArtistBio
                {
                    OwnerMembershipId = ownerMembershipId,
                    LastUpdated = DateTime.UtcNow,
                    AcceptingComissions = 0,
                    OtherNames = "",
                    Email = "",
                    PlayerNamePrivacyLevel = 0,
                    Text = "",
                    Url1 = "",
                    Url2 = "",
                    Url3 = "",
                    AnimateImages = "",
                    InanimateImages = "",
                    AnimalImages = "",

                };
                return output;
            }
        }

        public static void SaveAuthorArtistBio(AuthorArtistBio input, int membershipId)
        {
            IAuthorArtistBioRepository repo = new EFAuthorArtistBioRepository();
            AuthorArtistBio saveMe = repo.AuthorArtistBios.FirstOrDefault(a => a.OwnerMembershipId == membershipId);
            if (saveMe == null)
            {
                saveMe = new AuthorArtistBio
                {
                    OwnerMembershipId = membershipId,
                };
            }

            saveMe.AcceptingComissions = input.AcceptingComissions;
            saveMe.Email = input.Email;
            saveMe.OtherNames = input.OtherNames;
            saveMe.PlayerNamePrivacyLevel = input.PlayerNamePrivacyLevel;
            saveMe.Text = input.Text;
            saveMe.Url1 = input.Url1;
            saveMe.Url2 = input.Url2;
            saveMe.Url3 = input.Url3;
            saveMe.AnimateImages = input.AnimateImages;
            saveMe.AnimalImages = input.AnimalImages;
            saveMe.InanimateImages = input.InanimateImages;
            saveMe.LastUpdated = DateTime.UtcNow;
            repo.SaveAuthorArtistBio(saveMe);
        }

        public static bool PlayerHasArtistAuthorBio(int id)
        {
            IAuthorArtistBioRepository repo = new EFAuthorArtistBioRepository();
            AuthorArtistBio bio = repo.AuthorArtistBios.FirstOrDefault(p => p.OwnerMembershipId == id);
            if (bio != null)
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
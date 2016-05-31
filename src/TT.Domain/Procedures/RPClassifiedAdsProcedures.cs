using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Models;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Queries.RPClassifiedAds;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.DTOs.RPClassifiedAds;

namespace TT.Domain.Procedures
{/*
    public static class RPClassifiedAdsProcedures
    {

        /// <summary>
        /// Given an RPClassified ad input, commit these changes to the database
        /// </summary>
        /// <param name="input">RP ad to be committed to the database</param>
        /// <param name="player">Player to own this RP ad</param>
        public static void SaveAd(RPClassifiedAd input, Player player)
        {
            /*IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            RPClassifiedAd ad = repo.RPClassifiedAds.FirstOrDefault(i => i.Id == input.Id);

            if (ad == null)
            {
                ad = new RPClassifiedAd
                {
                    OwnerMembershipId = player.MembershipId,
                    CreationTimestamp = DateTime.UtcNow

                };
            }
            ad.Text = input.Text;
            ad.YesThemes = input.YesThemes;
            ad.NoThemes = input.NoThemes;
            ad.RefreshTimestamp = DateTime.UtcNow;
            ad.Title = input.Title;
            ad.PreferredTimezones = input.PreferredTimezones;

            repo.SaveRPClassifiedAd(ad);


            RPClassifiedAdDetail ad;
            if((ad = DomainRegistry.Repository.FindSingle(new GetRPClassifiedAd() { RPClassifiedAdId = input.Id })) != null)
            {
                var cmd = new UpdateRPClassifiedAd()
                {
                    UserId = player.MembershipId,
                    RPClassifiedAdId = input.Id,

                    Title = input.Title ?? ad.Title,
                    Text = input.Text ?? ad.Text,
                    YesThemes = input.YesThemes ?? ad.YesThemes,
                    NoThemes = input.NoThemes ?? ad.NoThemes,
                    PreferredTimezones = input.PreferredTimezones ?? ad.PreferredTimezones
                };

                DomainRegistry.Repository.Execute(cmd);
            }
            else
            {
                var cmd = new CreateRPClassifiedAd()
                {
                    UserId = player.MembershipId,
                    Title = input.Title,
                    Text = input.Text,
                    YesThemes = input.YesThemes,
                    NoThemes = input.NoThemes,
                    PreferredTimezones = input.PreferredTimezones
                };

                DomainRegistry.Repository.Execute(cmd);
            }
        }

        public static void RefreshAd(int id, string userId)
        {
            /*IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            RPClassifiedAd ad = repo.RPClassifiedAds.FirstOrDefault(i => i.Id == id);
            ad.RefreshTimestamp = DateTime.UtcNow;
            repo.SaveRPClassifiedAd(ad);

            DomainRegistry.Repository.Execute(new RefreshRPClassifiedAd() { RPClassifiedAdId = id, UserId = userId });
        }

        public static void DeleteAd(int id, string userId)
        {
            /*
            IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            repo.DeleteRPClassifiedAd(id);
            

            DomainRegistry.Repository.Execute(new DeleteRPClassifiedAd() { RPClassifiedAdId = id, UserId = userId });
        }

        public static IEnumerable<RPClassifiedAdDetail> GetPlayersClassifiedAds(Player player)
        {
            /*
            IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            return repo.RPClassifiedAds.Where(i => i.OwnerMembershipId == player.MembershipId);
            

            return DomainRegistry.Repository.Find(new GetUserRPClassifiedAds() { UserId = player.MembershipId });
        }

        public static IEnumerable<RPClassifiedAdDetail> GetClassifiedAds()
        {
            /*IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            DateTime markOnlineCutoff = DateTime.UtcNow.AddDays(-3);
            return repo.RPClassifiedAds.Where(i => i.RefreshTimestamp > markOnlineCutoff);

            return DomainRegistry.Repository.Find(new GetRPClassifiedAds() { CutOff = DateTime.UtcNow.AddDays(-3) });
        }

        public static RPClassifiedAd GetClassifiedAd(int id)
        {
            IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            RPClassifiedAd output = repo.RPClassifiedAds.FirstOrDefault(i => i.Id == id);

            if (output == null)
            {
                output = new RPClassifiedAd();
            }

            return output;
        }

        public static int GetPlayerClassifiedAdCount(Player player)
        {
            IRPClassifiedAdRepository repo = new EFRPClassifiedAdsRepository();
            return repo.RPClassifiedAds.Where(i => i.OwnerMembershipId == player.MembershipId).Count();
        }

      

    }*/
}
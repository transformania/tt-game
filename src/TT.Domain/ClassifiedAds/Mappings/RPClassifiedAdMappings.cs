using System.Data.Entity.ModelConfiguration;
using AutoMapper;
using TT.Domain.ClassifiedAds.DTOs;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Players.Entities;

namespace TT.Domain.ClassifiedAds.Mappings
{
    public class RPClassifiedAdModelBuilder : EntityTypeConfiguration<RPClassifiedAd>
    {
        public RPClassifiedAdModelBuilder()
        {
            ToTable("RPClassifiedAds");
            HasKey(cr => cr.Id);
            HasRequired(cr => cr.User).WithMany(u => u.RPClassifiedAds).HasForeignKey(cr => cr.OwnerMembershipId);
        }
    }

    public class RPClassifiedAdMapping : Profile
    {
        protected override void Configure()
        {
            CreateMap<RPClassifiedAd, RPClassifiedAdDetail>();
            CreateMap<Player, PlayerDetail>();
        }
    }
}

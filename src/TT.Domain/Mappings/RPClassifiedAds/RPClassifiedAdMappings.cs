using AutoMapper;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.DTOs.RPClassifiedAds;
using Highway.Data;
using System.Data.Entity;
using TT.Domain.Entities.Players;
using TT.Domain.Entities.Identity;
using System.Linq;
using System.Data.Entity.ModelConfiguration;

namespace TT.Domain.Mappings.RPClassifiedAds
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

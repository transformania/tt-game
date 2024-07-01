using System.Data.Entity.ModelConfiguration;
using TT.Domain.Entities.RPClassifiedAds;

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
}

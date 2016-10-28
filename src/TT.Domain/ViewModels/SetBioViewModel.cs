using System;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class SetBioViewModel
    {
        public int Id { get; set; }
        public string OwnerMembershipId { get; set; }
        public string Text { get; set; }
        public string WebsiteURL { get; set; }
        public int PublicVisibility { get; set; }
        public string OtherNames { get; set; }
        public string Tags { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsDonator { get; set; }

        public SetBioViewModel()
        {
            Timestamp = DateTime.UtcNow;
            WebsiteURL = "";
            Text = "";
        }

        public SetBioViewModel(PlayerBio playerBio)
        {
            Id = playerBio.Id;
            OwnerMembershipId = playerBio.OwnerMembershipId;
            Text = playerBio.Text;
            WebsiteURL = playerBio.WebsiteURL;
            PublicVisibility = playerBio.PublicVisibility;
            Tags = playerBio.Tags;
            Timestamp = playerBio.Timestamp;
        }

        public SetBioViewModel(string ownerMembershipId) : this()
        {
            OwnerMembershipId = ownerMembershipId;
        }
    }
}

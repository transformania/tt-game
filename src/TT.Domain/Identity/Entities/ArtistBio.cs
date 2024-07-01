using System;
using TT.Domain.Entities;
using TT.Domain.Identity.DTOs;

namespace TT.Domain.Identity.Entities
{
    public class ArtistBio : Entity<int>
    {
        public User Owner { get; protected set; }
        public int PlayerNamePrivacyLevel { get; protected set; }
        public string OtherNames { get; protected set; }
        public string Email { get; protected set; }
        public string Url1 { get; protected set; }
        public string Url2 { get; protected set; }
        public string Url3 { get; protected set; }
        public string Text { get; protected set; }
        public int AcceptingComissions { get; protected set; }
        public DateTime LastUpdated { get; protected set; }
        public string AnimateImages { get; protected set; }
        public string InanimateImages { get; protected set; }
        public string AnimalImages { get; protected set; }
        public bool IsLive { get; protected set; }

        private ArtistBio() { }

        public void SetLiveStatus(bool isLive)
        {
            IsLive = isLive;
        }

        public ArtistBioDetail MapToDto()
        {
            return new ArtistBioDetail
            {
                Id = Id,
                Owner = Owner.MapToDto(),
                PlayerNamePrivacyLevel = PlayerNamePrivacyLevel,
                OtherNames = OtherNames,
                Email = Email,
                Url1 = Url1,
                Url2 = Url2,
                Url3 = Url3,
                Text = Text,
                AcceptingComissions = AcceptingComissions,
                LastUpdated = LastUpdated,
                AnimateImages = AnimateImages,
                InanimateImages = InanimateImages,
                AnimalImages = AnimalImages,
                IsLive = IsLive
            };
        }
    }
}

using System;

namespace TT.Domain.DTOs.Identity
{
    public class ArtistBioDetail
    {
        public int Id { get; set; }
        public UserDetail Owner { get; set; }
        public int PlayerNamePrivacyLevel { get; set; }
        public string OtherNames { get; set; }
        public string Email { get; set; }
        public string Url1 { get; set; }
        public string Url2 { get; set; }
        public string Url3 { get; set; }
        public string Text { get; set; }
        public int AcceptingComissions { get; set; }
        public DateTime LastUpdated { get; set; }
        public string AnimateImages { get; set; }
        public string InanimateImages { get; set; }
        public string AnimalImages { get; set; }
        public bool IsLive { get; set; }

    }
}

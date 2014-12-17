using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class AuthorArtistBio
    {
        public int Id { get; set; }
        public int OwnerMembershipId { get; set; }
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
    }
}
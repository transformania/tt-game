using System.Collections.Generic;
using TT.Domain.Identity.DTOs;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class ContributePageViewModel
    {
        public Player Me { get; set; }
        public bool HasPublicArtistBio { get; set; }
        public IEnumerable<ArtistBioDetail> ArtistBios { get; set; }
        public IEnumerable<string> SelectedForms { get; set; }
    }
}

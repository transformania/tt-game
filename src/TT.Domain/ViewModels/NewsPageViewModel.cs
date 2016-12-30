using System.Collections.Generic;
using TT.Domain.DTOs.Identity;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class NewsPageViewModel
    {
        public IEnumerable<ArtistBioDetail> ArtistBios { get; set; }
        public IEnumerable<NewsPost> NewsPosts { get; set; }
    }
}

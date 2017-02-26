using System.Collections.Generic;
using TT.Domain.Identity.DTOs;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class NewsPageViewModel
    {
        public IEnumerable<ArtistBioDetail> ArtistBios { get; set; }
        public IEnumerable<NewsPost> NewsPosts { get; set; }
    }
}

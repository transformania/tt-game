using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IAuthorArtistBioRepository
    {

        IQueryable<AuthorArtistBio> AuthorArtistBios { get; }

        void SaveAuthorArtistBio(AuthorArtistBio AuthorArtistBio);

        void DeleteAuthorArtistBio(int AuthorArtistBioId);

    }
}
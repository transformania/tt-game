using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IAuthorArtistBioRepository
    {

        IQueryable<AuthorArtistBio> AuthorArtistBios { get; }

        void SaveAuthorArtistBio(AuthorArtistBio AuthorArtistBio);

        void DeleteAuthorArtistBio(int AuthorArtistBioId);

    }
}
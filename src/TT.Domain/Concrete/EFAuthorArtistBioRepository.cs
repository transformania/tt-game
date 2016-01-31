using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFAuthorArtistBioRepository : IAuthorArtistBioRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<AuthorArtistBio> AuthorArtistBios
        {
            get { return context.AuthorArtistBios; }
        }

        public void SaveAuthorArtistBio(AuthorArtistBio AuthorArtistBio)
        {
            if (AuthorArtistBio.Id == 0)
            {
                context.AuthorArtistBios.Add(AuthorArtistBio);
            }
            else
            {
                AuthorArtistBio editMe = context.AuthorArtistBios.Find(AuthorArtistBio.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = AuthorArtistBio.Name;
                    // dbEntry.AuthorArtistBio = AuthorArtistBio.AuthorArtistBio;
                    // dbEntry.TimeStamp = AuthorArtistBio.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteAuthorArtistBio(int id)
        {

            AuthorArtistBio dbEntry = context.AuthorArtistBios.Find(id);
            if (dbEntry != null)
            {
                context.AuthorArtistBios.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
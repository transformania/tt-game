using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFNewsPostRepository : INewsPostRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<NewsPost> NewsPosts
        {
            get { return context.NewsPosts; }
        }

        public void SaveNewsPost(NewsPost NewsPost)
        {
            if (NewsPost.Id == 0)
            {
                context.NewsPosts.Add(NewsPost);
            }
            else
            {
                NewsPost editMe = context.NewsPosts.Find(NewsPost.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = NewsPost.Name;
                    // dbEntry.Message = NewsPost.Message;
                    // dbEntry.TimeStamp = NewsPost.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteNewsPost(int id)
        {

            NewsPost dbEntry = context.NewsPosts.Find(id);
            if (dbEntry != null)
            {
                context.NewsPosts.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
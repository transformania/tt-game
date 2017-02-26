using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface INewsPostRepository
    {

        IQueryable<NewsPost> NewsPosts { get; }

        void SaveNewsPost(NewsPost NewsPost);

        void DeleteNewsPost(int NewsPostId);

    }
}
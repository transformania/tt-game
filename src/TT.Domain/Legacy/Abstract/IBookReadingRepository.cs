using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IBookReadingRepository
    {

        IQueryable<BookReading> BookReadings { get; }

        void SaveBookReading(BookReading BookReading);

        void DeleteBookReading(int BookReadingId);

    }
}
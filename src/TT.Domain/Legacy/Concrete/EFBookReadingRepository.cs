using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFBookReadingRepository : IBookReadingRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<BookReading> BookReadings
        {
            get { return context.BookReadings; }
        }

        public void SaveBookReading(BookReading BookReading)
        {
            if (BookReading.Id == 0)
            {
                context.BookReadings.Add(BookReading);
            }
            else
            {
                BookReading editMe = context.BookReadings.Find(BookReading.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = BookReading.Name;
                    // dbEntry.Message = BookReading.Message;
                    // dbEntry.TimeStamp = BookReading.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteBookReading(int id)
        {

            BookReading dbEntry = context.BookReadings.Find(id);
            if (dbEntry != null)
            {
                context.BookReadings.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
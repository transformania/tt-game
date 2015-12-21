using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IBookReadingRepository
    {

        IQueryable<BookReading> BookReadings { get; }

        void SaveBookReading(BookReading BookReading);

        void DeleteBookReading(int BookReadingId);

    }
}
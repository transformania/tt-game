using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IPollEntryRepository
    {

        IQueryable<PollEntry> PollEntries { get; }

        void SavePollEntry(PollEntry PollEntry);

        void DeletePollEntry(int PollEntryId);

    }
}
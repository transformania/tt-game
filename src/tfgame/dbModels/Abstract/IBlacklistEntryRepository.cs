using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IBlacklistEntryRepository
    {

        IQueryable<BlacklistEntry> BlacklistEntries { get; }

        void SaveBlacklistEntry(BlacklistEntry BlacklistEntry);

        void DeleteBlacklistEntry(int BlacklistEntryId);

    }
}
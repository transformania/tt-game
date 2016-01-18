using System.Linq;
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
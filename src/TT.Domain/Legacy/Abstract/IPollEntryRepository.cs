using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPollEntryRepository
    {

        IQueryable<PollEntry> PollEntries { get; }

        void SavePollEntry(PollEntry PollEntry);

        void DeletePollEntry(int PollEntryId);

    }
}
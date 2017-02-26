using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IBlacklistEntryRepository
    {

        IQueryable<BlacklistEntry> BlacklistEntries { get; }

        void SaveBlacklistEntry(BlacklistEntry BlacklistEntry);

        void DeleteBlacklistEntry(int BlacklistEntryId);

    }
}
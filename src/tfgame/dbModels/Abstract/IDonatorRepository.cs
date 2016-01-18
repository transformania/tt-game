using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDonatorRepository
    {

        IQueryable<Donator> Donators { get; }

        void SaveDonator(Donator Donator);

        void DeleteDonator(int DonatorId);

    }
}
using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDonatorRepository
    {

        IQueryable<Donator> Donators { get; }

        void SaveDonator(Donator Donator);

        void DeleteDonator(int DonatorId);

    }
}
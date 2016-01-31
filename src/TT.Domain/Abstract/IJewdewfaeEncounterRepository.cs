using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IJewdewfaeEncounterRepository
    {

        IQueryable<JewdewfaeEncounter> JewdewfaeEncounters { get; }

        void SaveJewdewfaeEncounter(JewdewfaeEncounter JewdewfaeEncounter);

        void DeleteJewdewfaeEncounter(int JewdewfaeEncounterId);

    }
}
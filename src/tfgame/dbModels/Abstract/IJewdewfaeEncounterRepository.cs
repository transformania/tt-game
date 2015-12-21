using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IJewdewfaeEncounterRepository
    {

        IQueryable<JewdewfaeEncounter> JewdewfaeEncounters { get; }

        void SaveJewdewfaeEncounter(JewdewfaeEncounter JewdewfaeEncounter);

        void DeleteJewdewfaeEncounter(int JewdewfaeEncounterId);

    }
}
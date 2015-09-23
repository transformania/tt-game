using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class QuestProcedures
    {

        public static IEnumerable<QuestStart> GetAllQuestStarts()
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStarts;
        }


        public static bool PlayerCanQuestHere(string location, Player player)
        {

            return true;
        }
    }
}
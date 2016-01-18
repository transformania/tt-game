using System.Collections.Generic;
using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;

namespace tfgame.Statics
{
    public static class FormStatics
    {

        public static DbStaticForm GetForm(string dbFormName)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.DbStaticForms.FirstOrDefault(s => s.dbName == dbFormName);
        }

        public static IEnumerable<DbStaticForm> GetAllAnimateForms()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.DbStaticForms.Where(s => s.MobilityType == "full");
        }

        public static List<RAMBuffBox> FormRAMBuffBoxes;

    }

}
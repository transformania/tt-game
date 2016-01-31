using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.ViewModels;

namespace TT.Domain.Statics
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
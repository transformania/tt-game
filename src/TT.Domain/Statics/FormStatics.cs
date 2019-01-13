using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    public static class FormStatics
    {

        public static DbStaticForm GetForm(int formSourceId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.DbStaticForms.FirstOrDefault(s => s.Id == formSourceId);
        }

        public static IEnumerable<DbStaticForm> GetAllAnimateForms()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.DbStaticForms.Where(s => s.MobilityType == PvPStatics.MobilityFull);
        }
    }

}
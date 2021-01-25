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

        // IDs of animate forms an item can curse its owner into if no TF curse is specified for the item.
        // Set to empty or null to disable.
        public static readonly int[] DefaultTFCurseForms = { };
    }
}
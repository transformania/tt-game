using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IMindControlRepository
    {

        IQueryable<MindControl> MindControls { get; }

        void SaveMindControl(MindControl MindControl);

        void DeleteMindControl(int MindControlId);

    }
}
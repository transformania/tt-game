using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IMindControlRepository
    {

        IQueryable<MindControl> MindControls { get; }

        void SaveMindControl(MindControl MindControl);

        void DeleteMindControl(int MindControlId);

    }
}
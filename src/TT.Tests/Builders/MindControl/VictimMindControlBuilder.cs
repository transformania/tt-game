using TT.Domain.Entities.MindControl;

namespace TT.Tests.Builders.MindControl
{
    public class VictimMindControlBuilder : Builder<VictimMindControl, int>
    {
        public VictimMindControlBuilder()
        {
            Instance = Create();
            With(u => u.Id, 50);
        }
    }
}

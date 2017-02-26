using TT.Domain.Players.Entities;

namespace TT.Domain.Entities.MindControl
{
    public class VictimMindControl : Entity<int>
    {
        public Player Victim { get; protected set; }
        public int TurnsRemaining { get; protected set; }
        public string Type { get; protected set; }
        public int TimesUsedThisTurn { get; protected set; }

        private VictimMindControl() { }
    }
}

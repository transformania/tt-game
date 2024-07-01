using TT.Domain.Combat.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Entities.MindControl
{
    public class VictimMindControl : Entity<int>
    {
        public Player Victim { get; protected set; }
        public int TurnsRemaining { get; protected set; }
        public int FormSourceId { get; protected set; }
        public int TimesUsedThisTurn { get; protected set; }

        private VictimMindControl() { }

        public VictimMindControlDetail MapToDto()
        {
            return new VictimMindControlDetail
            {
                Id = this.Id,
                TurnsRemaining = this.TurnsRemaining,
                FormSourceId = this.FormSourceId,
                TimesUsedThisTurn = this.TimesUsedThisTurn,
                Victim = Victim.MapToDto(),
            };
        }
    }
}

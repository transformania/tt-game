using TT.Domain.Players.DTOs;

namespace TT.Domain.Combat.DTOs
{
    public class VictimMindControlDetail
    {

        public int Id { get; set; }
        public PlayerDetail Victim { get; set; }
        public int TurnsRemaining { get; set; }
        public string Type { get; set; }
        public int TimesUsedThisTurn { get; set; }

    }
}

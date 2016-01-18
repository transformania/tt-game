namespace tfgame.dbModels.Models
{
    public class MindControl
    {
        public int Id { get; set;}
        public int MasterId { get; set; }
        public int VictimId { get; set; }
        public int TurnsRemaining { get; set; }
        public string Type { get; set; }
        public int TimesUsedThisTurn { get; set; }

    }
}
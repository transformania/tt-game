namespace TT.Domain.World.DTOs
{
    public class PvPLeaderboardEntryDetail
    {
        public int Rank { get;  set; }
        public string PlayerName { get;  set; }
        public string Sex { get;  set; }
        public string CovenantName { get;  set; }
        public string FormName { get;  set; }
        public string Mobility { get;  set; }
        public FormImageDetail FormSource { get;  set; }
        public int Level { get;  set; }
        public int DungeonPoints { get;  set; }
    }

    public class FormImageDetail
    {
        public string PortraitUrl { get; set; }
    }
}

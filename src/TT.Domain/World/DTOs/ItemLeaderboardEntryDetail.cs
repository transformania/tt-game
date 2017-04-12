namespace TT.Domain.World.DTOs
{
    public class ItemLeaderboardEntryDetail
    {
        public int Rank { get; set; }
        public string PlayerName { get; set; }
        public int GameMode { get; set; }
        public string Sex { get; set; }
        public string ItemName { get; set; }
        public ItemImageDetail ItemSource { get; set; }
        public string CovenantName { get; set; }
        public string ItemType { get; set; }

        public int Level { get; set; }
        public float XP { get; set; }
    }

    public class ItemImageDetail
    {
        public string PortraitUrl { get; set; }
    }

}

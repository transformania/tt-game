using TT.Domain.DTOs.Players;

namespace TT.Domain.DTOs.Item
{
    public class ItemFormerPlayerDetail
    {
        public int Id { get; set; }
        public ItemSourceDetail ItemSource { get; set; }
        public PlayerDetail FormerPlayer { get; set; }
        public int Level { get; set; }
        public bool IsPermanent { get; set; }
        public string Nickname { get; set; }

    }
}

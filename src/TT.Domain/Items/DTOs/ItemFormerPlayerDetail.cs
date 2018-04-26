using TT.Domain.Players.DTOs;

namespace TT.Domain.Items.DTOs
{
    public class ItemFormerPlayerDetail
    {
        public int Id { get; set; }
        public ItemSourceDetail ItemSource { get; set; }
        public PlayerDetail FormerPlayer { get; set; }
        public int Level { get; set; }
        public bool IsPermanent { get; set; }
    }
}

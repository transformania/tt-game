using System.Collections.Generic;
using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels
{
    public class TalkToSoulbinderViewModel
    {
        public IEnumerable<ItemDetail> Items { get; set; }
        public int Money { get; set; }
        public IEnumerable<ItemDetail> AllSoulboundItems { get; set; }
        public IEnumerable<ItemDetail> NPCOwnedSoulboundItems { get; set; }
    }
}

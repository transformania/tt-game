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

        public bool CanSoulbind(ItemDetail item)
        {
            return item.FormerPlayer != null &&
                   item.IsPermanent &&
                   item.SoulboundToPlayer == null &&
                   item.ConsentsToSoulbinding;
        }

        public string GetReason(ItemDetail item)
        {
            if (item.SoulboundToPlayer != null)
            {
                return "Already soulbound";
            }
            else if (item.FormerPlayer == null)
            {
                return "Item is not souled";
            }
            else if (!item.IsPermanent)
            {
                return "Item is not locked";
            }
            else if (!item.ConsentsToSoulbinding)
            {
                return "Item has not consented";
            }

            return "";
        }
    }
}

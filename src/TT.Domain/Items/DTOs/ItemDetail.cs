using System;
using System.Collections.Generic;

namespace TT.Domain.Items.DTOs
{
    public class ItemDetail
    {
        public int Id { get;  set; }
        public string dbName { get;  set; }
        public InventoryItemSource ItemSource { get;  set; }
        public PlayPageItemDetail.PlayPagePlayerDetail Owner { get;  set; }
        public PlayPageItemDetail.PlayPagePlayerDetail FormerPlayer { get;  set; }
        public string dbLocationName { get;  set; }
        public bool IsEquipped { get;  set; }
        public int TurnsUntilUse { get;  set; }
        public int Level { get;  set; }
        public bool EquippedThisTurn { get;  set; }
        public int PvPEnabled { get;  set; }
        public bool IsPermanent { get;  set; }
        public DateTime LastSouledTimestamp { get;  set; }
        public DateTime LastSold { get; protected set; }
        public ICollection<ItemRuneDetail> Runes { get; set; }
        public ItemRuneDetail EmbeddedOnItem { get; set; }
    }

    public class InventoryItemSource
    {

        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string PortraitUrl { get; set; }
        public decimal MoneyValue { get; set; }
        public decimal MoneyValueSell { get; set; }
        public string ItemType { get; set; }
        public int? GivesEffectSourceId { get; set; }
        public string CurseTFFormdbName { get; set; }

        public int? CurseTFFormSourceId { get; set; }

        public float Discipline { get; set; }
        public float Perception { get; set; }
        public float Charisma { get; set; }

        public float Fortitude { get; set; }
        public float Agility { get; set; }
        public float Allure { get; set; }

        public float Magicka { get; set; }
        public float Succour { get; set; }
        public float Luck { get; set; }

        public int? RuneLevel { get; set; }

    }

}

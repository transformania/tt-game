using System;
using TT.Domain.Players.DTOs;

namespace TT.Domain.Items.DTOs
{
    public class ItemDetail
    {
        public int Id { get;  set; }
        public string dbName { get;  set; }
        public ItemSourceDetail ItemSource { get;  set; }
        public PlayerDetail Owner { get;  set; }
        public PlayerDetail FormerPlayer { get; set; }
        public string dbLocationName { get;  set; }
        public string VictimName { get;  set; }
        public bool IsEquipped { get;  set; }
        public int TurnsUntilUse { get;  set; }
        public int Level { get;  set; }
        public DateTime TimeDropped { get;  set; }
        public bool EquippedThisTurn { get;  set; }
        public int PvPEnabled { get;  set; }
        public bool IsPermanent { get;  set; }
        public string Nickname { get;  set; }
        public DateTime LastSouledTimestamp { get;  set; }
        public DateTime LastSold { get;  set; }

        public string GetFullName()
        {
            if (this.Nickname.IsNullOrEmpty())
            {
                return VictimName;
            }
            else
            {
                string[] nameArray = this.VictimName.Split(' ');
                return nameArray[0] + " '" + this.Nickname + "' " + nameArray[1];
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class DbStaticFurniture
    {
        public int Id { get; set; }
        public string dbType { get; set; }
        public string FriendlyName { get; set; }
        public string GivesEffect { get; set; }
        public decimal APReserveRefillAmount { get; set; }
        public decimal BaseCost { get; set; }
        public int BaseContractTurnLength { get; set; }
        public string GivesItem { get; set; }
        public decimal MinutesUntilReuse { get; set; }
        public string Description { get; set; }
        public string PortraitUrl { get; set; }
    }
}
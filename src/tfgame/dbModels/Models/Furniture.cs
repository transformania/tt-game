using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Furniture
    {
        public int Id { get; set; }
        public string dbType { get; set; }
        public int CovenantId { get; set; }
        public DateTime LastUseTimestamp { get; set; }
        public int ContractTurnDuration { get; set; }
        public int ContractStartTurn { get; set; }
        public int ContractEndTurn { get; set; }
        public decimal Price { get; set; }
        public string LastUsersIds { get; set; }
        public string HumanName { get; set; }
    }

    public class Furniture_VM
    {
        public int Id { get; set; }
        public string dbType { get; set; }
        public int CovenantId { get; set; }
        public DateTime LastUseTimestamp { get; set; }
        public int ContractTurnDuration { get; set; }
        public int ContractStartTurn { get; set; }
        public int ContractEndTurn { get; set; }
        public decimal Price { get; set; }
        public string LastUsersIds { get; set; }
        public string HumanName { get; set; }
    }
}
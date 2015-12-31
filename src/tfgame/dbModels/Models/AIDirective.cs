using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class AIDirective
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string State { get; set; }
        public int TargetPlayerId { get; set; }
        public string TargetLocation { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Var1 { get; set; }
        public decimal Var2 { get; set; }
        public decimal Var3 { get; set; }
        public decimal Var4 { get; set; }
        public decimal Var5 { get; set; }
        public string sVar1 { get; set; }
        public string sVar2 { get; set; }
        public string sVar3 { get; set; }
        public bool DoNotRecycleMe { get; set; }
        public int SpawnTurn { get; set; }
    }
}
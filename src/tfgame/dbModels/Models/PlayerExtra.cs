using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class PlayerExtra
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ProtectionToggleTurnsRemaining { get; set; }
    }
}
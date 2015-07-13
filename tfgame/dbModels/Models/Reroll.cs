using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Reroll
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public int CharacterGeneration { get; set; }
        public DateTime LastCharacterCreation { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace tfgame.dbModels.Models
{
    public class Reroll
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string MembershipId { get; set; }
        public int CharacterGeneration { get; set; }
        public DateTime LastCharacterCreation { get; set; }
    }
}
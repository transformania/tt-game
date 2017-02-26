using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
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
using System;
using TT.Domain.DTOs.AI;
using TT.Domain.DTOs.Identity;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Forms;

namespace TT.Domain.DTOs.Players
{
    public class PlayerDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public UserDetail User { get; set; }
        public FormSource FormSource { get; set; }
        public NPCDetail NPC { get; set; }
        public string Gender { get; set; }

        public int DonatorLevel { get; set; }

        public InanimateXPDetail ItemXP { get; set; }

        public string Mobility { get; set; }

        public int BotId { get; set; } // TODO:  convert this to a nullable FK referencing NPC table

        public string FullName
        {
            get
            {
                return DonatorLevel < 2 || String.IsNullOrEmpty(Nickname) ?
                    $"{FirstName} {LastName}" :
                    $"{FirstName} '{Nickname}' {LastName}";
            }
        }

    }
}

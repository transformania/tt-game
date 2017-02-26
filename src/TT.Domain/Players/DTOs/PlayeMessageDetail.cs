using System;

namespace TT.Domain.Players.DTOs
{
    public class PlayerMessageDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }

        public string GetFullName()
        {
            if (this.DonatorLevel >= 2 && !this.Nickname.IsNullOrEmpty())
            {
                return this.FirstName + " '" + this.Nickname + "' " + this.LastName;
            }
            else
            {
                return this.FirstName + " " + this.LastName;
            }
        }
    }
}

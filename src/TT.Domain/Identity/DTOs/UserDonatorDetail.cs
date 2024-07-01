using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.DTOs
{
    public class UserDonatorDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public DonatorDetail Donator { get; set; }

        public string GetStyleColor()
        {
            var caseSwitch = this.Donator?.Tier;
            return caseSwitch switch
            {
                0 => "lightgray",
                1 => "white",
                2 => "cyan",
                3 => "lightgreen",
                _ => ""
            };
        }
    }
}

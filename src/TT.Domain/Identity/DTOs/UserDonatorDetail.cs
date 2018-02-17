using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.DTOs
{
    public class UserDonatorDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public Donator Donator { get; set; }

        public string GetStyleColor()
        {
            var caseSwitch = this.Donator.Tier;
            switch (caseSwitch)
            {
                case 0:
                    return "lightgray";
                case 1:
                    return "white";
                case 2:
                    return "cyan";
                case 3:
                    return "lightgreen";
            }
            return "";
        }
    }
}

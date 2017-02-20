using System.Collections.Generic;
using TT.Domain.DTOs.Identity;

namespace TT.Domain.ViewModels
{
    public class SettingsPageViewModel
    {
        public TT.Domain.Models.Player Player { get; set; }
        public double TimeUntilReroll { get; set; }
        public double TimeUntilLogout { get; set; }
        public IEnumerable<StrikeDetail> Strikes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using TT.Domain.Identity.DTOs;

namespace TT.Domain.ViewModels
{
    public class SettingsPageViewModel
    {
        public TT.Domain.Models.Player Player { get; set; }
        public TT.Domain.Models.Item PlayerItem { get; set; }
        public double TimeUntilReroll { get; set; }
        public double TimeUntilLogout { get; set; }
        public IEnumerable<StrikeDetail> Strikes { get; set; }
        public bool ChaosChangesEnabled { get; set; }
    }
}

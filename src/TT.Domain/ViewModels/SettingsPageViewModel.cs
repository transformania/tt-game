using System;
using System.Collections.Generic;
using TT.Domain.Identity.DTOs;

namespace TT.Domain.ViewModels
{
    public class SettingsPageViewModel
    {
        public TT.Domain.Models.Player Player { get; set; }
        public TT.Domain.Items.DTOs.ItemDetail PlayerItem { get; set; }
        public double TimeUntilReroll { get; set; }
        public double TimeUntilLogout { get; set; }
        public IEnumerable<StrikeDetail> Strikes { get; set; }
        public bool ChaosChangesEnabled { get; set; }
        public bool OwnershipVisibilityEnabled { get; set; }
        public bool IsOnlineToggled { get; set; }
        public bool FriendOnlyMessages { get; set; }
        public string ReservedName { get; set; }
        public bool IsBossDisabled { get; set; }
        public List<int> GivenBossForms { get; set; }
        public bool BossEnableAfterDefeat { get; set; }
        public bool BossActive { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Entities;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.DTOs;

namespace TT.Domain.Identity.Entities
{
    public class User : Entity<string>
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }

        public UserSecurityStamp SecurityStamp { get; private set; }

        public ICollection<RPClassifiedAd> RPClassifiedAds { get; private set; } = new List<RPClassifiedAd>();
        public ICollection<Stat> Stats { get; private set; } = new List<Stat>();
        public ICollection<Strike> Strikes { get; private set; } = new List<Strike>();
        public ICollection<Strike> StrikesGiven { get; private set; } = new List<Strike>();
        public ICollection<Report> ReportsReceived { get; private set; } = new List<Report>(); // reports this user has received from other players
        public ICollection<Report> ReportsGiven { get; private set; } = new List<Report>(); // reports this user has assigned against other players
        public ICollection<Role> Roles { get; private set; } = new List<Role>();
        public Donator Donator { get; protected set; }
        public DateTime? LockoutEndDateUtc { get; protected set; }
        public String AccountLockoutMessage { get; protected set; }
        public String PvPLockoutMessage { get; protected set; }

        public ArtistBio ArtistBio { get; protected set; }

        public bool AllowOwnershipVisibility { get; protected set; }
        public bool AllowChaosChanges { get; protected set; }
        public bool PvPLock { get; protected set; }
        public bool OnlineToggle { get; protected set; }

        private User() { }

        public void UpdateDonator(UpdateDonator cmd)
        {
            this.Donator.Update(cmd.PatreonName, cmd.Tier, cmd.ActualDonationAmount, cmd.SpecialNotes);
        }

        public void CreateDonator(CreateDonator cmd)
        {
            this.Donator = Donator.Create(cmd.PatreonName, cmd.Tier, cmd.ActualDonationAmount, cmd.SpecialNotes);
        }

        public void AddStat(string type, float amount)
        {
            var stat = this.Stats.FirstOrDefault(a => a.AchievementType == type);
            if (stat == null)
            {
                this.Stats.Add(Stat.Create(this, amount, type));
            }
            else
            {
                stat.AddAmount(amount);
            }
        }

        public void SetAllowChaosChanges(bool allowChaosChanges)
        {
            this.AllowChaosChanges = allowChaosChanges;
        }

        public void SetAllowOwnershipVisibility(bool allowOwnershipVisibility)
        {
            this.AllowOwnershipVisibility = allowOwnershipVisibility;
        }

        public void SetPvPLockChanges(bool SetPvPLock)
        {
            this.PvPLock = SetPvPLock;
        }
        public void SetOnlineToggleChanges(bool SetOnlineToggle)
        {
            this.OnlineToggle = SetOnlineToggle;
        }

        public void SetLockoutEndDateUtc(DateTime dateTime)
        {
            LockoutEndDateUtc = dateTime;
        }

        public void SetAccountLockoutMessage(String message)
        {
            AccountLockoutMessage = message;
        }

        public void SetPvPLockoutMessage(String message)
        {
            PvPLockoutMessage = message;
        }

        public UserDetail MapToDto()
        {
            return new UserDetail
            {
                Id = Id,
                UserName = UserName,
                Email = Email,
                AllowChaosChanges = AllowChaosChanges,
            };
        }

        public UserDonatorDetail MapToDonatorDto()
        {
            return new UserDonatorDetail
            {
                Id = Id,
                UserName = UserName,
                Donator = Donator?.MapToDto(false)
            };
        }
    }

    public class UserSecurityStamp : Entity<string>
    {
        public string SecurityStamp { get; private set; }

        public User User { get; private set; }

        private UserSecurityStamp() { }

        public void ResetSecurityStamp(ResetSecurityStamp cmd)
        {
            SecurityStamp = Guid.NewGuid().ToString();
        }
    }
}
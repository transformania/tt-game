﻿using System;
using System.Collections.Generic;

namespace TT.Domain.Items.DTOs
{
    public class PlayPageItemDetail
    {
        public int Id { get; set; }
        public PlayPageItemSourceDetail ItemSource { get; set; }
        public PlayPagePlayerDetail FormerPlayer { get; set; }
        public string dbLocationName { get; set; }
        public string VictimName { get; set; }
        public int Level { get; set; }
        public int PvPEnabled { get; set; }
        public string Nickname { get; set; }
        public ICollection<PlayPageItemRuneDetail> Runes { get; set; }

        public string GetFullName()
        {
            if (this.Nickname.IsNullOrEmpty())
            {
                return VictimName;
            }
            else
            {
                string[] nameArray = this.VictimName.Split(' ');
                return nameArray[0] + " '" + this.Nickname + "' " + nameArray[1];
            }
        }

        public class PlayPagePlayerDetail
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Nickname { get; set; }
            public string Gender { get; set; }
            public int DonatorLevel { get; set; }

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

        public class PlayPageItemSourceDetail
        {
            public string FriendlyName { get; set; }
            public string PortraitUrl { get; set; }
            public string ItemType { get; set; }
        }

        public class PlayPageItemRuneDetail
        {
            public PlayPageItemSourceDetail ItemSource { get; set; }
        }

    }
}
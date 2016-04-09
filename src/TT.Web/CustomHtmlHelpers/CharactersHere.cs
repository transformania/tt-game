using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Queries.Statics;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.CustomHtmlHelpers
{
    public static class CharactersHere
    {

        public static MvcHtmlString PrintPlayerHTML(this HtmlHelper html, Player player)
        {
            string output = "<div class='name'>" + player.FirstName + " " + player.LastName + "</div>";
            
            return new MvcHtmlString(output);
        }


        public static MvcHtmlString PrintMCIcon(Player player)
        {
            string output = "";

            if (player.MindControlIsActive)
            {
                output = "<span class='icon icon-mc'></span>";
            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintGenderIcon(Player player)
        {
            string output = "<span class=";

            if (player.Gender == "male")
            {
                output += "'icon icon-male'>";
            } else {
                output += "'icon icon-female'>";
            }

            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintGenderIcon(string gender)
        {
            string output = "<span class=";

            if (gender == "male")
            {
                output += "'icon icon-male'>";
            }
            else
            {
                output += "'icon icon-female'>";
            }

            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintPvPIcon(Player player)
        {

            if (player.BotId < AIStatics.ActivePlayerBotId)
            {
                return new MvcHtmlString("");
            }

            if (player.GameMode == 0)
            {
                return new MvcHtmlString("<span class='icon icon-superprotection' title='This player is in SuperProtection mode.'></span>");
            }
            if (player.GameMode == 1)
            {
                return new MvcHtmlString("<span class='icon icon-protection' title='This player is in Protection mode.'></span>");
            }
            else if (player.BotId == AIStatics.ActivePlayerBotId)
            {
                return new MvcHtmlString("<span class='icon icon-pvp' title='This player is in PvP mode.'></span>");
            }

            return new MvcHtmlString("");

        }

        public static MvcHtmlString PrintDonatorIcon(Player player)
        {
            if (player.DonatorLevel >= 1)
            {
                return new MvcHtmlString("<span class='icon icon-donate' title='This player supports this game on Patreon monthly.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintPvPIcon(Item item)
        {

            if (item.PvPEnabled == 2)
            {
                return new MvcHtmlString("<span class='icon icon-pvp' title='This item is in PvP mode.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintPvPIcon(Item_VM item)
        {

            if (item.PvPEnabled == 2)
            {
                return new MvcHtmlString("<span class='icon icon-pvp' title='This item is in PvP mode.'></span>");
            }
            else if (item.PvPEnabled == 1)
            {
                return new MvcHtmlString("<span class='icon icon-protection' title='This item is in Protection mode.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintPvPIcon(Covenant covenant)
        {

            if (covenant.IsPvP)
            {
                return new MvcHtmlString("<span class='icon icon-pvp' title='This covenant is in PvP mode.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintRPIcon(Player player)
        {

            if (player.InRP)
            {
                return new MvcHtmlString("<span class='icon icon-rp' title='This player has indicated that they enjoy roleplaying.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintItemTypeIcon(DbStaticItem item)
        {
            string output = "<span class=";
            output += "'icon icon-" + item.ItemType + "' title='" + item.ItemType + "'>";
            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintItemTypeIcon(StaticItem item)
        {
            string output = "<span class=";
            output += "'icon icon-" + item.ItemType + "' title='" + item.ItemType + "'>";
            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString TruncateToLength(string text, int maxLength)
        {
            if (text.Length <= maxLength)
            {
                return new MvcHtmlString(text);
            }
            else
            {
                int actualLength = text.Length;
                string output = text.Substring(0, maxLength);

                return new MvcHtmlString(output + "...");
            }
        }



      //  public static MvcHtmlString

        public static MvcHtmlString DatetimeToTimeago(DateTime then)
        {

            string output = "";

         

            double minutesAgo = Math.Abs(Math.Floor(then.Subtract(DateTime.UtcNow).TotalMinutes));

            double hoursAgo = Math.Floor(minutesAgo / 60); ;
            minutesAgo = minutesAgo % 60;

            if (hoursAgo == 0 && minutesAgo == 0)
            {
                output += "moments ago";
            }

            else
            {
                if (hoursAgo == 1)
                {
                    output += hoursAgo + " hour ";
                }
                if (hoursAgo > 1)
                {
                    output += hoursAgo + " hours ";
                }

                output += minutesAgo + " minutes ago";

            }


            return new MvcHtmlString(output);
        }

        public static MvcHtmlString DatetimeToTimeago_WithSeconds(DateTime then)
        {

            string output = "";



            double secondsAgo = Math.Abs(Math.Floor(then.Subtract(DateTime.UtcNow).TotalSeconds));

            double minutesAgo = Math.Floor(secondsAgo / 60);

            double hoursAgo = Math.Floor(minutesAgo / 60);
            minutesAgo = minutesAgo % 60;
            secondsAgo = secondsAgo % 60;

            //if (hoursAgo == 0 && minutesAgo == 0)
            //{
            //    output += "moments ago";
            //}

            //else
           // {
                if (hoursAgo == 1)
                {
                    output += hoursAgo + " hour ";
                }
                if (hoursAgo > 1)
                {
                    output += hoursAgo + " hours ";
                }

                output += minutesAgo + " minutes and " + secondsAgo + " seconds ago";

         //   }


            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetBuffedStat(ItemViewModel item, decimal amount)
        {
            string output = "";

            if (item.dbItem.Level > 1 && item.Item.ItemType != "consumable")
            {
                output = "<b>" + amount + "</b><span style='color:  blue;'>  <b>(" + (((item.dbItem.Level-1) * PvPStatics.Item_LevelBonusModifier*amount) + amount) + ")</b></span>";
            } else {
                output = "<b>" + amount + "</b>";
            }

       
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetBuffedStat(ItemViewModel item, float amount)
        {
            string output = "";

            if (item.dbItem.Level > 1 && item.Item.ItemType != "consumable")
            {
                output = "<b>" + amount + "</b><span style='color:  blue;'>  <b>(" + (((item.dbItem.Level - 1) * (float)PvPStatics.Item_LevelBonusModifier * amount) + amount) + ")</b></span>";
            }
            else
            {
                output = "<b>" + amount + "</b>";
            }


            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetImageURL(PlayerFormViewModel player, bool thumb = false)
        {

            ///Images/PvP/itemsPortraits/@Model.Item.PortraitUrl
            ///

            string output = "";
            string strThumb = "";
            string strPortraitUrl = "";

            if (player.Player.Mobility == "full")
            {
                output = "/Images/PvP/portraits/";
                strPortraitUrl = player.Form.PortraitUrl;
            }
            else
            {
                strPortraitUrl = new GetStaticItem { DbName = player.Form.BecomesItemDbName }.Find().PortraitUrl;
                if (player.Player.Mobility == "animal")
                {
                    output = "/Images/PvP/animalPortraits/";
                }
                if (player.Player.Mobility == "inanimate")
                {
                    output = "/Images/PvP/itemsPortraits/";
                }

            }

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + output + strThumb + strPortraitUrl)) strThumb = "";
            }

            output += strThumb + strPortraitUrl;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetImageURL(ItemViewModel item, bool thumb = false)
        {
            string output = "";
            string strThumb = "";
            string strItemType;

            if (item.Item.ItemType == PvPStatics.ItemType_Pet)
            {
                strItemType = "animalPortraits/";
            }
            else
            {
                strItemType = "itemsPortraits/";
            }

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/" + strItemType + strThumb + item.Item.PortraitUrl)) strThumb = "";
            }

            output = "/Images/PvP/" + strItemType + strThumb + item.Item.PortraitUrl;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetImageURL(DbStaticItem item, bool thumb = false)
        {
            string output = "";
            string strThumb = "";
            string strItemType;

            if (item.ItemType == PvPStatics.ItemType_Pet)
            {
                strItemType = "animalPortraits/";
            }
            else
            {
                strItemType = "itemsPortraits/";
            }

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/" + strItemType + strThumb + item.PortraitUrl)) strThumb = "";
            }

            output = "/Images/PvP/" + strItemType + strThumb + item.PortraitUrl;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString StyleIfBot(Player player)
        {
            if (player.BotId <= AIStatics.PsychopathBotId)
            {
                return new MvcHtmlString("bot");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString GetEffectFriendlyName(string effect)
        {
            string friendlyName = EffectStatics.GetStaticEffect2(effect).FriendlyName;
            return new MvcHtmlString(friendlyName);
        }

        public static MvcHtmlString GetSpellTypeClass(SkillViewModel2 skill)
        {

            if (skill.MobilityType == "full")
            {
                return new MvcHtmlString("action_attack full");
            }
            else if (skill.MobilityType == "inanimate")
            {
                return new MvcHtmlString("action_attack inanimate");
            }
            else if (skill.MobilityType == "animal")
            {
                return new MvcHtmlString("action_attack animal");
            }
            else if (skill.MobilityType == "weaken")
            {
                return new MvcHtmlString("action_attack weaken");
            }
            else if (skill.MobilityType == "curse")
            {
                return new MvcHtmlString("action_attack curse");
            }
            else if (skill.MobilityType == "mindcontrol")
            {
                return new MvcHtmlString("action_attack mindcontrol");
            }
            else 

            return new MvcHtmlString("");
        }

        public static MvcHtmlString PrintOwnerSubPortrait(PlayerFormViewModel owner)
        {

            // <div class='subportrait' style='background-image: url(../Images/PvP/portraits/@Model.WornBy.Form.PortraitUrl); '></div>

            string output = "";

            if (owner == null)
            {
                return new MvcHtmlString(output);
            }
            else
            {
                string strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/portraits/" + strThumb + owner.Form.PortraitUrl)) strThumb = "";
                output = "<div class='subportrait' style='background-image: url(../Images/PvP/portraits/" + strThumb + owner.Form.PortraitUrl + ");' title = 'You are owned by " + owner.Player.FirstName + " " + owner.Player.LastName + ", a " + owner.Form.FriendlyName + ".'></div>";
                return new MvcHtmlString(output);
            }
        }

        public static MvcHtmlString GetCovenantInfo(Player player)
        {
            if (player.Covenant <= 0)
            {
                return new MvcHtmlString("");
            }
            else
            {

                if (!CovenantDictionary.IdNameFlagLookup.Any())
                {
                    CovenantProcedures.LoadCovenantDictionary();
                }

                CovenantNameFlag temp = CovenantDictionary.IdNameFlagLookup.FirstOrDefault(c => c.Key == player.Covenant).Value;
                string output = "<span class='covRptName'>Member of <b>" + temp.Name + "</b></span>";

                return new MvcHtmlString(output);
            }
        }

        public static MvcHtmlString GetCovenantInfoShort(Player player)
        {
            if (player.Covenant <= 0)
            {
                return new MvcHtmlString("");
            }
            else
            {

                if (!CovenantDictionary.IdNameFlagLookup.Any())
                {
                    CovenantProcedures.LoadCovenantDictionary();
                }

                CovenantNameFlag temp = CovenantDictionary.IdNameFlagLookup.FirstOrDefault(c => c.Key == player.Covenant).Value;
                string output = "<span class='covRptName'><b>" + temp.Name + "</b></span>";

                return new MvcHtmlString(output);
            }
        }

        public static MvcHtmlString PrintPermanencyIcon(ItemViewModel item)
        {
            if (item.dbItem.IsPermanent)
            {
                return new MvcHtmlString("<span class='icon icon-permanent' title=\"This player\'s soul is permanently sealed into this item.\"></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintPermanencyIcon(Item item)
        {
            if (item.IsPermanent)
            {
                return new MvcHtmlString("<span class='icon icon-permanent' title=\"This player\'s soul is permanently sealed into this item.\"></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintSouledIcon(Item item)
        {
            if (!item.VictimName.IsNullOrEmpty())
            {
                double timeAgo = Math.Abs(Math.Floor(item.LastSouledTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

                if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[0])
                {
                    return new MvcHtmlString("<span class='icon icon-souled0'></span>");
                }
                else if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[1])
                {
                    return new MvcHtmlString("<span class='icon icon-souled1'></span>");
                }
                else if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[2])
                {
                    return new MvcHtmlString("<span class='icon icon-souled2'></span>");
                }
                else
                {
                    return new MvcHtmlString("");
                }
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintSouledIcon(ItemViewModel item)
        {
            if (!item.dbItem.VictimName.IsNullOrEmpty())
            {
                double timeAgo = Math.Abs(Math.Floor(item.dbItem.LastSouledTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

                if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[0])
                {
                    return new MvcHtmlString("<span class='icon icon-souled0'></span>");
                }
                else if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[1])
                {
                    return new MvcHtmlString("<span class='icon icon-souled1'></span>");
                }
                else if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[2])
                {
                    return new MvcHtmlString("<span class='icon icon-souled2'></span>");
                }
                else
                {
                    return new MvcHtmlString("");
                }
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString StringToWebsite(string url)
        {
            if (url.IsNullOrEmpty())
            {
                return new MvcHtmlString("");
            }

            string prettyurl = url;

            if (!url.Contains("http://"))
            {
                url = "http://" + url;
            }

            string output = "<a href='" + url + "'>" + prettyurl + "</a>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString WithinBalanceTarget(decimal amount, string type)
        {
            if (type == "Forms")
            {
                if (amount > 10 || amount < -10)
                {
                    return new MvcHtmlString("<span class='bad'>" + amount + "</span>");
                }
                else
                {
                    return new MvcHtmlString("<span class='good'>" + amount + "</span>");
                }
            }
            else if (type == "Items")
            {
                if (amount > 35 || amount < 15)
                {
                    return new MvcHtmlString("<span class='bad'>" + amount + "</span>");
                }
                else
                {
                    return new MvcHtmlString("<span class='good'>" + amount + "</span>");
                }
            }
            if (type == "Pets")
            {
                if (amount > 60 || amount < 40)
                {
                    return new MvcHtmlString("<span class='bad'>" + amount + "</span>");
                }
                else
                {
                    return new MvcHtmlString("<span class='good'>" + amount + "</span>");
                }
            }
            else {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintFurnitureAvailability(FurnitureViewModel furniture)
        {
            string output = "";
            double minutesUntilReuse = TT.Domain.Procedures.FurnitureProcedures.GetMinutesUntilReuse(furniture);

            if (minutesUntilReuse <= 0)
            {
                output = "<span class='good'>Available for use!</span>";
            }
            else
            {
                output = "<span class = 'bad'>Available in " + (int)Math.Ceiling(minutesUntilReuse) + " minutes.</span>";
            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintCovenantSafeground(Covenant cov)
        {
            string output = "";

            if (!cov.HomeLocation.IsNullOrEmpty())
            {
                string locName = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == cov.HomeLocation).Name;
                output = "<span class = 'covMemberCount'>Safeground at <b>" + locName + "</b>.</span>";
            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintCovenantColorCode(LocationInfo info)
        {
            string output = "red";

            string hexValue = info.CovenantId.ToString("X");

            Random rand = new Random();

            // Convert the hex string back to the number
           // int decAgain = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintStatDescriptionPopup(string statName)
        {
            string output = BuffMap.BuffDetailsMap[statName].DisplayName + " <span class='statPopupBubble' onclick='alert(\"";
            output += BuffMap.BuffDetailsMap[statName].Description;
            output += "\");'><b style='cursor: pointer'>[?]</b></span>";
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintStatIcons(string statName)
        {
            string output = "<span class='goodEffectGroup'>";
            List<string> iconsToPrint = BuffMap.BuffDetailsMap[statName].PlusIcons;
            foreach (string s in iconsToPrint)
            {
                output += "<span class='" + s + "'></span>";
            }
            output += "</span>";


            output += "<span class='badEffectGroup'>";
            iconsToPrint = BuffMap.BuffDetailsMap[statName].MinusIcons;
            foreach (string s in iconsToPrint)
            {
                output += "<span class='" + s + "'></span>";
            }
            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintDiceIcon(QuestConnection connection)
        {
            string output = "";

            if (connection.RequiresRolls())
            {
                output = "<img src='../Images/PvP/Icons/dice.png' style='width: 24px; height: 24px; '>";
            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintDuelIcon(Player player)
        {
            if (player.InDuel > 0)
            {
                return new MvcHtmlString("<span class='icon icon-duel' title='This player is actively in a duel.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString PrintQuestIcon(Player player)
        {
            if (player.InQuest > 0)
            {
                return new MvcHtmlString("<span class='icon icon-quest' title='This player is in a quest.'></span>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }


        //public static MvcHtmlString PrintCovenantColorCode(LocationInfo info)
        //{
        //    string output = "red";

        //    string hexValue = info.CovenantId.ToString("X");

        //    Random rand = new Random();

        //    // Convert the hex string back to the number
        //    // int decAgain = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);

        //    return new MvcHtmlString(output);
        //}

    }
}
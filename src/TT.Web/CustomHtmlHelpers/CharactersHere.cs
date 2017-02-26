using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TT.Domain.Items.DTOs;
using TT.Domain.Messages.DTOs;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain.World.Queries;

namespace TT.Web.CustomHtmlHelpers
{
    public static class CharactersHere
    {

        public static MvcHtmlString PrintPlayerHTML(this HtmlHelper html, Player player)
        {
            var output = "<div class='name'>" + player.FirstName + " " + player.LastName + "</div>";
            
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintMCIcon(Player player)
        {
            var output = "";

            if (player.MindControlIsActive)
            {
                output = "<span class='icon icon-mc'></span>";
            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintGenderIcon(string gender)
        {
            var output = "<span class=";

            if (gender == PvPStatics.GenderMale)
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

            if (player.GameMode == GameModeStatics.SuperProtection)
            {
                return new MvcHtmlString("<span class='icon icon-superprotection' title='This player is in SuperProtection mode.'></span>");
            }
            if (player.GameMode == GameModeStatics.Protection)
            {
                return new MvcHtmlString("<span class='icon icon-protection' title='This player is in Protection mode.'></span>");
            }
            return player.BotId == AIStatics.ActivePlayerBotId ? new MvcHtmlString("<span class='icon icon-pvp' title='This player is in PvP mode.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintDonatorIcon(Player player)
        {
            return player.DonatorLevel >= 1 ? new MvcHtmlString("<span class='icon icon-donate' title='This player supports this game on Patreon monthly.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintPvPIcon(Item_VM item)
        {

            if (item.PvPEnabled == 2)
            {
                return new MvcHtmlString("<span class='icon icon-pvp' title='This item is in PvP mode.'></span>");
            }
            return item.PvPEnabled == 1 ? new MvcHtmlString("<span class='icon icon-protection' title='This item is in Protection mode.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintPvPIcon(ItemDetail item)
        {

            if (item.PvPEnabled == 2)
            {
                return new MvcHtmlString("<span class='icon icon-pvp' title='This item is in PvP mode.'></span>");
            }
            return item.PvPEnabled == 1 ? new MvcHtmlString("<span class='icon icon-protection' title='This item is in Protection mode.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintPvPIcon(Covenant covenant)
        {
            return covenant.IsPvP ? new MvcHtmlString("<span class='icon icon-pvp' title='This covenant is in PvP mode.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintRPIcon(Player player)
        {
            return player.InRP ? new MvcHtmlString("<span class='icon icon-rp' title='This player has indicated that they enjoy roleplaying.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintItemTypeIcon(string itemType)
        {
            var output = "<span class=";
            output += "'icon icon-" + itemType + "' title='" + itemType + "'>";
            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintItemTypeIcon(DbStaticItem item)
        {
            return PrintItemTypeIcon(item.ItemType);
        }

        public static MvcHtmlString PrintItemTypeIcon(ItemListingDetail item)
        {
            return PrintItemTypeIcon(item.ItemSource.ItemType);
        }

        public static MvcHtmlString PrintItemTypeIcon(ItemSourceDetail item)
        {
            return PrintItemTypeIcon(item.ItemType);
        }

        public static MvcHtmlString PrintItemTypeIcon(StaticItem item)
        {
            return PrintItemTypeIcon(item.ItemType);
        }

        public static MvcHtmlString TruncateToLength(string text, int maxLength)
        {
            if (text.Length <= maxLength)
            {
                return new MvcHtmlString(text);
            }
            var output = text.Substring(0, maxLength);

            return new MvcHtmlString(output + "...");
        }


        public static MvcHtmlString DatetimeToTimeago(DateTime then)
        {

            var output = "";

            var minutesAgo = Math.Abs(Math.Floor(then.Subtract(DateTime.UtcNow).TotalMinutes));

            var hoursAgo = (int)Math.Floor(minutesAgo / 60);
            minutesAgo = minutesAgo % 60;

            if (hoursAgo == 0 && (int)minutesAgo == 0)
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

                output += (int)minutesAgo + " minutes ago";

            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString DatetimeToTimeago_WithSeconds(DateTime then)
        {

            var output = "";

            var secondsAgo = Math.Abs(Math.Floor(then.Subtract(DateTime.UtcNow).TotalSeconds));

            var minutesAgo = Math.Floor(secondsAgo / 60);

            var hoursAgo = (int)Math.Floor(minutesAgo / 60);
            minutesAgo = minutesAgo % 60;
            secondsAgo = secondsAgo % 60;

            if (hoursAgo == 1)
            {
                output += hoursAgo + " hour ";
            }
            if (hoursAgo > 1)
            {
                output += hoursAgo + " hours ";
            }

            output += (int)minutesAgo + " minutes and " + (int)secondsAgo + " seconds ago";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetBuffedStat(ItemViewModel item, decimal amount)
        {
            string output;

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
            string output;

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
            var output = "";
            var strThumb = "";
            string strPortraitUrl;

            if (player.Player.Mobility == PvPStatics.MobilityFull)
            {
                output = "/Images/PvP/portraits/";
                strPortraitUrl = player.Form.PortraitUrl;
            }
            else
            {
                strPortraitUrl = new GetStaticItem { DbName = player.Form.BecomesItemDbName }.Find().PortraitUrl;
                if (player.Player.Mobility == PvPStatics.MobilityPet)
                {
                    output = "/Images/PvP/animalPortraits/";
                }
                if (player.Player.Mobility == PvPStatics.MobilityInanimate)
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
            var strThumb = "";

            var strItemType = item.Item.ItemType == PvPStatics.ItemType_Pet ? "animalPortraits/" : "itemsPortraits/";

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/" + strItemType + strThumb + item.Item.PortraitUrl)) strThumb = "";
            }

            var output = "/Images/PvP/" + strItemType + strThumb + item.Item.PortraitUrl;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetFormImageURL(string imageName, bool thumb = false)
        {
            var strThumb = "";

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/portraits/"  + strThumb + imageName)) strThumb = "";
            }

            var output = "/Images/PvP/portraits/" + strThumb + imageName;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetImageUrl(string imageName, string mobility, bool thumb = false)
        {
            var strItemType = "";
            var strThumb = "";

            if (mobility == PvPStatics.MobilityFull)
                strItemType = "portraits/";
            else if (mobility == PvPStatics.MobilityInanimate)
                strItemType = "itemsPortraits/";
            else if (mobility == PvPStatics.MobilityPet)
                strItemType = "animalPortraits/";

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/" + strItemType + strThumb + imageName)) strThumb = "";
            }

            var output = "/Images/PvP/" + strItemType + strThumb + imageName;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetImageURL(ItemListingDetail item, bool thumb = false)
        {
            var strThumb = "";

            var strItemType = item.ItemSource.ItemType == PvPStatics.ItemType_Pet ? "animalPortraits/" : "itemsPortraits/";

            if (thumb)
            {
                strThumb = "Thumbnails/100/";
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/" + strItemType + strThumb + item.ItemSource.PortraitUrl)) strThumb = "";
            }

            var output = "/Images/PvP/" + strItemType + strThumb + item.ItemSource.PortraitUrl;
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString StyleIfBot(Player player)
        {
            return player.BotId <= AIStatics.PsychopathBotId ? new MvcHtmlString("bot") : new MvcHtmlString("");
        }

        public static MvcHtmlString GetEffectFriendlyName(string effect)
        {
            return new MvcHtmlString(EffectStatics.GetStaticEffect2(effect).FriendlyName);
        }

        public static MvcHtmlString GetSpellTypeClass(SkillViewModel skill)
        {

            if (skill.MobilityType == PvPStatics.MobilityFull)
            {
                return new MvcHtmlString("action_attack full");
            }
            if (skill.MobilityType == PvPStatics.MobilityInanimate)
            {
                return new MvcHtmlString("action_attack inanimate");
            }
            if (skill.MobilityType == PvPStatics.MobilityPet)
            {
                return new MvcHtmlString("action_attack animal");
            }
            if (skill.MobilityType == "weaken")
            {
                return new MvcHtmlString("action_attack weaken");
            }
            if (skill.MobilityType == "curse")
            {
                return new MvcHtmlString("action_attack curse");
            }
            return skill.MobilityType == PvPStatics.MobilityMindControl ? new MvcHtmlString("action_attack mindcontrol") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintOwnerSubPortrait(PlayerFormViewModel owner)
        {
            var output = "";

            if (owner == null)
            {
                return new MvcHtmlString(output);
            }
            var strThumb = "Thumbnails/100/";
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Images/PvP/portraits/" + strThumb + owner.Form.PortraitUrl)) strThumb = "";
            output = "<div class='subportrait' style='background-image: url(../Images/PvP/portraits/" + strThumb + owner.Form.PortraitUrl + ");' title = 'You are owned by " + owner.Player.GetFullName() + ", a " + owner.Form.FriendlyName + ".'></div>";
            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetCovenantInfo(Player player)
        {
            if (player.Covenant <= 0)
            {
                return new MvcHtmlString("");
            }
            if (!CovenantDictionary.IdNameFlagLookup.Any())
            {
                CovenantProcedures.LoadCovenantDictionary();
            }

            var temp = CovenantDictionary.IdNameFlagLookup.FirstOrDefault(c => c.Key == player.Covenant).Value;
            var output = "<span class='covRptName'>Member of <b>" + temp.Name + "</b></span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString GetCovenantInfoShort(Player player)
        {
            if (player.Covenant <= 0)
            {
                return new MvcHtmlString("");
            }
            if (!CovenantDictionary.IdNameFlagLookup.Any())
            {
                CovenantProcedures.LoadCovenantDictionary();
            }

            var temp = CovenantDictionary.IdNameFlagLookup.FirstOrDefault(c => c.Key == player.Covenant).Value;
            var output = "<span class='covRptName'><b>" + temp.Name + "</b></span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintPermanencyIcon(bool isPermanent)
        {
            return isPermanent ? new MvcHtmlString("<span class='icon icon-permanent' title=\"This player\'s soul is permanently sealed into this item.\"></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintPermanencyIcon(Item item)
        {
            return item.IsPermanent ? new MvcHtmlString("<span class='icon icon-permanent' title=\"This player\'s soul is permanently sealed into this item.\"></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintSouledIcon(ItemViewModel item)
        {
            if (item.dbItem.VictimName.IsNullOrEmpty()) return new MvcHtmlString("");
            var timeAgo = Math.Abs(Math.Floor(item.dbItem.LastSouledTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[0])
            {
                return new MvcHtmlString("<span class='icon icon-souled0'></span>");
            }
            if (timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[1])
            {
                return new MvcHtmlString("<span class='icon icon-souled1'></span>");
            }
            return timeAgo < PvPStatics.Item_SoulActivityLevels_Minutes[2] ? new MvcHtmlString("<span class='icon icon-souled2'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString WithinBalanceTarget(decimal amount, string type)
        {
            if (type == "Forms")
            {
                if (amount > 10 || amount < -10)
                {
                    return new MvcHtmlString("<span class='bad'>" + amount + "</span>");
                }
                return new MvcHtmlString("<span class='good'>" + amount + "</span>");
            }
            if (type == "Items")
            {
                if (amount > 35 || amount < 15)
                {
                    return new MvcHtmlString("<span class='bad'>" + amount + "</span>");
                }
                return new MvcHtmlString("<span class='good'>" + amount + "</span>");
            }
            if (type == "Pets")
            {
                if (amount > 60 || amount < 40)
                {
                    return new MvcHtmlString("<span class='bad'>" + amount + "</span>");
                }
                return new MvcHtmlString("<span class='good'>" + amount + "</span>");
            }
            return new MvcHtmlString("");
        }

        public static MvcHtmlString PrintFurnitureAvailability(FurnitureViewModel furniture)
        {
            string output;
            var minutesUntilReuse = FurnitureProcedures.GetMinutesUntilReuse(furniture);

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
            var output = "";

            if (cov.HomeLocation.IsNullOrEmpty()) return new MvcHtmlString(output);
            var loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == cov.HomeLocation);
            if (loc == null) return new MvcHtmlString(output);
            var locName = loc.Name;
            output = "<span class = 'covMemberCount'>Safeground at <b>" + locName + "</b>.</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintCovenantColorCode(LocationInfo info)
        {
            return new MvcHtmlString("red");
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
            var iconsToPrint = BuffMap.BuffDetailsMap[statName].PlusIcons;
            var output = iconsToPrint.Aggregate("<span class='goodEffectGroup'>", (current, s) => current + ("<span class='" + s + "'></span>"));
            output += "</span>";


            output += "<span class='badEffectGroup'>";
            iconsToPrint = BuffMap.BuffDetailsMap[statName].MinusIcons;
            output = iconsToPrint.Aggregate(output, (current, s) => current + ("<span class='" + s + "'></span>"));
            output += "</span>";

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintDiceIcon(QuestConnection connection)
        {
            var output = "";

            if (connection.RequiresRolls())
            {
                output = "<img src='../Images/PvP/Icons/dice.png' style='width: 24px; height: 24px; '>";
            }

            return new MvcHtmlString(output);
        }

        public static MvcHtmlString PrintDuelIcon(Player player)
        {
            return player.InDuel > 0 ? new MvcHtmlString("<span class='icon icon-duel' title='This player is actively in a duel.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintQuestIcon(Player player)
        {
            return player.InQuest > 0 ? new MvcHtmlString("<span class='icon icon-quest' title='This player is in a quest.'></span>") : new MvcHtmlString("");
        }

        public static MvcHtmlString PrintMessageReadStatus(MessageDetail msg)
        {
            if (msg.ReadStatus == MessageStatics.Read)
            {
                return new MvcHtmlString("Read");
            }
            else
            {
                return new MvcHtmlString("Unread");
            }

        }
    }
}
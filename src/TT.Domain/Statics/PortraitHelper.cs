using System;
using System.IO;

namespace TT.Domain.Statics
{
    public static class PortraitHelper
    {
        public static string GetGraphic(string mobilityType, string portaitUrl, bool useThumb = false)
        {
            var strItemType = "";

            if (mobilityType == PvPStatics.MobilityFull)
            {
                strItemType = "portraits/";
            }

            else if (mobilityType == PvPStatics.MobilityInanimate)
            {
                strItemType = "itemsPortraits/";
            }

            else if (mobilityType == PvPStatics.MobilityPet)
            {
                strItemType = "animalPortraits/";
            }

            var strThumb = useThumb ? "Thumbnails/100/" : "";
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{PvPStatics.ImageFolder}{strItemType}{strThumb}{portaitUrl}")) strThumb = "";

            return $"{PvPStatics.ImageURL}{strItemType}{strThumb}{portaitUrl}";
        }

        public static string GetGraphicByItemType(string itemType, string portraitUrl, bool useThumb = false)
        {
            var mobility = itemType == PvPStatics.ItemType_Pet ? PvPStatics.MobilityPet : PvPStatics.MobilityInanimate;
            return GetGraphic(mobility, portraitUrl, useThumb);
        }

    }
}

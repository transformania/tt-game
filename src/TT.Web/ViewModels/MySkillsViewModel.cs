using System;
using System.Collections.Generic;
using System.IO;
using TT.Domain.Skills.DTOs;
using TT.Domain.Statics;

namespace TT.Web.ViewModels
{
    public class MySkillsViewModel
    {

        public IEnumerable<SkillSourceFormSourceDetail> skills { get; set; }

        public MySkillsViewModel(IEnumerable<SkillSourceFormSourceDetail> input)
        {
            skills = input;
        }

        public bool ShouldDisplayGraphic(SkillSourceFormSourceDetail skill)
        {
            return skill.SkillSource.MobilityType == PvPStatics.MobilityFull ||
                   skill.SkillSource.MobilityType == PvPStatics.MobilityInanimate ||
                   skill.SkillSource.MobilityType == PvPStatics.MobilityPet;
        }

        public string GetGraphic(SkillSourceFormSourceDetail skill)
        {
            var strItemType = "";
            var imageName = "";

            if (skill.SkillSource.MobilityType == PvPStatics.MobilityFull)
            {
                imageName = skill.SkillSource.FormSource.PortraitUrl;
                strItemType = "portraits/";
            }

            else if (skill.SkillSource.MobilityType == PvPStatics.MobilityInanimate)
            {
                imageName = skill.SkillSource.FormSource.ItemSource.PortraitUrl;
                strItemType = "itemsPortraits/";
            }

            else if (skill.SkillSource.MobilityType == PvPStatics.MobilityPet)
            {
                imageName = skill.SkillSource.FormSource.ItemSource.PortraitUrl;
                strItemType = "animalPortraits/";
            }
            else
            {
                imageName = "";
            }

            var strThumb = "Thumbnails/100/";
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{PvPStatics.ImageFolder}{strItemType}{strThumb}{imageName}")) strThumb = "";

            return $"{PvPStatics.ImageURL}{strItemType}{strThumb}{imageName}";
        }

    }
}
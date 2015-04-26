using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace tfgame.ViewModels
{
    public class Form
    {
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string TFEnergyType { get; set; }
        public decimal TFEnergyRequired { get; set; }
        public string Gender { get; set; }
        public string MobilityType { get; set; }
        public string BecomesItemDbName { get; set; }
        public string PortraitUrl { get; set; }
        public bool IsUnique { get; set; }

        public decimal HealthBonusPercent { get; set; }
        public decimal ManaBonusPercent { get; set; }
        public decimal ExtraSkillCriticalPercent { get; set; }
        public decimal HealthRecoveryPerUpdate { get; set; }
        public decimal ManaRecoveryPerUpdate { get; set; }
        public decimal SneakPercent { get; set; }
        public decimal EvasionPercent { get; set; }
        public decimal EvasionNegationPercent { get; set; }
        public decimal MeditationExtraMana { get; set; }
        public decimal CleanseExtraHealth { get; set; }
        public decimal MoveActionPointDiscount { get; set; }
        public decimal SpellExtraTFEnergyPercent { get; set; }
        public decimal SpellExtraHealthDamagePercent { get; set; }
        public decimal CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal SpellMisfireChanceReduction { get; set; }
        public decimal SpellHealthDamageResistance { get; set; }
        public decimal SpellTFEnergyDamageResistance { get; set; }
        public decimal ExtraInventorySpace { get; set; }


        public float Discipline { get; set; }
        public float Perception { get; set; }
        public float Charisma { get; set; }
        public float Submission_Dominance { get; set; }

        public float Fortitude { get; set; }
        public float Agility { get; set; }
        public float Allure { get; set; }
        public float Corruption_Purity { get; set; }

        public float Magicka { get; set; }
        public float Succour { get; set; }
        public float Luck { get; set; }
        public float Chaos_Order { get; set; }

        public string TFMessage_20_Percent_1st { get; set; }
        public string TFMessage_40_Percent_1st { get; set; }
        public string TFMessage_60_Percent_1st { get; set; }
        public string TFMessage_80_Percent_1st { get; set; }
        public string TFMessage_100_Percent_1st { get; set; }
        public string TFMessage_Completed_1st { get; set; }

        public string TFMessage_20_Percent_1st_M { get; set; }
        public string TFMessage_40_Percent_1st_M { get; set; }
        public string TFMessage_60_Percent_1st_M { get; set; }
        public string TFMessage_80_Percent_1st_M { get; set; }
        public string TFMessage_100_Percent_1st_M { get; set; }
        public string TFMessage_Completed_1st_M { get; set; }

        public string TFMessage_20_Percent_1st_F { get; set; }
        public string TFMessage_40_Percent_1st_F { get; set; }
        public string TFMessage_60_Percent_1st_F { get; set; }
        public string TFMessage_80_Percent_1st_F { get; set; }
        public string TFMessage_100_Percent_1st_F { get; set; }
        public string TFMessage_Completed_1st_F { get; set; }

        public string TFMessage_20_Percent_3rd { get; set; }
        public string TFMessage_40_Percent_3rd { get; set; }
        public string TFMessage_60_Percent_3rd { get; set; }
        public string TFMessage_80_Percent_3rd { get; set; }
        public string TFMessage_100_Percent_3rd { get; set; }
        public string TFMessage_Completed_3rd { get; set; }

        public string TFMessage_20_Percent_3rd_M { get; set; }
        public string TFMessage_40_Percent_3rd_M { get; set; }
        public string TFMessage_60_Percent_3rd_M { get; set; }
        public string TFMessage_80_Percent_3rd_M { get; set; }
        public string TFMessage_100_Percent_3rd_M { get; set; }
        public string TFMessage_Completed_3rd_M { get; set; }

        public string TFMessage_20_Percent_3rd_F { get; set; }
        public string TFMessage_40_Percent_3rd_F { get; set; }
        public string TFMessage_60_Percent_3rd_F { get; set; }
        public string TFMessage_80_Percent_3rd_F { get; set; }
        public string TFMessage_100_Percent_3rd_F { get; set; }
        public string TFMessage_Completed_3rd_F { get; set; }


        public BuffBox FormBuffs { get; set; }

        //public string GetDescription()
        //{
        //    if (this.Description != null && this.Description != "")
        //    {
        //        return this.Description;
        //    }
        //    else
        //    {

        //        try { 

        //        //load up form XML
        //        string path = HttpContext.Current.Server.MapPath("~/XMLs/TFMessages/" + this.dbName + ".xml");

        //        Form xmlForm = null;

        //        var serializer = new XmlSerializer(typeof(Form));
        //        using (var reader = XmlReader.Create(path))
        //        {
        //            xmlForm = (Form)serializer.Deserialize(reader);
        //        }

        //        return xmlForm.Description;

        //        }
        //        catch
        //        {
        //            return "ERROR LOADING DESCRIPTION FOR THIS FORM.  Please report this is a bug.";
        //        }

        //    }
        //}

        // todo... bonuses based on forms later

    }
}
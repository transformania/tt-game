using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace tfgame.ViewModels
{
    public class StaticEffect
    {
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public int AvailableAtLevel { get; set; }
        public string PreRequesite { get; set; }

        public bool isLevelUpPerk { get; set; }
        public int Duration { get; set; }
        public int Cooldown { get; set; }

        public string ObtainedAtLocation { get; set; }

        public string MessageWhenHit { get; set; }
        public string MessageWhenHit_M { get; set; }
        public string MessageWhenHit_F { get; set; }

        public string AttackerWhenHit { get; set; }
        public string AttackerWhenHit_M { get; set; }
        public string AttackerWhenHit_F { get; set; }

        public decimal HealthBonusPercent { get; set; }
        public decimal ManaBonusPercent { get; set; }
        public decimal SpellResistancePercent { get; set; }
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

        public decimal InstantHealthRestore { get; set; }
        public decimal InstantManaRestore { get; set; }

        public string GetMessageWhenHit(string gender)
        {

            if (gender == "male" && this.MessageWhenHit_M != null && this.MessageWhenHit_M != "")
            {
                return this.MessageWhenHit_M;
            }
            else if (gender == "female" && this.MessageWhenHit_F != null && this.MessageWhenHit_F != "")
            {
                return this.MessageWhenHit_F;
            }
            else if (this.MessageWhenHit != null)
            {
                return this.MessageWhenHit;
            }

    
            else
            {
                //load up form XML

                try
                {

                    string path = HttpContext.Current.Server.MapPath("~/XMLs/Effects/" + this.dbName + ".xml");

                    StaticEffect xmlEffect = null;

                    var serializer = new XmlSerializer(typeof(StaticEffect));
                    using (var reader = XmlReader.Create(path))
                    {
                        xmlEffect = (StaticEffect)serializer.Deserialize(reader);
                    }

                    if (gender == "male" && xmlEffect.MessageWhenHit_M != null && xmlEffect.MessageWhenHit_M != "") {
                        return xmlEffect.MessageWhenHit_M;
                    }
                    else if (gender == "female" && xmlEffect.MessageWhenHit_F != null && xmlEffect.MessageWhenHit_F != "")
                    {
                        return xmlEffect.MessageWhenHit_F;
                    }
                    else if (xmlEffect.MessageWhenHit != null)
                    {
                        return xmlEffect.MessageWhenHit;
                    }
                    

                }
                catch
                {
                    return "EFFECT GAIN TEXT NOT FOUND.  This is a bug.";
                }

            }

            return "";
        }

        public string GetAttackerWhenHit(string gender)
        {

            if (gender == "male" && this.AttackerWhenHit_M != null)
            {
                return this.AttackerWhenHit_M;
            }
            else if (gender == "female" && this.AttackerWhenHit_F != null)
            {
                return this.AttackerWhenHit_F;
            }
            else if (this.AttackerWhenHit != null)
            {
                return this.AttackerWhenHit;
            }


            else
            {
                //load up form XML

                try
                {

                    string path = HttpContext.Current.Server.MapPath("~/XMLs/Effects/" + this.dbName + ".xml");

                    StaticEffect xmlEffect = null;

                    var serializer = new XmlSerializer(typeof(StaticEffect));
                    using (var reader = XmlReader.Create(path))
                    {
                        xmlEffect = (StaticEffect)serializer.Deserialize(reader);
                    }

                    if (gender == "male" && xmlEffect.AttackerWhenHit_M != null)
                    {
                        return xmlEffect.AttackerWhenHit_M;
                    }
                    else if (gender == "female" && xmlEffect.AttackerWhenHit_F != null)
                    {
                        return xmlEffect.AttackerWhenHit_F;
                    }
                    else if (xmlEffect.AttackerWhenHit != null)
                    {
                        return xmlEffect.AttackerWhenHit;
                    }


                }
                catch
                {
                    return "EFFECT ATTACK TEXT NOT FOUND.  This is a bug.";
                }

            }

            return "";
        }

    }
}
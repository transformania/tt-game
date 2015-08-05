using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace tfgame.ViewModels
{
    public class StaticSkill
    {
        //this class stores all of the skills as available to everyone.  It has absolutely nothing to do with what an individual player has.

        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string FormdbName { get; set; }
        public string Description { get; set; }
        public decimal ManaCost { get; set; }
        public decimal TFPointsAmount { get; set; }
        public decimal HealthDamageAmount { get; set; }
        public string LearnedAtRegion { get; set; }
        public string LearnedAtLocation { get; set; }
        public string DiscoveryMessage { get; set; }

        public bool IsPlayerLearnable { get; set; }

        public string GivesEffect { get; set; }

        public string ExclusiveToForm { get; set; }
        public string ExclusiveToItem { get; set; }

        public string GetDiscoveryMessage()
        {
            if (this.DiscoveryMessage != null && this.DiscoveryMessage != "")
            {
                return this.DiscoveryMessage;
            } else {
                //load up form XML

                try { 

                string path = HttpContext.Current.Server.MapPath("~/XMLs/SkillMessages/" + this.dbName + ".xml");

                StaticSkill xmlSkill = null;

                var serializer = new XmlSerializer(typeof(StaticSkill));
                using (var reader = XmlReader.Create(path))
                {
                    xmlSkill = (StaticSkill)serializer.Deserialize(reader);
                }

                return xmlSkill.DiscoveryMessage;

                }
                catch
                {
                    return "DISCOVERY TEXT NOT FOUND.  This is a bug.";
                }

            }
        }

        public string GetXMLExportName(string submitterName)
        {
            return this.dbName = "skill_" + this.FriendlyName.Replace(" ", "_") + "_" + submitterName.Replace(" ", "_");
        }

    }
}
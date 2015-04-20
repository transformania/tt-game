using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;
using tfgame.Procedures.BossProcedures;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;

namespace tfgame.Controllers
{
    [InitializeSimpleMembership]
    public class PvPAdminController : Controller
    {
        //
        // GET: /PvPAdmin/
        public ActionResult PvPAdmin()
        {

            #region location checks

            string output = "<h1>Location Errors: </h1><br>";
            List<Location> places = LocationsStatics.LocationList.GetLocation.ToList();

            foreach (Location place in places)
            {
                if (place.Name_North != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_North);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + " (" + place.Name + ")</b> is referencing location <b>" + place.Name_North + "</b> which is null.<br>";
                    }
                    else
                    {
                        if (x.Name_South != place.dbName)
                        {
                            output += "North:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
                if (place.Name_East != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_East);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + "(" + place.Name + ")</b> is referencing location <b>" + place.Name_East + "</b> which is null.";
                    }
                    else
                    {
                        if (x.Name_West != place.dbName)
                        {
                            output += "East:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
                if (place.Name_West != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_West);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + "(" + place.Name + ")</b> is referencing location <b>" + place.Name_West + "</b> which is null.";
                    }
                    else
                    {
                        if (x.Name_East != place.dbName)
                        {
                            output += "West:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
                if (place.Name_South != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_South);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + "(" + place.Name + ")</b> is referencing location <b>" + place.Name_South + "</b> which is null.";
                    }
                    else
                    {
                        if (x.Name_North != place.dbName)
                        {
                            output += "South:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
            }

            #endregion

            //#region check skills to forms

            //output += "<h1>Skill to Form Errors</h1><br>";
            //List<DbStaticSkill> skills = SkillStatics.GetAllStaticSkills().ToList();

            //foreach (DbStaticSkill skill in skills)
            //{

            //    // if this skill is learned in a region, make sure a region of that names does exist
            //    if (skill.LearnedAtRegion != null && skill.LearnedAtRegion != "")
            //    {
            //        if (LocationsStatics.LocationList.GetLocation.Where(l => l.Region == skill.LearnedAtRegion).Count() == 0)
            //        {
            //            output += "Skill <b> " + skill.dbName + " is said to be found at region <b>" + skill.LearnedAtRegion + "</b>, but that region does not exist.<br>";
            //        }
            //    }

            //    Form form = FormStatics.GetForm.FirstOrDefault(f => f.dbName == skill.FormdbName);

            //    if (form == null && skill.FormdbName != "none" && skill.ExclusiveToForm == null)
            //    {
            //        output += "Skill <b>" + skill.dbName + "</b> refers to form <b>" + skill.FormdbName + "</b> but that form does not exist!<br><br>";
            //    }


            //    if (skill.ExclusiveToForm != null)
            //    {
            //        Form exclusiveToform = FormStatics.GetForm.FirstOrDefault(f => f.dbName == skill.ExclusiveToForm);
            //        if (exclusiveToform == null)
            //        {
            //            output += "Curse <b>" + skill.dbName + "</b> is exclusive to form <b>" + skill.ExclusiveToForm + "</b> but that form does not exist!<br><br>";
            //        }

            //    }

            //}

            //#endregion

            //#region check forms to items

            //output += "<h1>Form to item Errors</h1><br>";
            //List<Form> forms = FormStatics.GetForm.ToList();

            //foreach (Form form in forms)
            //{
            //    StaticItem item = ItemStatics.GetStaticItem.FirstOrDefault(i => i.dbName == form.BecomesItemDbName);

            //    if (item == null && (form.BecomesItemDbName != null))
            //    {
            //        output += "Form <b>" + form.dbName + "</b> refers to item <b>" + form.BecomesItemDbName + "</b> but that item does not exist!<br><br>";
            //    }

            //}

            //#endregion

            //List<StaticItem> itemsWithEffects = ItemStatics.GetStaticItem.Where(i => i.GivesEffect != null && i.GivesEffect != "").ToList();

            //foreach (StaticItem item in itemsWithEffects)
            //{
            //    StaticEffect effect = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == item.GivesEffect);

            //    if (effect == null)
            //    {
            //        output += "Item <b>" + item.dbName + "  (" + item.FriendlyName + ")</b> refers to effect <b>" + item.GivesEffect + "</b> but that effect does not exist!<br><br>";
            //    }
            //}


            @ViewBag.Message = output;
            return View("~/Views/PvP/PvPAdmin.cshtml");
        }

        public ActionResult Index()
        {

            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            ViewBag.Message = TempData["Message"];
            return View();
        }

        public ActionResult UpdateWorld()
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<Player> players = playerRepo.Players.ToList();

            foreach (Player player in players)
            {
                player.ActionPoints += 3;

                if (player.ActionPoints > PvPStatics.MaximumStoreableActionPoints)
                {
                    player.ActionPoints = PvPStatics.MaximumStoreableActionPoints;
                }
                BuffBox buffs = ItemProcedures.GetPlayerBuffs(player);
                player.Health += buffs.HealthRecoveryPerUpdate();
                player.Mana += buffs.ManaRecoveryPerUpdate();

                if (player.Health > player.MaxHealth)
                {
                    player.Health = player.MaxHealth;
                }
                if (player.Mana > player.MaxMana)
                {
                    player.Mana = player.MaxMana;
                }

            }

            foreach (Player player in players)
            {
                playerRepo.SavePlayer(player);
            }

            string output = "done";

            return View("~/Views/PvPAdmin/UpdateWorld.cshtml", output); ;
            // return View("UpdateWorld.cshtml","PvPAdmin", output);

        }

        public ActionResult SpawnAI(int number, int offset)
        {
            AIProcedures.SpawnAIPsychopaths(number, offset);
            return View("Play");
        }

        public ActionResult RunAIActions(string password)
        {

            if (password == null || password != "oogabooga99")
            {
                TempData["Result"] = "WRONG PASSWORD";
                return RedirectToAction("Play");
            }

            AIProcedures.RunPsychopathActions();

            IPlayerRepository playerRepo = new EFPlayerRepository();

            // automatically spawn in more bots when they go down
            int botCount = playerRepo.Players.Where(b => b.MembershipId == -2 && b.Mobility == "full").Count();
            if (botCount < 15)
            {
                AIProcedures.SpawnAIPsychopaths(15 - botCount, 0);

            }

            return View("Play");
        }

        public ActionResult WriteContributionToFile(int contributionId)
        {
            //http://localhost:53130/PvPAdmin/WriteContributionToFile?contributionId=110

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution con = contributionRepo.Contributions.FirstOrDefault(c => c.Id == contributionId);

            string path = Server.MapPath("~/Z_selfHelp/");
            string output = "";

            //  }, new StaticSkill {
            //                dbName = "",
            //                FriendlyName = "",
            //                FormdbName = "",
            //                Description = "",
            //                ManaCost = 0,
            //                HealthDamageAmount = 0,
            //                TFPointsAmount = 0,
            //                DiscoveryMessage = "",
            //                LearnedAtRegion = "",
            //}

            string nl = Environment.NewLine;

            string skilldbname = "skill_" + con.Skill_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");
            string formdbname = "form_" + con.Form_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");
            string itemdbname = "";

            if (con.Form_MobilityType == "inanimate")
            {
                itemdbname = "item_" + con.Form_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");
            }
            else if (con.Form_MobilityType == "animal")
            {
                itemdbname = "animal_" + con.Form_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");
            }

            #region write skill



            output = "}, new StaticSkill {" + nl;
            output += "\t\t dbName = \"" + skilldbname + "\"," + nl;
            output += "\t\t FriendlyName = \"" + con.Skill_FriendlyName + "\"," + nl;
            output += "\t\t FormdbName = \"" + formdbname + "\"," + nl;
            output += "\t\t Description = \"" + con.Skill_Description + "\"," + nl;
            output += "\t\t ManaCost = " + con.Skill_ManaCost + "M," + nl;
            output += "\t\t HealthDamageAmount = " + con.Skill_HealthDamageAmount + "M," + nl;
            output += "\t\t TFPointsAmount = " + con.Skill_TFPointsAmount + "M," + nl;

            // new system for writing skill messages out?

            string skillXMLPath = Server.MapPath("~/XMLs/SkillMessages/" + skilldbname + ".xml");

            SkillProcedures.DeleteOldXML(skillXMLPath);
            using (StreamWriter writer = new StreamWriter(skillXMLPath, false))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><StaticSkill xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + nl);
                writer.WriteLine("<DiscoveryMessage>" + con.Skill_DiscoveryMessage + "</DiscoveryMessage>" + nl);
                writer.WriteLine("<Description>" + con.Skill_Description + "</Description>" + nl);
                writer.WriteLine("</StaticSkill>" + nl);
            }

            //  output += "\t\t DiscoveryMessage = \"" + con.Skill_DiscoveryMessage + "\"," + nl;

            if (con.Skill_LearnedAtRegion != null && con.Skill_LearnedAtRegion != "")
            {
                output += "// YOU MUST DO THIS YOURSELF" + nl;
                output += "\t\t LearnedAtRegion = \"" + con.Skill_LearnedAtRegion + "\"," + nl;
            }

            output += "}" + nl + nl + nl;

            #endregion

            //new Form {
            //    dbName = "",
            //   FriendlyName = "",
            //   Description = "",
            //   Gender = "female",
            //   TFEnergyRequired = 68,
            //   MobilityType = "inanimate",
            //   PortraitUrl = "",
            //    BecomesItemDbName = "",
            //    FormBuffs = new BuffBox{},

            #region write form

            output += "}, new Form {" + nl;
            output += "\t\t dbName = \"" + formdbname + "\"," + nl;
            output += "\t\t FriendlyName = \"" + con.Form_FriendlyName + "\"," + nl;
            //output += "\t\t Description = \"" + con.Form_Description + "\"," + nl;
            output += "\t\t Gender = \"" + con.Form_Gender + "\"," + nl;
            output += "\t\t TFEnergyRequired = " + con.Form_TFEnergyRequired + "M," + nl;
            output += "\t\t MobilityType = \"" + con.Form_MobilityType + "\"," + nl;
            output += "\t\t PortraitUrl = \"\"," + nl;

            if (con.Form_MobilityType == "inanimate" || con.Form_MobilityType == "animal")
            {
                output += "\t\t BecomesItemDbName = \"" + itemdbname + "\"," + nl;
            }

            output += "\t\t FormBuffs = new BuffBox{}" + nl;
            output += "}" + nl + nl + nl;

            if (con.Form_Bonuses != null && con.Form_Bonuses != "")
            {
                output += con.Form_Bonuses + nl + nl + nl;
            }

            #endregion

            #region write to item

            if (con.Form_MobilityType != "full")
            {

                //   new StaticItem {
                //    dbName = "",
                //    FriendlyName = "",
                //    PortraitUrl = "",
                //    Description = "",
                //    ItemType = "",
                //    Findable = false,
                //    MoneyValue = 50,

                output += "}, new StaticItem {" + nl;
                output += "\t\t dbName = \"" + itemdbname + "\"," + nl;
                output += "\t\t FriendlyName = \"" + con.Item_FriendlyName + "\"," + nl;
                output += "\t\t PortraitUrl = \"\"," + nl;
                output += "\t\t Description = \"" + con.Item_Description + "\"," + nl;
                output += "\t\t ItemType = PvPStatics.ItemType_" + con.Item_ItemType + "," + nl;
                output += "\t\t Findable = false," + nl;

                output += "}" + nl + nl + nl;

                if (con.Item_Bonuses != null && con.Item_Bonuses != "")
                {
                    output += con.Item_Bonuses + nl + nl + nl;
                }


            }

            #endregion

            output += "New spell, " + con.Skill_FriendlyName + ", submitted by ";

            if (con.SubmitterUrl != null && con.SubmitterUrl != "")
            {
                output += "<a href=\"" + con.SubmitterUrl + "\">" + con.SubmitterName + "</a>!";
            }
            else
            {
                output += con.SubmitterName + "!";
            }

            if (con.AdditionalSubmitterNames != null && con.AdditionalSubmitterNames != "")
            {
                output += "  Additional credits go to " + con.AdditionalSubmitterNames + ".";
            }

            if (con.AssignedToArtist != null && con.AssignedToArtist != "")
            {
                output += "  .  Graphic is by " + con.AssignedToArtist + ".";
            }

            #region write to XML

            // ---------- WRITE TF XML --------------

            using (var stream = new FileStream(path + "spell.txt", FileMode.Truncate))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(output);
                }
            }


            string xmlPath = Server.MapPath("~/XMLs/TFMessages/" + formdbname + ".xml");




            string xmlout = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Form xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + nl;

            if (con.Form_Description != null)
            {
                xmlout += "<Description>" + con.Form_Description + "</Description>" + nl + nl;
            }

            // 1st generic 20
            if (con.Form_TFMessage_20_Percent_1st != null && con.Form_TFMessage_20_Percent_1st != "")
            {
                xmlout += "<TFMessage_20_Percent_1st>" + con.Form_TFMessage_20_Percent_1st + "</TFMessage_20_Percent_1st>" + nl + nl;
            }

            // 1st generic 40
            if (con.Form_TFMessage_40_Percent_1st != null && con.Form_TFMessage_40_Percent_1st != "")
            {
                xmlout += "<TFMessage_40_Percent_1st>" + con.Form_TFMessage_40_Percent_1st + "</TFMessage_40_Percent_1st>" + nl + nl;
            }

            // 1st generic 60
            if (con.Form_TFMessage_60_Percent_1st != null && con.Form_TFMessage_60_Percent_1st != "")
            {
                xmlout += "<TFMessage_60_Percent_1st>" + con.Form_TFMessage_60_Percent_1st + "</TFMessage_60_Percent_1st>" + nl + nl;
            }

            // 1st generic 80
            if (con.Form_TFMessage_80_Percent_1st != null && con.Form_TFMessage_80_Percent_1st != "")
            {
                xmlout += "<TFMessage_80_Percent_1st>" + con.Form_TFMessage_80_Percent_1st + "</TFMessage_80_Percent_1st>" + nl + nl;
            }


            // 1st generic 100
            if (con.Form_TFMessage_100_Percent_1st != null && con.Form_TFMessage_100_Percent_1st != "")
            {
                xmlout += "<TFMessage_100_Percent_1st>" + con.Form_TFMessage_100_Percent_1st + "</TFMessage_100_Percent_1st>" + nl + nl;
            }

            // 1st generic completed
            if (con.Form_TFMessage_Completed_1st != null && con.Form_TFMessage_Completed_1st != "")
            {
                xmlout += "<TFMessage_Completed_1st>" + con.Form_TFMessage_Completed_1st + "</TFMessage_Completed_1st>" + nl + nl;
            }

            /////////// 1st M

            // 1st generic 20
            if (con.Form_TFMessage_20_Percent_1st_M != null && con.Form_TFMessage_20_Percent_1st_M != "")
            {
                xmlout += "<TFMessage_20_Percent_1st_M>" + con.Form_TFMessage_20_Percent_1st_M + "</TFMessage_20_Percent_1st_M>" + nl + nl;
            }

            // 1st generic 40
            if (con.Form_TFMessage_40_Percent_1st_M != null && con.Form_TFMessage_40_Percent_1st_M != "")
            {
                xmlout += "<TFMessage_40_Percent_1st_M>" + con.Form_TFMessage_40_Percent_1st_M + "</TFMessage_40_Percent_1st_M>" + nl + nl;
            }

            // 1st generic 60
            if (con.Form_TFMessage_60_Percent_1st_M != null && con.Form_TFMessage_60_Percent_1st_M != "")
            {
                xmlout += "<TFMessage_60_Percent_1st_M>" + con.Form_TFMessage_60_Percent_1st_M + "</TFMessage_60_Percent_1st_M>" + nl + nl;
            }

            // 1st generic 80
            if (con.Form_TFMessage_80_Percent_1st_M != null && con.Form_TFMessage_80_Percent_1st_M != "")
            {
                xmlout += "<TFMessage_80_Percent_1st_M>" + con.Form_TFMessage_80_Percent_1st_M + "</TFMessage_80_Percent_1st_M>" + nl + nl;
            }


            // 1st generic 100
            if (con.Form_TFMessage_100_Percent_1st_M != null && con.Form_TFMessage_100_Percent_1st_M != "")
            {
                xmlout += "<TFMessage_100_Percent_1st_M>" + con.Form_TFMessage_100_Percent_1st_M + "</TFMessage_100_Percent_1st_M>" + nl + nl;
            }

            // 1st generic completed
            if (con.Form_TFMessage_Completed_1st_M != null && con.Form_TFMessage_Completed_1st_M != "")
            {
                xmlout += "<TFMessage_Completed_1st_M>" + con.Form_TFMessage_Completed_1st_M + "</TFMessage_Completed_1st_M>" + nl + nl;
            }

            /////////// 1st M

            // 1st generic 20
            if (con.Form_TFMessage_20_Percent_1st_F != null && con.Form_TFMessage_20_Percent_1st_F != "")
            {
                xmlout += "<TFMessage_20_Percent_1st_F>" + con.Form_TFMessage_20_Percent_1st_F + "</TFMessage_20_Percent_1st_F>" + nl + nl;
            }

            // 1st generic 40
            if (con.Form_TFMessage_40_Percent_1st_F != null && con.Form_TFMessage_40_Percent_1st_F != "")
            {
                xmlout += "<TFMessage_40_Percent_1st_F>" + con.Form_TFMessage_40_Percent_1st_F + "</TFMessage_40_Percent_1st_F>" + nl + nl;
            }

            // 1st generic 60
            if (con.Form_TFMessage_60_Percent_1st_F != null && con.Form_TFMessage_60_Percent_1st_F != "")
            {
                xmlout += "<TFMessage_60_Percent_1st_F>" + con.Form_TFMessage_60_Percent_1st_F + "</TFMessage_60_Percent_1st_F>" + nl + nl;
            }

            // 1st generic 80
            if (con.Form_TFMessage_80_Percent_1st_F != null && con.Form_TFMessage_80_Percent_1st_F != "")
            {
                xmlout += "<TFMessage_80_Percent_1st_F>" + con.Form_TFMessage_80_Percent_1st_F + "</TFMessage_80_Percent_1st_F>" + nl + nl;
            }


            // 1st generic 100
            if (con.Form_TFMessage_100_Percent_1st_F != null && con.Form_TFMessage_100_Percent_1st_F != "")
            {
                xmlout += "<TFMessage_100_Percent_1st_F>" + con.Form_TFMessage_100_Percent_1st_F + "</TFMessage_100_Percent_1st_F>" + nl + nl;
            }

            // 1st generic completed
            if (con.Form_TFMessage_Completed_1st_F != null && con.Form_TFMessage_Completed_1st_F != "")
            {
                xmlout += "<TFMessage_Completed_1st_F>" + con.Form_TFMessage_Completed_1st_F + "</TFMessage_Completed_1st_F>" + nl + nl;
            }

            //////////////////////////////

            // 1st generic 20
            if (con.Form_TFMessage_20_Percent_3rd != null && con.Form_TFMessage_20_Percent_3rd != "")
            {
                xmlout += "<TFMessage_20_Percent_3rd>" + con.Form_TFMessage_20_Percent_3rd + "</TFMessage_20_Percent_3rd>" + nl + nl;
            }

            // 1st generic 40
            if (con.Form_TFMessage_40_Percent_3rd != null && con.Form_TFMessage_40_Percent_3rd != "")
            {
                xmlout += "<TFMessage_40_Percent_3rd>" + con.Form_TFMessage_40_Percent_3rd + "</TFMessage_40_Percent_3rd>" + nl + nl;
            }

            // 1st generic 60
            if (con.Form_TFMessage_60_Percent_3rd != null && con.Form_TFMessage_60_Percent_3rd != "")
            {
                xmlout += "<TFMessage_60_Percent_3rd>" + con.Form_TFMessage_60_Percent_3rd + "</TFMessage_60_Percent_3rd>" + nl + nl;
            }

            // 1st generic 80
            if (con.Form_TFMessage_80_Percent_3rd != null && con.Form_TFMessage_80_Percent_3rd != "")
            {
                xmlout += "<TFMessage_80_Percent_3rd>" + con.Form_TFMessage_80_Percent_3rd + "</TFMessage_80_Percent_3rd>" + nl + nl;
            }


            // 1st generic 100
            if (con.Form_TFMessage_100_Percent_3rd != null && con.Form_TFMessage_100_Percent_3rd != "")
            {
                xmlout += "<TFMessage_100_Percent_3rd>" + con.Form_TFMessage_100_Percent_3rd + "</TFMessage_100_Percent_3rd>" + nl + nl;
            }

            // 1st generic completed
            if (con.Form_TFMessage_Completed_3rd != null && con.Form_TFMessage_Completed_3rd != "")
            {
                xmlout += "<TFMessage_Completed_3rd>" + con.Form_TFMessage_Completed_3rd + "</TFMessage_Completed_3rd>" + nl + nl;
            }

            /////////// 1st M

            // 1st generic 20
            if (con.Form_TFMessage_20_Percent_3rd_M != null && con.Form_TFMessage_20_Percent_3rd_M != "")
            {
                xmlout += "<TFMessage_20_Percent_3rd_M>" + con.Form_TFMessage_20_Percent_3rd_M + "</TFMessage_20_Percent_3rd_M>" + nl + nl;
            }

            // 1st generic 40
            if (con.Form_TFMessage_40_Percent_3rd_M != null && con.Form_TFMessage_40_Percent_3rd_M != "")
            {
                xmlout += "<TFMessage_40_Percent_3rd_M>" + con.Form_TFMessage_40_Percent_3rd_M + "</TFMessage_40_Percent_3rd_M>" + nl + nl;
            }

            // 1st generic 60
            if (con.Form_TFMessage_60_Percent_3rd_M != null && con.Form_TFMessage_60_Percent_3rd_M != "")
            {
                xmlout += "<TFMessage_60_Percent_3rd_M>" + con.Form_TFMessage_60_Percent_3rd_M + "</TFMessage_60_Percent_3rd_M>" + nl + nl;
            }

            // 1st generic 80
            if (con.Form_TFMessage_80_Percent_3rd_M != null && con.Form_TFMessage_80_Percent_3rd_M != "")
            {
                xmlout += "<TFMessage_80_Percent_3rd_M>" + con.Form_TFMessage_80_Percent_3rd_M + "</TFMessage_80_Percent_3rd_M>" + nl + nl;
            }


            // 1st generic 100
            if (con.Form_TFMessage_100_Percent_3rd_M != null && con.Form_TFMessage_100_Percent_3rd_M != "")
            {
                xmlout += "<TFMessage_100_Percent_3rd_M>" + con.Form_TFMessage_100_Percent_3rd_M + "</TFMessage_100_Percent_3rd_M>" + nl + nl;
            }

            // 1st generic completed
            if (con.Form_TFMessage_Completed_3rd_M != null && con.Form_TFMessage_Completed_3rd_M != "")
            {
                xmlout += "<TFMessage_Completed_3rd_M>" + con.Form_TFMessage_Completed_3rd_M + "</TFMessage_Completed_3rd_M>" + nl + nl;
            }

            /////////// 1st M

            // 1st generic 20
            if (con.Form_TFMessage_20_Percent_3rd_F != null && con.Form_TFMessage_20_Percent_3rd_F != "")
            {
                xmlout += "<TFMessage_20_Percent_3rd_F>" + con.Form_TFMessage_20_Percent_3rd_F + "</TFMessage_20_Percent_3rd_F>" + nl + nl;
            }

            // 1st generic 40
            if (con.Form_TFMessage_40_Percent_3rd_F != null && con.Form_TFMessage_40_Percent_3rd_F != "")
            {
                xmlout += "<TFMessage_40_Percent_3rd_F>" + con.Form_TFMessage_40_Percent_3rd_F + "</TFMessage_40_Percent_3rd_F>" + nl + nl;
            }

            // 1st generic 60
            if (con.Form_TFMessage_60_Percent_3rd_F != null && con.Form_TFMessage_60_Percent_3rd_F != "")
            {
                xmlout += "<TFMessage_60_Percent_3rd_F>" + con.Form_TFMessage_60_Percent_3rd_F + "</TFMessage_60_Percent_3rd_F>" + nl + nl;
            }

            // 1st generic 80
            if (con.Form_TFMessage_80_Percent_3rd_F != null && con.Form_TFMessage_80_Percent_3rd_F != "")
            {
                xmlout += "<TFMessage_80_Percent_3rd_F>" + con.Form_TFMessage_80_Percent_3rd_F + "</TFMessage_80_Percent_3rd_F>" + nl + nl;
            }


            // 1st generic 100
            if (con.Form_TFMessage_100_Percent_3rd_F != null && con.Form_TFMessage_100_Percent_3rd_F != "")
            {
                xmlout += "<TFMessage_100_Percent_3rd_F>" + con.Form_TFMessage_100_Percent_3rd_F + "</TFMessage_100_Percent_3rd_F>" + nl + nl;
            }

            // 1st generic completed
            if (con.Form_TFMessage_Completed_3rd_F != null && con.Form_TFMessage_Completed_3rd_F != "")
            {
                xmlout += "<TFMessage_Completed_3rd_F>" + con.Form_TFMessage_Completed_3rd_F + "</TFMessage_Completed_3rd_F>" + nl + nl;
            }




            xmlout += "</Form>";

            //using (var stream = new FileStream(xmlPath, FileMode.Truncate))
            //{
            //    using (var writer = new StreamWriter(stream))
            //    {
            //        writer.Write(xmlout);
            //    }
            //}

            SkillProcedures.DeleteOldXML(xmlPath);

            using (StreamWriter writer = new StreamWriter(xmlPath, false))
            {

                writer.WriteLine(xmlout);
            }

            #endregion

            return View("Play");
        }

        public ActionResult WriteEffectContributionToFile(int contributionId)
        {
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();

            EffectContribution con = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == contributionId);

            string path = Server.MapPath("~/Z_selfHelp/");
            string nl = Environment.NewLine;
            string output = "";


            string effectdbName = "effect_" + con.Effect_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");
            string effectXMLPath = Server.MapPath("~/XMLs/Effects/" + effectdbName + ".xml");

            // if there is any skill to be used, write that out to file as needed
            if (con.Skill_FriendlyName != null)
            {
                string skilldbName = "skill_" + con.Skill_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");

                output = "}, new StaticSkill {" + nl;
                output += "\t\t dbName = \"" + skilldbName + "\"," + nl;
                output += "\t\t FriendlyName = \"" + con.Skill_FriendlyName + "\"," + nl;
                output += "\t\t Description = \"" + con.Skill_Description + "\"," + nl;
                output += "\t\t ManaCost = " + con.Skill_ManaCost + "M," + nl;

                if (con.Skill_UniqueToForm != null && con.Skill_UniqueToForm != null)
                {
                    output += "\t\t ExclusiveToForm = \"" + con.Skill_UniqueToForm + "\"," + nl;
                }

                if (con.Skill_UniqueToItem != null && con.Skill_UniqueToItem != null)
                {
                    output += "\t\t ExclusiveToItem = \"" + con.Skill_UniqueToItem + "\"," + nl;
                }

                if (con.Skill_UniqueToLocation != null && con.Skill_UniqueToLocation != "")
                {
                    output += "\t\t UniqueToLocation = \"" + con.Skill_UniqueToLocation + "\"," + nl;
                }

                output += "\t\t GivesEffect = \"" + effectdbName + "\"," + nl;

                output += "}," + nl + nl + nl;

                // Is there any need to write a skill XML?  The text all happens with the StaticEffect object, so I think maybe not.

                //string skillXMLPath = Server.MapPath("~/XMLs/SkillMessages/" + skilldbname + ".xml");
                //SkillProcedures.DeleteOldXML(skillXMLPath);
                //using (StreamWriter writer = new StreamWriter(skillXMLPath, false))
                //{
                //    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><StaticSkill xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + nl);
                //    writer.WriteLine("<DiscoveryMessage>" + con.Effect_AttackHitText + "</DiscoveryMessage>" + nl);
                //    writer.WriteLine("<Description>" + con.Skill_Description + "</Description>" + nl);
                //    writer.WriteLine("</StaticSkill>" + nl);
                //}
            }

            // write effect

            output += "}, new StaticEffect {" + nl;
            output += "\t\t dbName = \"" + effectdbName + "\"," + nl;
            output += "\t\t FriendlyName = \"" + con.Effect_FriendlyName + "\"," + nl;
            output += "\t\t Description = \"" + con.Effect_Description + "\"," + nl;
            output += "\t\t Duration = " + con.Effect_Duration + "," + nl;
            output += "\t\t Cooldown = " + con.Effect_Cooldown + "," + nl;
            output += "\t\t AvailableAtLevel = 0," + nl;
            output += "}," + nl + nl + nl;

            output += con.Effect_Bonuses + nl + nl + nl;




            SkillProcedures.DeleteOldXML(effectXMLPath);
            using (StreamWriter writer = new StreamWriter(effectXMLPath, false))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><StaticEffect xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + nl);

                writer.WriteLine("<Description>" + con.Effect_Description + "</Description>" + nl);

                if (con.Effect_AttackHitText != null)
                {
                    writer.WriteLine("<AttackerWhenHit>" + con.Effect_AttackHitText + "</AttackerWhenHit>" + nl);
                }

                if (con.Effect_AttackHitText != null)
                {
                    writer.WriteLine("<AttackerWhenHit_M>" + con.Effect_AttackHitText + "</AttackerWhenHit_M>" + nl);
                }

                if (con.Effect_AttackHitText != null)
                {
                    writer.WriteLine("<AttackerWhenHit_F>" + con.Effect_AttackHitText + "</AttackerWhenHit_F>" + nl);
                }

                //////////////

                if (con.Effect_VictimHitText != null)
                {
                    writer.WriteLine("<MessageWhenHit>" + con.Effect_VictimHitText + "</MessageWhenHit>" + nl);
                }

                if (con.Effect_VictimHitText_M != null)
                {
                    writer.WriteLine("<MessageWhenHit_M>" + con.Effect_VictimHitText_M + "</MessageWhenHit_M>" + nl);
                }

                if (con.Effect_VictimHitText_F != null)
                {
                    writer.WriteLine("<MessageWhenHit_F>" + con.Effect_VictimHitText_F + "</MessageWhenHit_F>" + nl);
                }

                writer.WriteLine("</StaticEffect>" + nl);
            }

            output += "New curse, " + con.Skill_FriendlyName + ", submitted by " + con.SubmitterName + " with additional credits by " + con.AdditionalSubmitterNames + ".";


            using (var stream = new FileStream(path + "spell.txt", FileMode.Truncate))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(output);
                }
            }

            string effectdbname = "skill_" + con.Skill_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");

            return View("Play");
        }

        public ActionResult ApproveEffectContributionList()
        {

            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play","PvP");
            }

            IEffectContributionRepository effectConRepo = new EFEffectContributionRepository();
            IEnumerable<EffectContribution> output = effectConRepo.EffectContributions.Where(c => c.IsLive == false && c.ReadyForReview == true && c.ApprovedByAdmin == false && c.ProofreadingCopy == false);

            return View(output);
        }

        public ActionResult ApproveEffectContribution(int id)
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            IEffectContributionRepository effectConRepo = new EFEffectContributionRepository();

            EffectContribution OldCopy = effectConRepo.EffectContributions.FirstOrDefault(c => c.Id == id);

            EffectContribution ProofreadCopy = effectConRepo.EffectContributions.FirstOrDefault(c => c.ProofreadingCopy == true && c.Effect_FriendlyName == OldCopy.Effect_FriendlyName);

            if (ProofreadCopy != null)
            {
                ViewBag.Message = "There's already a proofreading copy for this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }
            else
            {
                ProofreadCopy = new EffectContribution();
            }

            OldCopy.ApprovedByAdmin = true;

            effectConRepo.SaveEffectContribution(OldCopy);

            ProofreadCopy.OwnerMemberhipId = OldCopy.OwnerMemberhipId;
            ProofreadCopy.SubmitterName = OldCopy.SubmitterName;
            ProofreadCopy.SubmitterURL = OldCopy.SubmitterURL;
            ProofreadCopy.ReadyForReview = OldCopy.ReadyForReview;

            ProofreadCopy.Skill_FriendlyName = OldCopy.Skill_FriendlyName;
            ProofreadCopy.Skill_UniqueToForm = OldCopy.Skill_UniqueToForm;
            ProofreadCopy.Skill_UniqueToItem = OldCopy.Skill_UniqueToItem;
            ProofreadCopy.Skill_UniqueToLocation = OldCopy.Skill_UniqueToLocation;
            ProofreadCopy.Skill_Description = OldCopy.Skill_Description;
            ProofreadCopy.Skill_ManaCost = OldCopy.Skill_ManaCost;

            ProofreadCopy.Effect_FriendlyName = OldCopy.Effect_FriendlyName;
            ProofreadCopy.Effect_Description = OldCopy.Effect_Description;
            ProofreadCopy.Effect_Duration = OldCopy.Effect_Duration;
            ProofreadCopy.Effect_Cooldown = OldCopy.Effect_Cooldown;
            ProofreadCopy.Effect_Bonuses = OldCopy.Effect_Bonuses;
            ProofreadCopy.Effect_VictimHitText = OldCopy.Effect_VictimHitText;
            ProofreadCopy.Effect_VictimHitText_M = OldCopy.Effect_VictimHitText_M;
            ProofreadCopy.Effect_VictimHitText_F = OldCopy.Effect_VictimHitText_F;
            ProofreadCopy.Effect_AttackHitText = OldCopy.Effect_AttackHitText;
            ProofreadCopy.Effect_AttackHitText_M = OldCopy.Effect_AttackHitText_M;
            ProofreadCopy.Effect_AttackHitText_F = OldCopy.Effect_AttackHitText_F;

            ProofreadCopy.Timestamp = DateTime.UtcNow;
            ProofreadCopy.ProofreadingCopy = true;
            ProofreadCopy.ProofreadingCopyForOriginalId = OldCopy.Id;
            ProofreadCopy.ApprovedByAdmin = true;

            ProofreadCopy.AdditionalSubmitterNames = OldCopy.AdditionalSubmitterNames;
            ProofreadCopy.Notes = OldCopy.Notes;

            // stats
            ProofreadCopy.HealthBonusPercent = OldCopy.HealthBonusPercent;
            ProofreadCopy.ManaBonusPercent = OldCopy.ManaBonusPercent;
            ProofreadCopy.ExtraSkillCriticalPercent = OldCopy.ExtraSkillCriticalPercent;
            ProofreadCopy.HealthRecoveryPerUpdate = OldCopy.HealthRecoveryPerUpdate;
            ProofreadCopy.ManaRecoveryPerUpdate = OldCopy.ManaRecoveryPerUpdate;
            ProofreadCopy.SneakPercent = OldCopy.SneakPercent;
            ProofreadCopy.EvasionPercent = OldCopy.EvasionPercent;
            ProofreadCopy.EvasionNegationPercent = OldCopy.EvasionNegationPercent;
            ProofreadCopy.MeditationExtraMana = OldCopy.MeditationExtraMana;
            ProofreadCopy.CleanseExtraHealth = OldCopy.CleanseExtraHealth;
            ProofreadCopy.MoveActionPointDiscount = OldCopy.MoveActionPointDiscount;
            ProofreadCopy.SpellExtraTFEnergyPercent = OldCopy.SpellExtraTFEnergyPercent;
            ProofreadCopy.SpellExtraHealthDamagePercent = OldCopy.SpellExtraHealthDamagePercent;
            ProofreadCopy.CleanseExtraTFEnergyRemovalPercent = OldCopy.CleanseExtraTFEnergyRemovalPercent;
            ProofreadCopy.SpellMisfireChanceReduction = OldCopy.SpellMisfireChanceReduction;
            ProofreadCopy.SpellHealthDamageResistance = OldCopy.SpellHealthDamageResistance;
            ProofreadCopy.SpellTFEnergyDamageResistance = OldCopy.SpellTFEnergyDamageResistance;
            ProofreadCopy.ExtraInventorySpace = OldCopy.ExtraInventorySpace;

            effectConRepo.SaveEffectContribution(ProofreadCopy);


            return RedirectToAction("ApproveEffectContributionList");

            // IEffectContributionRepository conRepo = new EFEffectContributionRepository();
        }

        public ActionResult DoesLocationHaveXML()
        {

            string nl = Environment.NewLine;
            string output = "";
            ViewBag.Message = "";

            foreach (Location loc in LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != ""))
            {

                if (loc.Description == null || loc.Description == "")
                {

                    string xmlpathLocation = Server.MapPath("~/XMLs/LocationDescriptions/" + loc.dbName + ".xml");

                    if (!System.IO.File.Exists(xmlpathLocation))
                    {
                        output += xmlpathLocation + "</br>";
                    }
                   // SkillProcedures.DeleteOldXML(xmlpathLocation);

                    //using (StreamWriter writer = new StreamWriter(xmlpathLocation, false))
                    //{
                    //    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + nl);
                    //    writer.WriteLine("<Description>" + loc.Description + "</Description>" + nl);
                    //    writer.WriteLine("</Location>" + nl);
                    //}

                }



            }

            ViewBag.Text = output;

            return View("Play");
        }

        public ActionResult PublicBroadcast()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }
            else
            {
                PublicBroadcastViewModel output = new PublicBroadcastViewModel();
                return View(output);
            }
        }

        public ActionResult SendPublicBroadcast(PublicBroadcastViewModel input)
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }
            else
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                IPlayerLogRepository logRepo = new EFPlayerLogRepository();

                string msg = "<span class='bad'>PUBLIC SERVER NOTE:  " + input.Message + "</span>";

                List<Player> players = playerRepo.Players.Where(p => p.MembershipId > 0).ToList();

                string errors = "";

                foreach (Player p in players)
                {
                    try
                    {
                        PlayerLogProcedures.AddPlayerLog(p.Id, msg, true);
                    }
                    catch (Exception e)
                    {
                        errors += "<p>" + p.FirstName + " " + p.LastName + "encountered:  " + e.ToString() + "</p><br/>";
                    }
                }

                TempData["Message"] = errors;

                return RedirectToAction("Index");
            }
        }

        public ActionResult ChangeGameDate()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }
            else
            {
                PublicBroadcastViewModel output = new PublicBroadcastViewModel();
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);

                if (stats != null)
                {
                    output.Message = stats.GameNewsDate;
                }

                return View(output);
            }
        }

        public ActionResult ChangeGameDateSend(PublicBroadcastViewModel input)
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }
            else
            {
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.GameNewsDate = input.Message;
                repo.SavePvPWorldStat(stats);
                PvPStatics.LastGameUpdate = stats.GameNewsDate;

             //   TempData["Message"] = errors;

                return RedirectToAction("Index");
            }
        }

        public ActionResult test()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            AIProcedures.SpawnBartender();
            
            NoticeProcedures.PushNotice(PlayerProcedures.GetPlayerFromMembership(), "test");

            return RedirectToAction("Index");
        }

        public ActionResult SpawnNPCs()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            AIProcedures.SpawnLindella();
            BossProcedures_PetMerchant.SpawnPetMerchant();
            BossProcedures_Fae.SpawnFae();
            AIProcedures.SpawnBartender();

            return RedirectToAction("Index");
        }

        public ActionResult ItemPetJSON()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_JSON) == false)
            {
                return View("Play", "PvP");
            }

            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            return Json(itemRepo.DbStaticItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormJSON()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_JSON) == false)
            {
                return View("Play", "PvP");
            }

            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            return Json(formRepo.DbStaticForms, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SpellJSON()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_JSON) == false)
            {
                return View("Play", "PvP");
            }

            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            IEnumerable<DbStaticSkill> output = skillRepo.DbStaticSkills.ToList();
            foreach (DbStaticSkill s in output)
            {
                s.LearnedAtLocation = "";
                s.LearnedAtRegion = "";
            }
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EffectJSON()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_JSON) == false)
            {
                return View("Play", "PvP");
            }

            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return Json(effectRepo.DbStaticEffects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FurnitureJSON()
        {
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_JSON) == false)
            {
                return View("Play", "PvP");
            }

            IDbStaticFurnitureRepository effectRepo = new EFDbStaticFurnitureRepository();
            return Json(effectRepo.DbStaticFurnitures, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveContributionList()
        {
            // assert only admin can view this
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_Previewer) == false)
            {
                return View("Play", "PvP");
            }

            IContributionRepository contRepo = new EFContributionRepository();

            IEnumerable<Contribution> output = contRepo.Contributions.Where(c => c.IsLive == false && c.IsReadyForReview == true && c.AdminApproved == false && c.ProofreadingCopy == false);
            return View(output);

        }

        

        public ActionResult ApproveContribution(int id)
        {
            // assert only admin can view this
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();

            Contribution OldCopy = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            Contribution ProofreadCopy = contributionRepo.Contributions.FirstOrDefault(c => c.ProofreadingCopy == true && c.Skill_FriendlyName == OldCopy.Skill_FriendlyName);


            Player owner = PlayerProcedures.GetPlayerFromMembership(OldCopy.OwnerMembershipId);

            if (owner != null)
            {
                PlayerLogProcedures.AddPlayerLog(owner.Id, "<b>A contribution you have submitted has been approved.</b>", true);
            }

            if (ProofreadCopy != null)
            {
                ViewBag.Message = "There's already a proofreading copy for this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }
            else
            {
                ProofreadCopy = new Contribution();
            }

            // unlock the proofreading flag since it has been saved


            ProofreadCopy.OwnerMembershipId = OldCopy.OwnerMembershipId;
            ProofreadCopy.IsReadyForReview = true;
            ProofreadCopy.IsLive = false;
            ProofreadCopy.ProofreadingCopy = true;
            ProofreadCopy.CheckedOutBy = "";
            ProofreadCopy.ProofreadingLockIsOn = false;
            ProofreadCopy.AdminApproved = true;

            ProofreadCopy.Skill_FriendlyName = OldCopy.Skill_FriendlyName;
            ProofreadCopy.Skill_FormFriendlyName = OldCopy.Skill_FormFriendlyName;
            ProofreadCopy.Skill_Description = OldCopy.Skill_Description;
            ProofreadCopy.Skill_ManaCost = OldCopy.Skill_ManaCost;
            ProofreadCopy.Skill_TFPointsAmount = OldCopy.Skill_TFPointsAmount;
            ProofreadCopy.Skill_HealthDamageAmount = OldCopy.Skill_HealthDamageAmount;
            ProofreadCopy.Skill_LearnedAtLocationOrRegion = OldCopy.Skill_LearnedAtLocationOrRegion;
            ProofreadCopy.Skill_LearnedAtRegion = OldCopy.Skill_LearnedAtRegion;
            ProofreadCopy.Skill_DiscoveryMessage = OldCopy.Skill_DiscoveryMessage;

            ProofreadCopy.Form_FriendlyName = OldCopy.Form_FriendlyName;
            ProofreadCopy.Form_Description = OldCopy.Form_Description;
            ProofreadCopy.Form_TFEnergyRequired = OldCopy.Form_TFEnergyRequired;
            ProofreadCopy.Form_Gender = OldCopy.Form_Gender;
            ProofreadCopy.Form_MobilityType = OldCopy.Form_MobilityType;
            ProofreadCopy.Form_BecomesItemDbName = OldCopy.Form_BecomesItemDbName;
            ProofreadCopy.Form_Bonuses = OldCopy.Form_Bonuses;

            ProofreadCopy.Form_TFMessage_20_Percent_1st = OldCopy.Form_TFMessage_20_Percent_1st;
            ProofreadCopy.Form_TFMessage_40_Percent_1st = OldCopy.Form_TFMessage_40_Percent_1st;
            ProofreadCopy.Form_TFMessage_60_Percent_1st = OldCopy.Form_TFMessage_60_Percent_1st;
            ProofreadCopy.Form_TFMessage_80_Percent_1st = OldCopy.Form_TFMessage_80_Percent_1st;
            ProofreadCopy.Form_TFMessage_100_Percent_1st = OldCopy.Form_TFMessage_100_Percent_1st;
            ProofreadCopy.Form_TFMessage_Completed_1st = OldCopy.Form_TFMessage_Completed_1st;

            ProofreadCopy.Form_TFMessage_20_Percent_1st_M = OldCopy.Form_TFMessage_20_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_40_Percent_1st_M = OldCopy.Form_TFMessage_40_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_60_Percent_1st_M = OldCopy.Form_TFMessage_60_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_80_Percent_1st_M = OldCopy.Form_TFMessage_80_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_100_Percent_1st_M = OldCopy.Form_TFMessage_100_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_Completed_1st_M = OldCopy.Form_TFMessage_Completed_1st_M;

            ProofreadCopy.Form_TFMessage_20_Percent_1st_F = OldCopy.Form_TFMessage_20_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_40_Percent_1st_F = OldCopy.Form_TFMessage_40_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_60_Percent_1st_F = OldCopy.Form_TFMessage_60_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_80_Percent_1st_F = OldCopy.Form_TFMessage_80_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_100_Percent_1st_F = OldCopy.Form_TFMessage_100_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_Completed_1st_F = OldCopy.Form_TFMessage_Completed_1st_F;

            ProofreadCopy.Form_TFMessage_20_Percent_3rd = OldCopy.Form_TFMessage_20_Percent_3rd;
            ProofreadCopy.Form_TFMessage_40_Percent_3rd = OldCopy.Form_TFMessage_40_Percent_3rd;
            ProofreadCopy.Form_TFMessage_60_Percent_3rd = OldCopy.Form_TFMessage_60_Percent_3rd;
            ProofreadCopy.Form_TFMessage_80_Percent_3rd = OldCopy.Form_TFMessage_80_Percent_3rd;
            ProofreadCopy.Form_TFMessage_100_Percent_3rd = OldCopy.Form_TFMessage_100_Percent_3rd;
            ProofreadCopy.Form_TFMessage_Completed_3rd = OldCopy.Form_TFMessage_Completed_3rd;

            ProofreadCopy.Form_TFMessage_20_Percent_3rd_M = OldCopy.Form_TFMessage_20_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_40_Percent_3rd_M = OldCopy.Form_TFMessage_40_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_60_Percent_3rd_M = OldCopy.Form_TFMessage_60_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_80_Percent_3rd_M = OldCopy.Form_TFMessage_80_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_100_Percent_3rd_M = OldCopy.Form_TFMessage_100_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_Completed_3rd_M = OldCopy.Form_TFMessage_Completed_3rd_M;

            ProofreadCopy.Form_TFMessage_20_Percent_3rd_F = OldCopy.Form_TFMessage_20_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_40_Percent_3rd_F = OldCopy.Form_TFMessage_40_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_60_Percent_3rd_F = OldCopy.Form_TFMessage_60_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_80_Percent_3rd_F = OldCopy.Form_TFMessage_80_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_100_Percent_3rd_F = OldCopy.Form_TFMessage_100_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_Completed_3rd_F = OldCopy.Form_TFMessage_Completed_3rd_F;

            ProofreadCopy.Item_FriendlyName = OldCopy.Item_FriendlyName;
            ProofreadCopy.Item_Description = OldCopy.Item_Description;
            ProofreadCopy.Item_ItemType = OldCopy.Item_ItemType;
            ProofreadCopy.Item_UseCooldown = OldCopy.Item_UseCooldown;
            ProofreadCopy.Item_Bonuses = OldCopy.Item_Bonuses;

            ProofreadCopy.SubmitterName = OldCopy.SubmitterName;
            ProofreadCopy.AdditionalSubmitterNames = OldCopy.AdditionalSubmitterNames;
            ProofreadCopy.Notes = OldCopy.Notes;
            ProofreadCopy.SubmitterUrl = OldCopy.SubmitterUrl;

            ProofreadCopy.AssignedToArtist = OldCopy.AssignedToArtist;

            ProofreadCopy.CreationTimestamp = DateTime.UtcNow;

            ProofreadCopy.ProofreadingCopyForOriginalId = OldCopy.Id;

            ProofreadCopy.HealthBonusPercent = OldCopy.HealthBonusPercent;
            ProofreadCopy.ManaBonusPercent = OldCopy.ManaBonusPercent;
            ProofreadCopy.ExtraSkillCriticalPercent = OldCopy.ExtraSkillCriticalPercent;
            ProofreadCopy.HealthRecoveryPerUpdate = OldCopy.HealthRecoveryPerUpdate;
            ProofreadCopy.ManaRecoveryPerUpdate = OldCopy.ManaRecoveryPerUpdate;
            ProofreadCopy.SneakPercent = OldCopy.SneakPercent;
            ProofreadCopy.EvasionPercent = OldCopy.EvasionPercent;
            ProofreadCopy.EvasionNegationPercent = OldCopy.EvasionNegationPercent;
            ProofreadCopy.MeditationExtraMana = OldCopy.MeditationExtraMana;
            ProofreadCopy.CleanseExtraHealth = OldCopy.CleanseExtraHealth;
            ProofreadCopy.MoveActionPointDiscount = OldCopy.MoveActionPointDiscount;
            ProofreadCopy.SpellExtraTFEnergyPercent = OldCopy.SpellExtraTFEnergyPercent;
            ProofreadCopy.SpellExtraHealthDamagePercent = OldCopy.SpellExtraHealthDamagePercent;
            ProofreadCopy.CleanseExtraTFEnergyRemovalPercent = OldCopy.CleanseExtraTFEnergyRemovalPercent;
            ProofreadCopy.SpellMisfireChanceReduction = OldCopy.SpellMisfireChanceReduction;
            ProofreadCopy.SpellHealthDamageResistance = OldCopy.SpellHealthDamageResistance;
            ProofreadCopy.SpellTFEnergyDamageResistance = OldCopy.SpellTFEnergyDamageResistance;
            ProofreadCopy.ExtraInventorySpace = OldCopy.ExtraInventorySpace;

            ProofreadCopy.ImageURL = OldCopy.ImageURL;

            // ------------------ ORIGINAL COPY ------------------------------


            OldCopy.IsReadyForReview = true;
            OldCopy.IsLive = false;
            OldCopy.AdminApproved = true;
            OldCopy.ProofreadingCopy = false;

            

            contributionRepo.SaveContribution(ProofreadCopy);
            contributionRepo.SaveContribution(OldCopy);

            ViewBag.Message = "Success.";
            return RedirectToAction("ApproveContributionList");

        }

        public ActionResult RejectContribution(int id)
        {
            // assert only admin can view this
            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            IContributionRepository contRepo = new EFContributionRepository();

            Contribution contribution = contRepo.Contributions.FirstOrDefault(i => i.Id == id);
            contribution.IsReadyForReview = false;
            contRepo.SaveContribution(contribution);

            Player owner = PlayerProcedures.GetPlayerFromMembership(contribution.OwnerMembershipId);

            if (owner != null)
            {
                PlayerLogProcedures.AddPlayerLog(owner.Id, "<b>A contribution you have submitted has been rejected.  You should have received or will soon a message explaining why or what else needs to be done before this contribution can be accepted.  If you do not, please message Judoo on the forums.</b>", true);
            }


            return RedirectToAction("ApproveContributionList");
        }

        public ActionResult SpawnLindella()
        {
            AIProcedures.SpawnLindella();
            return View("Play");
        }

        public ActionResult SkillsFormsWithoutXMLs()
        {
            IContributionRepository contributionRepo = new EFContributionRepository();

            string output = "";

            List<Contribution> liveContributions = contributionRepo.Contributions.Where(i => i.IsLive == true && i.ProofreadingCopy == true).ToList();
            List<Contribution> noXMLs = new List<Contribution>();

            foreach (Contribution con in liveContributions)
            {

                string skilldbname = "skill_" + con.Skill_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");
                string formdbname = "form_" + con.Form_FriendlyName.Replace(" ", "_") + "_" + con.SubmitterName.Replace(" ", "_");

                string skillXMLName = Server.MapPath("~/XMLs/SkillMessages/" + skilldbname + ".xml");
                string formXMLName = Server.MapPath("~/XMLs/TFMessages/" + formdbname + ".xml");

                if (!System.IO.File.Exists(skillXMLName))
                {
                    output += "No XML file found for <span class='skill'>Skill</span> <b>" + con.Skill_FriendlyName + "</b>.  (Expected <i>" + skilldbname + ".xml</i>)</br>";
                }

                if (!System.IO.File.Exists(formXMLName))
                {
                    output += "No XML file found for <span class='form'>Form</span> <b>" + con.Form_FriendlyName + "</b>. (Expected <i>" + formdbname + ".xml</i>)</br>";
                }

                output += "<hr>";

            }


            ViewBag.Text = output;
            return View("Play");
        }

        public ActionResult ServerBalance_Forms()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            foreach (DbStaticForm form in FormStatics.GetAllAnimateForms())
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(form);
                decimal balance = bbox.GetBalance();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = form.dbName,
                    FriendlyName = form.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Forms";
            return View("ServerBalance", output.OrderByDescending(s => s.Balance));

        }

        public ActionResult ServerBalance_Items()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            foreach (DbStaticItem item in ItemStatics.GetAllNonPetItems())
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(item);
                decimal balance = bbox.GetBalance();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = item.dbName,
                    FriendlyName = item.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Items";
            return View("ServerBalance", output.OrderByDescending(s => s.Balance));

        }

        public ActionResult ServerBalance_Pets()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            foreach (DbStaticItem item in ItemStatics.GetAllPetItems())
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(item);
                decimal balance = bbox.GetBalance();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = item.dbName,
                    FriendlyName = item.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Pets";
            return View("ServerBalance", output.OrderByDescending(s => s.Balance));

        }

        public ActionResult ServerBalance_Effects()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            IEffectContributionRepository effectContributionRepo = new EFEffectContributionRepository();
            List<EffectContribution> effectsToAnalyze = effectContributionRepo.EffectContributions.Where(e => e.IsLive == true && e.ProofreadingCopy == true).ToList();

            foreach (EffectContribution effect in effectsToAnalyze)
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(effect);
                decimal balance = bbox.GetBalance__NoModifiersOrCaps();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = effect.GetEffectDbName(),
                    FriendlyName = effect.Effect_FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Effects";
            return View("ServerBalance", output.OrderByDescending(s => s.Balance));

        }

        //public ActionResult WriteSkillsFromMemoryToDatabase()
        //{
        //    IDbStaticSkillRepository repo = new EFDbStaticSkillRepository();
        //    foreach (StaticSkill sskill in SkillStatics.GetOldStaticSkill)
        //    {
        //        DbStaticSkill addme = new DbStaticSkill();

        //        addme.dbName = sskill.dbName;
        //        addme.Description = sskill.Description;
        //        addme.ExclusiveToForm = sskill.ExclusiveToForm;
        //        addme.ExclusiveToItem = sskill.ExclusiveToItem;
        //        addme.FormdbName = sskill.FormdbName;
        //        addme.FriendlyName = sskill.FriendlyName;
        //        addme.LearnedAtRegion = sskill.LearnedAtRegion;
        //        addme.LearnedAtLocation = sskill.LearnedAtLocation;
        //        addme.ManaCost = sskill.ManaCost;
        //        addme.HealthDamageAmount = sskill.HealthDamageAmount;
        //        addme.TFPointsAmount = sskill.TFPointsAmount;
        //        addme.GivesEffect = sskill.GivesEffect;

        //        if (sskill.DiscoveryMessage == null || sskill.DiscoveryMessage == "")
        //        {
        //            string path = HttpContext.Server.MapPath("~/XMLs/SkillMessages/" + sskill.dbName + ".xml");
        //            StaticSkill xmlSkill = null;
        //            try
        //            {
        //                var serializer = new XmlSerializer(typeof(StaticSkill));
        //                using (var reader = XmlReader.Create(path))
        //                {
        //                    xmlSkill = (StaticSkill)serializer.Deserialize(reader);
        //                    addme.DiscoveryMessage = xmlSkill.DiscoveryMessage;
        //                }

        //            }
        //            catch
        //            {
        //                // throw a more user-friendly exception if the XML file can't be loaded for some reason or other.
        //                //throw new Exception("Failed to load XML for this spell's transformation form.  This is a server error.");
        //            }

        //        }
        //        else
        //        {
        //            addme.DiscoveryMessage = sskill.DiscoveryMessage;
        //        }

        //        if (sskill.FormdbName != null && sskill.FormdbName != "")
        //        {
        //            try
        //            {
        //                addme.MobilityType = FormStatics.GetForm.FirstOrDefault(f => f.dbName == sskill.FormdbName).MobilityType;
        //            }
        //            catch
        //            {

        //            }
        //        }
        //        else if (sskill.GivesEffect != null && sskill.GivesEffect != "")
        //        {
        //            addme.MobilityType = "curse";
        //            // addme.MobilityType = EffectStatics.GetStaticEffect.FirstOrDefault(f => f.dbName == sskill.GivesEffect);
        //        }
        //        else
        //        {
        //            addme.MobilityType = "weaken";
        //        }


        //        repo.SaveDbStaticSkill(addme);

        //    }

        //    return View("Play");
        //}

        public ActionResult WriteFormsFromMemoryToDatabase()
        {
            //IDbStaticSkillRepository repo = new EFDbStaticSkillRepository();
            IDbStaticFormRepository repo = new EFDbStaticFormRepository();
            ITFMessageRepository tfrepo = new EFTFMessageRepository();

            List<DbStaticForm> updateNeeded = new List<DbStaticForm>();

            //foreach (DbStaticForm s in repo.DbStaticForms)
            //{
            //    if (s.Description == null || s.Description == "")
            //    {
            //        string path = HttpContext.Server.MapPath("~/XMLs/TFMessages/" + s.dbName + ".xml");

            //        Form xmlForm = null;

            //        try
            //        {

            //            var serializer = new XmlSerializer(typeof(Form));
            //            using (var reader = XmlReader.Create(path))
            //            {
            //                xmlForm = (Form)serializer.Deserialize(reader);
            //            }

            //            s.Description = xmlForm.Description;
            //            updateNeeded.Add(s);

            //        }
            //        catch (Exception e)
            //        {
            //            // throw a more user-friendly exception if the XML file can't be loaded for some reason or other.
            //            //throw new Exception("Failed to load XML for this spell's transformation form.  This is a server error.");
            //        }
            //    }
            //}

            //foreach (DbStaticForm s in updateNeeded)
            //{
            //    repo.SaveDbStaticForm(s);
            //}

            

            //    public string dbName { get; set; }
            //public string FriendlyName { get; set; }
            //public string Description { get; set; }
            //public string TFEnergyType { get; set; }
            //public decimal TFEnergyRequired { get; set; }
            //public string Gender { get; set; }
            //public string MobilityType { get; set; }
            //public string BecomesItemDbName { get; set; }
            //public string PortraitUrl { get; set; }

            //foreach (Form f in FormStatics.GetForm2)
            //{
            //    DbStaticForm sf = new DbStaticForm
            //    {
            //        dbName = f.dbName,
            //        FriendlyName = f.FriendlyName,
            //        Description = f.Description,
            //        TFEnergyType = f.TFEnergyType,
            //        TFEnergyRequired = f.TFEnergyRequired,
            //        Gender = f.Gender,
            //        MobilityType = f.MobilityType,
            //        BecomesItemDbName = f.BecomesItemDbName,
            //        PortraitUrl = f.PortraitUrl,
            //        IsUnique = false,

            //        HealthBonusPercent = f.FormBuffs.FromForm_HealthBonusPercent,
            //        ManaBonusPercent = f.FormBuffs.FromForm_ManaBonusPercent,
            //        ExtraSkillCriticalPercent = f.FormBuffs.FromForm_ExtraSkillCriticalPercent,
            //        HealthRecoveryPerUpdate = f.FormBuffs.FromForm_HealthRecoveryPerUpdate,
            //        ManaRecoveryPerUpdate = f.FormBuffs.FromForm_ManaRecoveryPerUpdate,
            //        SneakPercent = f.FormBuffs.FromForm_SneakPercent,
            //        EvasionPercent = f.FormBuffs.FromForm_EvasionPercent,
            //        EvasionNegationPercent = f.FormBuffs.FromForm_EvasionNegationPercent,
            //        MeditationExtraMana = f.FormBuffs.FromForm_MeditationExtraMana,
            //        CleanseExtraHealth = f.FormBuffs.FromForm_CleanseExtraHealth,
            //        MoveActionPointDiscount = f.FormBuffs.FromForm_MoveActionPointDiscount,
            //        SpellExtraTFEnergyPercent = f.FormBuffs.FromForm_SpellExtraTFEnergyPercent,
            //        SpellExtraHealthDamagePercent = f.FormBuffs.FromForm_SpellExtraHealthDamagePercent,
            //        CleanseExtraTFEnergyRemovalPercent = f.FormBuffs.FromForm_CleanseExtraTFEnergyRemovalPercent,
            //        SpellMisfireChanceReduction = f.FormBuffs.FromForm_SpellMisfireChanceReduction,
            //        SpellHealthDamageResistance = f.FormBuffs.FromForm_SpellHealthDamageResistance,
            //        SpellTFEnergyDamageResistance = f.FormBuffs.FromForm_SpellTFEnergyDamageResistance,
            //        ExtraInventorySpace = f.FormBuffs.FromForm_ExtraInventorySpace,

            //    };

            //    repo.SaveDbStaticForm(sf);

                // load the TF Messages from the appropriate XML file if need be
                //if (f.TFMessage_20_Percent_1st == null && f.TFMessage_20_Percent_1st_M == null && f.TFMessage_20_Percent_1st_F == null)
                //{
                //    Form copy = f;

                //    try
                //    {
                     //   copy = TFEnergyProcedures.LoadTFMessagesFromXML(f);

                //        TFMessage tfm = new TFMessage
                //        {
                //            FormDbName = copy.dbName,
                //            TFMessage_20_Percent_1st = copy.TFMessage_20_Percent_1st,
                //            TFMessage_40_Percent_1st = copy.TFMessage_40_Percent_1st,
                //            TFMessage_60_Percent_1st = copy.TFMessage_60_Percent_1st,
                //            TFMessage_80_Percent_1st = copy.TFMessage_80_Percent_1st,
                //            TFMessage_100_Percent_1st = copy.TFMessage_100_Percent_1st,
                //            TFMessage_Completed_1st = copy.TFMessage_Completed_1st,

                //            TFMessage_20_Percent_1st_M = copy.TFMessage_20_Percent_1st_M,
                //            TFMessage_40_Percent_1st_M = copy.TFMessage_40_Percent_1st_M,
                //            TFMessage_60_Percent_1st_M = copy.TFMessage_60_Percent_1st_M,
                //            TFMessage_80_Percent_1st_M = copy.TFMessage_80_Percent_1st_M,
                //            TFMessage_100_Percent_1st_M = copy.TFMessage_100_Percent_1st_M,
                //            TFMessage_Completed_1st_M = copy.TFMessage_Completed_1st_M,

                //            TFMessage_20_Percent_1st_F = copy.TFMessage_20_Percent_1st_F,
                //            TFMessage_40_Percent_1st_F = copy.TFMessage_40_Percent_1st_F,
                //            TFMessage_60_Percent_1st_F = copy.TFMessage_60_Percent_1st_F,
                //            TFMessage_80_Percent_1st_F = copy.TFMessage_80_Percent_1st_F,
                //            TFMessage_100_Percent_1st_F = copy.TFMessage_100_Percent_1st_F,
                //            TFMessage_Completed_1st_F = copy.TFMessage_Completed_1st_F,

                //            TFMessage_20_Percent_3rd = copy.TFMessage_20_Percent_3rd,
                //            TFMessage_40_Percent_3rd = copy.TFMessage_40_Percent_3rd,
                //            TFMessage_60_Percent_3rd = copy.TFMessage_60_Percent_3rd,
                //            TFMessage_80_Percent_3rd = copy.TFMessage_80_Percent_3rd,
                //            TFMessage_100_Percent_3rd = copy.TFMessage_100_Percent_3rd,
                //            TFMessage_Completed_3rd = copy.TFMessage_Completed_3rd,

                //            TFMessage_20_Percent_3rd_M = copy.TFMessage_20_Percent_3rd_M,
                //            TFMessage_40_Percent_3rd_M = copy.TFMessage_40_Percent_3rd_M,
                //            TFMessage_60_Percent_3rd_M = copy.TFMessage_60_Percent_3rd_M,
                //            TFMessage_80_Percent_3rd_M = copy.TFMessage_80_Percent_3rd_M,
                //            TFMessage_100_Percent_3rd_M = copy.TFMessage_100_Percent_3rd_M,
                //            TFMessage_Completed_3rd_M = copy.TFMessage_Completed_3rd_M,

                //            TFMessage_20_Percent_3rd_F = copy.TFMessage_20_Percent_3rd_F,
                //            TFMessage_40_Percent_3rd_F = copy.TFMessage_40_Percent_3rd_F,
                //            TFMessage_60_Percent_3rd_F = copy.TFMessage_60_Percent_3rd_F,
                //            TFMessage_80_Percent_3rd_F = copy.TFMessage_80_Percent_3rd_F,
                //            TFMessage_100_Percent_3rd_F = copy.TFMessage_100_Percent_3rd_F,
                //            TFMessage_Completed_3rd_F = copy.TFMessage_Completed_3rd_F,

                //        };
                //        tfrepo.SaveTFMessage(tfm);

                //    }
                //    catch
                //    {

                //    }

                //}
                //else
                //{
                //    TFMessage tfm = new TFMessage
                //    {
                //        FormDbName = f.dbName,
                //        TFMessage_20_Percent_1st = f.TFMessage_20_Percent_1st,
                //        TFMessage_40_Percent_1st = f.TFMessage_40_Percent_1st,
                //        TFMessage_60_Percent_1st = f.TFMessage_60_Percent_1st,
                //        TFMessage_80_Percent_1st = f.TFMessage_80_Percent_1st,
                //        TFMessage_100_Percent_1st = f.TFMessage_100_Percent_1st,
                //        TFMessage_Completed_1st = f.TFMessage_Completed_1st,

                //        TFMessage_20_Percent_1st_M = f.TFMessage_20_Percent_1st_M,
                //        TFMessage_40_Percent_1st_M = f.TFMessage_40_Percent_1st_M,
                //        TFMessage_60_Percent_1st_M = f.TFMessage_60_Percent_1st_M,
                //        TFMessage_80_Percent_1st_M = f.TFMessage_80_Percent_1st_M,
                //        TFMessage_100_Percent_1st_M = f.TFMessage_100_Percent_1st_M,
                //        TFMessage_Completed_1st_M = f.TFMessage_Completed_1st_M,

                //        TFMessage_20_Percent_1st_F = f.TFMessage_20_Percent_1st_F,
                //        TFMessage_40_Percent_1st_F = f.TFMessage_40_Percent_1st_F,
                //        TFMessage_60_Percent_1st_F = f.TFMessage_60_Percent_1st_F,
                //        TFMessage_80_Percent_1st_F = f.TFMessage_80_Percent_1st_F,
                //        TFMessage_100_Percent_1st_F = f.TFMessage_100_Percent_1st_F,
                //        TFMessage_Completed_1st_F = f.TFMessage_Completed_1st_F,

                //        TFMessage_20_Percent_3rd = f.TFMessage_20_Percent_3rd,
                //        TFMessage_40_Percent_3rd = f.TFMessage_40_Percent_3rd,
                //        TFMessage_60_Percent_3rd = f.TFMessage_60_Percent_3rd,
                //        TFMessage_80_Percent_3rd = f.TFMessage_80_Percent_3rd,
                //        TFMessage_100_Percent_3rd = f.TFMessage_100_Percent_3rd,
                //        TFMessage_Completed_3rd = f.TFMessage_Completed_3rd,

                //        TFMessage_20_Percent_3rd_M = f.TFMessage_20_Percent_3rd_M,
                //        TFMessage_40_Percent_3rd_M = f.TFMessage_40_Percent_3rd_M,
                //        TFMessage_60_Percent_3rd_M = f.TFMessage_60_Percent_3rd_M,
                //        TFMessage_80_Percent_3rd_M = f.TFMessage_80_Percent_3rd_M,
                //        TFMessage_100_Percent_3rd_M = f.TFMessage_100_Percent_3rd_M,
                //        TFMessage_Completed_3rd_M = f.TFMessage_Completed_3rd_M,

                //        TFMessage_20_Percent_3rd_F = f.TFMessage_20_Percent_3rd_F,
                //        TFMessage_40_Percent_3rd_F = f.TFMessage_40_Percent_3rd_F,
                //        TFMessage_60_Percent_3rd_F = f.TFMessage_60_Percent_3rd_F,
                //        TFMessage_80_Percent_3rd_F = f.TFMessage_80_Percent_3rd_F,
                //        TFMessage_100_Percent_3rd_F = f.TFMessage_100_Percent_3rd_F,
                //        TFMessage_Completed_3rd_F = f.TFMessage_Completed_3rd_F,

                //    };

                //    tfrepo.SaveTFMessage(tfm);
                //}



            //}

            return View("Play");
        }

        //public ActionResult WriteItemsFromMemoryToDatabase()
        //{

        //    IDbStaticItemRepository repo = new EFDbStaticItemRepository();
        //    foreach (StaticItem i in ItemStatics.GetStaticItem)
        //    {

        //        DbStaticItem ni = new DbStaticItem
        //        {
        //            dbName = i.dbName,
        //            FriendlyName = i.FriendlyName,
        //            Description = i.Description,
        //            PortraitUrl = i.PortraitUrl,
        //            MoneyValue = i.MoneyValue,
        //            ItemType = i.ItemType,
        //            UseCooldown = i.UseCooldown,
        //            Findable = i.Findable,
        //            FindWeight = i.FindWeight,
        //            GivesEffect = i.GivesEffect,
        //            IsUnique = false,

        //            HealthBonusPercent = i.HealthBonusPercent,
        //            ManaBonusPercent = i.ManaBonusPercent,
        //            ExtraSkillCriticalPercent = i.ExtraSkillCriticalPercent,
        //            HealthRecoveryPerUpdate = i.HealthRecoveryPerUpdate,
        //            ManaRecoveryPerUpdate = i.ManaRecoveryPerUpdate,
        //            SneakPercent = i.SneakPercent,
        //            EvasionPercent = i.EvasionPercent,
        //            EvasionNegationPercent = i.EvasionNegationPercent,
        //            MeditationExtraMana = i.MeditationExtraMana,
        //            CleanseExtraHealth = i.CleanseExtraHealth,
        //            MoveActionPointDiscount = i.MoveActionPointDiscount,
        //            SpellExtraTFEnergyPercent = i.SpellExtraTFEnergyPercent,
        //            SpellExtraHealthDamagePercent = i.SpellExtraHealthDamagePercent,
        //            CleanseExtraTFEnergyRemovalPercent = i.CleanseExtraTFEnergyRemovalPercent,
        //            SpellMisfireChanceReduction = i.SpellMisfireChanceReduction,
        //            SpellHealthDamageResistance = i.SpellHealthDamageResistance,
        //            SpellTFEnergyDamageResistance = i.SpellTFEnergyDamageResistance,
        //            ExtraInventorySpace = i.ExtraInventorySpace,

        //        };
        //        repo.SaveDbStaticItem(ni);
        //    }

        //    return View("Play");
        //}

        //public ActionResult WriteEffectsFromMemoryToDatabase()
        //{
        //    IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();

        //    foreach (StaticEffect ramEffect in EffectStatics.GetStaticEffect)
        //    {
        //        DbStaticEffect effect = new DbStaticEffect
        //        {
        //            dbName = ramEffect.dbName,
        //            FriendlyName = ramEffect.FriendlyName,
        //            AvailableAtLevel = ramEffect.AvailableAtLevel,
        //            Description = ramEffect.Description,
        //            PreRequesite = ramEffect.PreRequesite,
        //            isLevelUpPerk = ramEffect.isLevelUpPerk,
        //            Cooldown = ramEffect.Cooldown,
        //            Duration = ramEffect.Duration,
        //            ObtainedAtLocation = ramEffect.ObtainedAtLocation,

        //            AttackerWhenHit = ramEffect.AttackerWhenHit,
        //            AttackerWhenHit_M = ramEffect.AttackerWhenHit_M,
        //            AttackerWhenHit_F = ramEffect.AttackerWhenHit_F,

        //            MessageWhenHit = ramEffect.MessageWhenHit,
        //            MessageWhenHit_M = ramEffect.MessageWhenHit_M,
        //            MessageWhenHit_F = ramEffect.MessageWhenHit_F,

        //            HealthBonusPercent = ramEffect.HealthBonusPercent,
        //            ManaBonusPercent = ramEffect.ManaBonusPercent,
        //            ExtraSkillCriticalPercent = ramEffect.ExtraSkillCriticalPercent,
        //            HealthRecoveryPerUpdate = ramEffect.HealthRecoveryPerUpdate,
        //            ManaRecoveryPerUpdate = ramEffect.ManaRecoveryPerUpdate,
        //            SneakPercent = ramEffect.SneakPercent,
        //            EvasionPercent = ramEffect.EvasionPercent,
        //            EvasionNegationPercent = ramEffect.EvasionNegationPercent,
        //            MeditationExtraMana = ramEffect.MeditationExtraMana,
        //            CleanseExtraHealth = ramEffect.CleanseExtraHealth,
        //            MoveActionPointDiscount = ramEffect.MoveActionPointDiscount,
        //            SpellExtraTFEnergyPercent = ramEffect.SpellExtraTFEnergyPercent,
        //            SpellExtraHealthDamagePercent = ramEffect.SpellExtraHealthDamagePercent,
        //            CleanseExtraTFEnergyRemovalPercent = ramEffect.CleanseExtraTFEnergyRemovalPercent,
        //            SpellMisfireChanceReduction = ramEffect.SpellMisfireChanceReduction,
        //            SpellHealthDamageResistance = ramEffect.SpellHealthDamageResistance,
        //            SpellTFEnergyDamageResistance = ramEffect.SpellTFEnergyDamageResistance,
        //            ExtraInventorySpace = ramEffect.ExtraInventorySpace,

        //        };

        //        effectRepo.SaveDbStaticEffect(effect);
        //    }

        //    return View("Play");
        //}

        public ActionResult ViewServerLog(int turn)
        {
            IServerLogRepository serverLogRepo = new EFServerLogRepository();
            ServerLog log = serverLogRepo.ServerLogs.FirstOrDefault(t => t.TurnNumber == turn);
            return View(log);
        }

        public ActionResult ViewUpdateLogs()
        {
            IServerLogRepository serverLogRepo = new EFServerLogRepository();
            return View(serverLogRepo.ServerLogs);
        }

        public ActionResult WriteFaeEncounter()
        {

            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }


            FairyChallengeBag output = new FairyChallengeBag();

            try
            {
                // load data from the xml
                string filename = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/FairyChallengeText/fae_temp.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeBag));
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                output = (FairyChallengeBag)reader.Deserialize(file);
            }
            catch
            {

            }


            return View(output);
        }

        public ActionResult LoadSpecificEncounter(string filename)
        {

            FairyChallengeBag output = new FairyChallengeBag();

            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            // TODO:  finish this!
            return View("WriteFaeEncounter", output);
        }

        [ValidateInput(false)] 
        public ActionResult WriteFaeEncounterSend(FairyChallengeBag input)
        {

            // assert only admins can view this
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("Play", "PvP");
            }

            string path = Server.MapPath("~/XMLs/FairyChallengeText/");

           // input.title = "Serialization Overview";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeBag));

            System.IO.StreamWriter file = new System.IO.StreamWriter(
                path + "fae_temp.xml");
            writer.Serialize(file, input);
            file.Close();

            ViewBag.Result = "done";

            ViewBag.IntroText = input.IntroText.Replace("[", "<").Replace("]", ">");
            ViewBag.FailureText = input.FailureText.Replace("[", "<").Replace("]", ">");
            ViewBag.CorrectFormText = input.CorrectFormText.Replace("[", "<").Replace("]", ">");

       

            return View("WriteFaeEncounter", input);
        }

        [Authorize]
        public ActionResult ResetAllPlayersWithIPAddress(string address)
        {

            // assert only admin can view this
            bool iAmModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (iAmModerator == false)
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }

           
            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<Player> players = playerRepo.Players.Where(p => p.IpAddress == address).ToList();
            foreach (Player p in players)
            {
                p.IpAddress = "reset";
                playerRepo.SavePlayer(p);
           }

            return View("Play");
        
        }

        [Authorize]
        public ActionResult ToggleBanOnGlobalChat(int id)
        {

            // assert only admin can view this
            bool iAmModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (iAmModerator == false)
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }


            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bannedPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == id);

            if (bannedPlayer.IsBannedFromGlobalChat == true)
            {
                
                bannedPlayer.IsBannedFromGlobalChat = false;
                TempData["Result"] = "Ban has been LIFTED.";
            }
            else
            {
                bannedPlayer.IsBannedFromGlobalChat = true;
                TempData["Result"] = "Player has been banned from global chat.";
            }
            playerRepo.SavePlayer(bannedPlayer);

            return RedirectToAction("Play", "PvP");

        }

        public ActionResult Scratchpad()
        {
            return View();
        }

        public ActionResult AuditDonators()
        {

            // assert only admin can view this
            bool iAmAdmin= User.IsInRole(PvPStatics.Permissions_Admin);
            if (iAmAdmin == false)
            {
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }

            string output = "";

            IDonatorRepository repo = new EFDonatorRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            foreach (Donator d in repo.Donators.ToList())
            {
                Player player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == d.OwnerMembershipId);

                if (player != null && player.DonatorLevel > 0)
                {
                    output += "Looking at " + player.GetFullName() + ".";

                    if (player.DonatorLevel > d.Tier)
                    {
                        player.DonatorLevel = d.Tier;
                        output += "  Knocking down to tier " + d.Tier + ".  </br>";
                        string message = "<span class='bad'>MESSAGE FROM SERVER:  Your Patreon donation tier has been changed to " + d.Tier + ".  If you feel this is in error, please send a private message to Judoo on the forums or through Patreon.  Thank you for your past support!</span>";
                        PlayerLogProcedures.AddPlayerLog(player.Id, message, true);
                    }
                    else
                    {
                        output += "  Okay at tier " + d.Tier + ".  </br>";
                        string message = "<span class='good'>MESSAGE FROM SERVER:  Your Patreon donation has been processed and remains at Tier " + d.Tier + ".  Thank you for your support!</span>";
                        PlayerLogProcedures.AddPlayerLog(player.Id, message, true);
                    }

                    playerRepo.SavePlayer(player);

                }

               

            }

            TempData["Result"] = output;
            return RedirectToAction("Play", "PvP");
        }

        public ActionResult Killswitch()
        {
            if (User.IsInRole(PvPStatics.Permissions_Killswitcher) == true)
            {
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.LastUpdateTimestamp = stats.LastUpdateTimestamp.AddDays(1);
                repo.SavePvPWorldStat(stats);

                IPlayerLogRepository logRepo = new EFPlayerLogRepository();
                Player me = PlayerProcedures.GetPlayerFromMembership(69);
                PlayerLog newlog = new PlayerLog
                {
                    IsImportant = true,
                    Message = "<span class='bad'><b>" + WebSecurity.CurrentUserName + " activated the game pause killswitch.</b></span>",
                    PlayerId = me.Id,
                    Timestamp = DateTime.UtcNow,
                };
                logRepo.SavePlayerLog(newlog);

            }
            return RedirectToAction("Play", "PvP");
        }

        public ActionResult KillswitchRestore()
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == true)
            {
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.LastUpdateTimestamp = DateTime.UtcNow;
                repo.SavePvPWorldStat(stats);
            }
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult FindMissingThumbnails()
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == true)
            {
                return View();
            }
            else
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }
        }

        [Authorize]
        public ActionResult ViewPlayerItems(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == true || User.IsInRole(PvPStatics.Permissions_Moderator) == true)
            {
                ViewBag.playeritems = ItemProcedures.GetAllPlayerItems(id).OrderByDescending(i => i.dbItem.Level);
                ViewBag.player = PlayerProcedures.GetPlayerFormViewModel(id);
                return View();
            }
            else
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }
        }

        [Authorize]
        public ActionResult ViewItemTransferLog(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == true || User.IsInRole(PvPStatics.Permissions_Moderator) == true)
            {
                ViewBag.item = ItemProcedures.GetItemViewModel(id);
                ViewBag.transferlog = ItemTransferLogProcedures.GetItemTransferLog(id);
                return View();
            }
            else
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View("~/Views/PvP/PvPAdmin.cshtml");
            }
        }

    }


}
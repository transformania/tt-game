using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.Models;
using tfgame.Procedures;
using Microsoft.AspNet.Identity;

namespace tfgame.Controllers
{

    [Authorize]
    public class TFWorldController : Controller
    {
        //
        // GET: /TFWorld/

        public ActionResult Play()
        {

            if (System.Web.HttpContext.Current.Application["main"] == null)
            {
                Procedures.Procedures.LoadGameIntoMemory();
                Procedures.Procedures.LoadCharacterIntoMemory(((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));

            }

            return View();
        }

        public string GetLog()
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            if (System.Web.HttpContext.Current.Application["main"] == null)
            {
                //Procedures.Procedures.LoadGameIntoMemory();
               // Procedures.Procedures.LoadCharacterIntoMemory();
                
            }

            Procedures.Procedures.LoadCharacterIntoMemory(myMembershipId);

            Game timelog = System.Web.HttpContext.Current.Application["main"] as Game;

            string output = "";

            Character me = Procedures.Procedures.GetCharacter(myMembershipId);
            Scene scene = timelog.Scenes.FirstOrDefault(s => s.dbName == me.AtScene);

            foreach (string s in scene.SceneLog.Select(s => s.Message))
            {
                output += s;
            }

            return output;
        }

        public string AjaxTestChat(string statement)
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            int LogMaxSize = 20;

            Game chatlog = System.Web.HttpContext.Current.Application["main"] as Game;
            TimeMessage newPost = new TimeMessage
            {
                Timestamp = DateTime.UtcNow,
                Message = User.Identity.Name + ":<tiny>" + DateTime.UtcNow.ToString("HH:mm:ss") + ":</tiny>  <b>" + statement + " </b> <br>"
            };

            System.Web.HttpContext.Current.Application["main"] = chatlog;

            Scene scene = Procedures.Procedures.GetScene(myMembershipId);

            string output = "";
            foreach (string s in scene.SceneLog.Select(l => l.Message))
            {
                output += s;
            }

            scene.SceneLog.Add(newPost);

            int logSize = scene.SceneLog.Count();
            if (logSize > LogMaxSize)
            {
                scene.SceneLog = scene.SceneLog.Skip(logSize - LogMaxSize).ToList();
            }
            return output;
        }

        public void SaveUserToDatabase()
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            Character me = Procedures.Procedures.GetCharacter(myMembershipId);
            ICharacterRepository characterRepo = new EFCharacterRepository();
            
            
            dbModels.Models.Character dbMe = new dbModels.Models.Character();

            dbModels.Models.Character oldDbMe = characterRepo.Characters.FirstOrDefault(c => c.SimpleMembershipId == myMembershipId);

            if (oldDbMe == null)
            {
                oldDbMe = new dbModels.Models.Character();
            }
            oldDbMe.Health = me.Health;
            oldDbMe.HealthMax = me.HealthMax;
            oldDbMe.Mana = me.Mana;
            oldDbMe.ManaMax = me.ManaMax;
            oldDbMe.Name = me.Name;
            oldDbMe.SimpleMembershipId = me.Id;
            oldDbMe.Form = me.Form.FormName;
            oldDbMe.LastDbSave = DateTime.UtcNow;
            oldDbMe.AtScene = me.AtScene;

            characterRepo.SaveCharacter(oldDbMe);
        }

        public JsonResult GetSceneInfo()
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;

            Scene thisScene = Procedures.Procedures.GetScene(myMembershipId);
            SceneData output = new SceneData();

            output.Connections = thisScene.Connections;
            output.SceneName = thisScene.CasualName;
            output.Img = thisScene.Img;
            output.Characters = Procedures.Procedures.GetCharactersHere(myMembershipId);
            output.Me = Procedures.Procedures.GetCharacter(myMembershipId);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Move(string direction)
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = Procedures.Procedures.GetCharacter(myMembershipId);
            Scene here = Procedures.Procedures.GetScene(myMembershipId);
            Scene there = Procedures.Procedures.GetScene(direction);
            SelfQuery output = new SelfQuery();

            output.pLog = new List<PlogEntry>();

            // STEP 1:  Check to see that we pass all of the entry requirements
            List<BooleanPlogEntry> conditionResults = Procedures.Procedures.MeetsRequirements(there.EntryRequirements, myMembershipId);

            foreach (BooleanPlogEntry bentry in conditionResults)
            {
                output.pLog.Add(bentry.Plog);
            }

            // move success; move this character!
            if (Procedures.Procedures.DidAllRequirementsPass(conditionResults) == true)
            {
                Procedures.Procedures.RunEntryEvents(there, output.pLog, "pass", myMembershipId);
                // move character and trigger any entry events
                me.AtScene = direction;
                Procedures.Procedures.RunEntryEvents(there, output.pLog, myMembershipId);
            }
            else
            {
                Procedures.Procedures.RunEntryEvents(there, output.pLog, "fail", myMembershipId);
            }

            return Json(output, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RefreshCharactersItems()
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            List<Character> output = Procedures.Procedures.GetCharactersHere(myMembershipId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CharacterQuery(string character)
        {
            Character person = Procedures.Procedures.GetCharacter(character);

            return Json(person, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelfQuery()
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            SelfQuery output = new SelfQuery();

            Character me = Procedures.Procedures.GetCharacter(myMembershipId);

            output.character = me;
            output.pLog = new List<PlogEntry>();

            output.pLog.Add(new PlogEntry
            {
                Message = "-> Your name is " + me.Name + " and you are currently a " + me.Form.FormNameCasual + ".<br><br>-> " + me.Form.Description1st + "<br><br>",
                //Img = "woman01.jpg"
            });


            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LookAtScene()
        {
            int myMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            Scene here = Procedures.Procedures.GetScene(myMembershipId);
            SceneImgDescription output = new SceneImgDescription();
            output.Img = here.Img;
            output.Description = "-> " + here.Description + "<br><br>";
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public void WriteScenesToXML()
        {
            Procedures.Procedures.WriteScenesToXML();
        }

    }
}

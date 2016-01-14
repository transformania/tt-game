using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.Models;
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
                string myMembershipId = User.Identity.GetUserId();
                Procedures.Procedures.LoadGameIntoMemory();
                Procedures.Procedures.LoadCharacterIntoMemory(myMembershipId);

            }

            return View();
        }

        public string GetLog()
        {
            string myMembershipId = User.Identity.GetUserId();
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
            string myMembershipId = User.Identity.GetUserId();
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
            string myMembershipId = User.Identity.GetUserId();
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
            string myMembershipId = User.Identity.GetUserId();
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
            string myMembershipId = User.Identity.GetUserId();
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = Procedures.Procedures.GetCharacter(myMembershipId);
            Scene here = Procedures.Procedures.GetScene(myMembershipId);
            Scene there = Procedures.Procedures.GetSceneByName(direction);
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
            string myMembershipId = User.Identity.GetUserId();
            List<Character> output = Procedures.Procedures.GetCharactersHere(myMembershipId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CharacterQuery(string character)
        {
            Character person = Procedures.Procedures.GetCharacterByName(character);

            return Json(person, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelfQuery()
        {
            string myMembershipId = User.Identity.GetUserId();
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
            string myMembershipId = User.Identity.GetUserId();
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

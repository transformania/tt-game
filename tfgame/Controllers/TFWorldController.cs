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
using WebMatrix.WebData;

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
                Procedures.Procedures.LoadCharacterIntoMemory();

            }

            return View();
        }

        public string GetLog()
        {

            if (System.Web.HttpContext.Current.Application["main"] == null)
            {
                //Procedures.Procedures.LoadGameIntoMemory();
               // Procedures.Procedures.LoadCharacterIntoMemory();
                
            }

            Procedures.Procedures.LoadCharacterIntoMemory();

            Game timelog = System.Web.HttpContext.Current.Application["main"] as Game;

            string output = "";

            Character me = Procedures.Procedures.GetCharacter();
            Scene scene = timelog.Scenes.FirstOrDefault(s => s.dbName == me.AtScene);

            foreach (string s in scene.SceneLog.Select(s => s.Message))
            {
                output += s;
            }

            return output;
        }

        public string AjaxTestChat(string statement)
        {

            int LogMaxSize = 20;

            Game chatlog = System.Web.HttpContext.Current.Application["main"] as Game;
            TimeMessage newPost = new TimeMessage
            {
                Timestamp = DateTime.UtcNow,
                Message = WebSecurity.CurrentUserName + ":<tiny>" + DateTime.UtcNow.ToString("HH:mm:ss") + ":</tiny>  <b>" + statement + " </b> <br>"
            };

            System.Web.HttpContext.Current.Application["main"] = chatlog;

            Scene scene = Procedures.Procedures.GetScene();

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
            Character me = Procedures.Procedures.GetCharacter();
            ICharacterRepository characterRepo = new EFCharacterRepository();
            
            
            dbModels.Models.Character dbMe = new dbModels.Models.Character();

            dbModels.Models.Character oldDbMe = characterRepo.Characters.FirstOrDefault(c => c.SimpleMembershipId == WebSecurity.CurrentUserId);

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
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;

            Scene thisScene = Procedures.Procedures.GetScene();
            SceneData output = new SceneData();

            output.Connections = thisScene.Connections;
            output.SceneName = thisScene.CasualName;
            output.Img = thisScene.Img;
            output.Characters = Procedures.Procedures.GetCharactersHere();
            output.Me = Procedures.Procedures.GetCharacter();

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Move(string direction)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = Procedures.Procedures.GetCharacter();
            Scene here = Procedures.Procedures.GetScene();
            Scene there = Procedures.Procedures.GetScene(direction);
            SelfQuery output = new SelfQuery();

            output.pLog = new List<PlogEntry>();

            // STEP 1:  Check to see that we pass all of the entry requirements
            List<BooleanPlogEntry> conditionResults = Procedures.Procedures.MeetsRequirements(there.EntryRequirements);

            foreach (BooleanPlogEntry bentry in conditionResults)
            {
                output.pLog.Add(bentry.Plog);
            }

            // move success; move this character!
            if (Procedures.Procedures.DidAllRequirementsPass(conditionResults) == true)
            {
                Procedures.Procedures.RunEntryEvents(there, output.pLog, "pass");
                // move character and trigger any entry events
                me.AtScene = direction;
                Procedures.Procedures.RunEntryEvents(there, output.pLog);
            }
            else
            {
                Procedures.Procedures.RunEntryEvents(there, output.pLog, "fail");
            }

            return Json(output, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RefreshCharactersItems()
        {
            List<Character> output = Procedures.Procedures.GetCharactersHere();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CharacterQuery(string character)
        {
            Character person = Procedures.Procedures.GetCharacter(character);

            return Json(person, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelfQuery()
        {
            SelfQuery output = new SelfQuery();

            Character me = Procedures.Procedures.GetCharacter();

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
            Scene here = Procedures.Procedures.GetScene();
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

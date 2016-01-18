using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.Models;

namespace tfgame.Procedures
{
    public static class Procedures
    {
        public static void LoadGameIntoMemory()
        {

            Game game = new Game();
            game.Scenes = new List<Scene>();
            game.Characters = new List<Character>();
            game.Forms = new List<Form>();

            LoadScenesFromXML(game);
            LoadConnectionsFromXML(game);
            LoadFormsFromXML(game);

            System.Web.HttpContext.Current.Application["main"] = game;

        }

        public static void LoadCharacterIntoMemory(string membershipId)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = game.Characters.FirstOrDefault(c => c.Id == membershipId);
            if (me == null)
            {
                me = new Character();
                ICharacterRepository characterRepo = new EFCharacterRepository();
                dbModels.Models.Character dbme = characterRepo.Characters.FirstOrDefault(c => c.SimpleMembershipId == membershipId);

                // character is in the database, so load up stats from there
                if (dbme != null)
                {

                    me.Id = dbme.SimpleMembershipId;
                    me.Health = dbme.Health;
                    me.HealthMax = dbme.HealthMax;
                    me.Mana = dbme.Mana;
                    me.ManaMax = dbme.ManaMax;
                    me.Name = dbme.Name;
                    me.Form = game.Forms.FirstOrDefault(f => f.FormName == dbme.Form);
                    me.AtScene = dbme.AtScene;

                }

                    // player is not in memory nor has a presence in the database, so start a new one from scratch
                else
                {
                    me.Id = membershipId;
                    me.Health = 100;
                    me.HealthMax = 100;
                    me.Mana = 100;
                    me.ManaMax = 100;
                    me.Name = "No name";
                    me.Form = game.Forms.FirstOrDefault(f => f.FormName == "plainguy_01");
                    me.Form.FormName = "plain";
                    me.AtScene = "EastRoom";
                }

                game.Characters.Add(me);
            }

            // player is alread


        }



        public static Character GetCharacter(string membershipId)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = game.Characters.FirstOrDefault(c => c.Id == membershipId);
            return me;
        }

        public static Character GetCharacterByName(string Name)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character character = game.Characters.FirstOrDefault(c => c.Name == Name);
            return character;
        }

        public static Scene GetScene(string membershipId)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;

            string sceneName = game.Characters.FirstOrDefault(c => c.Id == membershipId).AtScene;
            Scene thisScene = game.Scenes.FirstOrDefault(s => s.dbName == sceneName);
            thisScene.Connections = game.Connections.Where(c => c.ParentSceneDbName == thisScene.dbName).ToList();
            //thisScene.Img = 

            return thisScene;
        }

        public static Scene GetSceneByName(string dbname)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Scene thisScene = game.Scenes.FirstOrDefault(s => s.dbName == dbname);
            return thisScene;
        }

        public static void SaveCharacter(Character character, string membershipId)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = game.Characters.FirstOrDefault(c => c.Id == membershipId);
        }

        public static void AddMessageToScene(Scene scene, string message)
        {
            // Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            TimeMessage newPost = new TimeMessage
            {
                Timestamp = DateTime.UtcNow,
                Message = " <tiny>" + DateTime.UtcNow.ToString("HH:mm:ss") + "</tiny> " + message + "<br>",
            };
            scene.SceneLog.Add(newPost);

            int LogMaxSize = 25;

            int logSize = scene.SceneLog.Count();
            if (logSize > LogMaxSize)
            {
                scene.SceneLog = scene.SceneLog.Skip(logSize - LogMaxSize).ToList();
            }
        }

        public static List<Character> GetCharactersHere(string membershipId)
        {
            return GetCharactersHere(membershipId, GetScene(membershipId));
        }

        public static List<Character> GetCharactersHere(string membershipId, Scene scene)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            return game.Characters.Where(c => c.AtScene == scene.dbName && c.Id != membershipId).ToList();
        }

        public static void WriteScenesToXML()
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            List<Scene> scenes = game.Scenes;

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Scene>));

            // Bitmap mapx = new Bitmap(Image.FromFile(Server.MapPath(@"~\Images\" + mName)));

            System.IO.StreamWriter file = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath(@"~/Saved XMLS/scenes.xml"));
            writer.Serialize(file, scenes);
            file.Close();

            string path = HttpContext.Current.Server.MapPath(@"~/Saved XMLS");
        }

        private static void LoadScenesFromXML(Game game)
        {
            IEnumerable<Scene> restoredScenes;
            var serializer = new XmlSerializer(typeof(List<Scene>));
            using (var reader = XmlReader.Create((HttpContext.Current.Server.MapPath(@"~/Saved XMLS/scenes.xml"))))
            {
                restoredScenes = (List<Scene>)serializer.Deserialize(reader);
            }

            //Scene testScene = restoredScenes.FirstOrDefault(s => s.dbName == "Bathroom");

            //Requirement req = testScene.EntryRequirements.First();
            //req.FailMessage = new PlogEntry
            //{
            //    Message = "For some reason you can't get inside the bathroom."
            //};

            //testScene.EntryRequirements = new List<Requirement>();
            //testScene.EntryRequirements.Add(new Requirement
            //{
            //    Type = Statics.RequirementTypes.Form,
            //    Value = "plainguy_01",
            //    PassMessage = new PlogEntry
            //    {
            //        Message = "You enter the bathroom."
            //    }

            //});

            game.Scenes = restoredScenes.ToList();
        }

        private static void LoadConnectionsFromXML(Game game)
        {
            IEnumerable<Connection> restoredConnections;
            var serializer = new XmlSerializer(typeof(List<Connection>));
            using (var reader = XmlReader.Create((HttpContext.Current.Server.MapPath(@"~/Saved XMLS/connections.xml"))))
            {
                restoredConnections = (List<Connection>)serializer.Deserialize(reader);
            }

            game.Connections = restoredConnections.ToList();
        }

        private static void LoadFormsFromXML(Game game)
        {
            IEnumerable<Form> restoredForms;
            var serializer = new XmlSerializer(typeof(List<Form>));
            using (var reader = XmlReader.Create((HttpContext.Current.Server.MapPath(@"~/Saved XMLS/forms.xml"))))
            {
                restoredForms = (List<Form>)serializer.Deserialize(reader);
            }

            game.Forms = restoredForms.ToList();
        }

        public static List<BooleanPlogEntry> MeetsRequirements(List<Requirement> requirements, string membershipId)
        {

            List<BooleanPlogEntry> output = new List<BooleanPlogEntry>();

            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = GetCharacter(membershipId);

            // no requirements; we're good here.
            if (requirements == null)
            {
                return output;
            }
            else
            {

                foreach (Requirement req in requirements)
                {
                    BooleanPlogEntry data = MeetsRequirement(req, membershipId);
                    output.Add(data);

                }

            }

            return output;


        }

        public static BooleanPlogEntry MeetsRequirement(Requirement requirement, string membershipId)
        {

            BooleanPlogEntry output = new BooleanPlogEntry();

            Game game = System.Web.HttpContext.Current.Application["main"] as Game;
            Character me = GetCharacter(membershipId);

            // FORM TYPE
            if (requirement.Type == Statics.RequirementTypes.Form)
            {

                // pass
                if (me.Form.FormName == requirement.Value)
                {
                    output.Passed = true;
                    output.Plog = requirement.PassMessage;
                }

                // fail
                else
                {
                    output.Passed = false;
                    output.Plog = requirement.FailMessage;
                }
            }

            // TRAIT TYPE
            else if (requirement.Type == Statics.RequirementTypes.Trait)
            {
                if (me.Form.Traits.Contains(requirement.Value) == true)
                {

                }
            }


            // finally, check inverse:
            if (requirement.Inverse == true)
            {
                output.Passed = !output.Passed;

                if (output.Passed == true)
                {
                    output.Plog = requirement.PassMessage;
                }
                else
                {
                    output.Plog = requirement.FailMessage;
                }

            }

            return output;
        }

        public static Boolean DidAllRequirementsPass(List<BooleanPlogEntry> requirements)
        {
            foreach (BooleanPlogEntry plogentry in requirements)
            {
                if (plogentry.Passed == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static List<PlogEntry> RunEntryEvents(Scene scene, List<PlogEntry> log, string membershipId)
        {
            List<BooleanPlogEntry> blog = new List<BooleanPlogEntry>();
            Boolean passed = true;

            // for each event for the scene we are moving to that has the "OnEnter" type
            foreach (Event evt in scene.Events.Where(e => e.Trigger.TriggerType == Statics.TriggerTypes.OnEnter))
            {

                foreach (Requirement req in evt.Requirements)
                {

                    BooleanPlogEntry result = MeetsRequirement(req, membershipId);

                    // condition has passed
                    if (result.Passed == true)
                    {
                        blog.Add(result);
                    }

                    // condition has failed, so break out of loop
                    else
                    {
                        passed = false;
                        break;
                    }

                }

                // conditions have passed so execute success actions
                if (passed == true)
                {
                    ExecuteActions(evt, "pass", blog, membershipId);
                } else {
                     ExecuteActions(evt, "fail", blog, membershipId);
                }

            }

            // convert our boolean log 

            foreach (BooleanPlogEntry bpe in blog)
            {
                log.Add(bpe.Plog);
            }

            return log;
        }

        public static List<BooleanPlogEntry> ExecuteActions(Event evt, string passOrFail, List<BooleanPlogEntry> log, string membershipId)
        {
        
            List<tfgame.Models.Action> actionsToExecute;

            if (passOrFail == "pass")
            {
                actionsToExecute = evt.PassActions;
            }
            else
            {
                actionsToExecute = evt.FailActions;
            }

            foreach (tfgame.Models.Action action in actionsToExecute)
            {

                List<BooleanPlogEntry> results = ExecuteAction(action, log, membershipId);

                ////List<BooleanPlogEntry> result = ExecuteAction(action, log);
                //foreach (BooleanPlogEntry message in results)
                //{
                //    log.Add(message);
                //}

            }

            return log;
        }

        public static List<BooleanPlogEntry> ExecuteAction(tfgame.Models.Action action, List<BooleanPlogEntry> log, string membershipId)
        {
            Game game = System.Web.HttpContext.Current.Application["main"] as Game;

            // TODO:  Check requirements

            switch (action.Type)
            {
                case Statics.ActionTypes.SetForm:
                    {
                        Character me = GetCharacter(membershipId);
                        me.Form = game.Forms.FirstOrDefault(f => f.FormName == action.Value);
                        break;
                    }
                case Statics.ActionTypes.AddItem:
                    {
                        break;
                    }
            }

            // if passed
            BooleanPlogEntry logAddition = new BooleanPlogEntry
            {
                Passed = true,
                Plog = action.PassMessage
            };

            log.Add(logAddition);

            return log;
        }

        public static List<PlogEntry> RunEntryEvents(Scene scene, List<PlogEntry> log, string passOrFail, string membershipId)
        {

            List<BooleanPlogEntry> blog = new List<BooleanPlogEntry>();
            //foreach (PlogEntry entry in log)
            //{
            //    blog.Add(new BooleanPlogEntry {
            //        Passed = true,
            //        Plog = entry
            //    });
            //}

            BooleanPlogEntry logAddition = new BooleanPlogEntry
            {
                Passed = true,
               // Plog = action.PassMessage
            };

            // execute every action of success actions
            if (passOrFail == "pass")
            {
                foreach (tfgame.Models.Action action in scene.EntrySuccessActions)
                {
                    ExecuteAction(action, blog, membershipId);
                }
            }
            else
            {
                foreach (tfgame.Models.Action action in scene.EntryFailActions)
                {
                    ExecuteAction(action, blog, membershipId);
                }
            }

            foreach (BooleanPlogEntry bentry in blog)
            {
                log.Add(bentry.Plog);
            }

            return log;
        }

    }

}
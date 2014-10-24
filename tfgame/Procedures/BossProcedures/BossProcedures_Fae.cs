using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_Fae
    {
        public static void SpawnFae()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");

            if (fae == null)
            {
                fae = new Player()
                {
                    FirstName = "Jewdewfae",
                    LastName = "the Pervfae",
                    ActionPoints = 120,
                    dbLocationName = "forest_hotspring",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_Perverted_Fairy_Judoo",
                    IsPetToId = -1,
                    Money = 1000,
                    Mobility = "full",
                    Level = 7,
                    MembershipId = -6,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(fae);

                fae = PlayerProcedures.ReadjustMaxes(fae, ItemProcedures.GetPlayerBuffs(fae));

                playerRepo.SavePlayer(fae);

                // give fae the fairy spell to counterattack with
                fae = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");
                DbStaticSkill skillToAdd = SkillStatics.GetStaticSkill("hey_listed_Varn");

                SkillProcedures.GiveSkillToPlayer(fae.Id, skillToAdd);

                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective directive = new AIDirective
                {
                    OwnerId = fae.Id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                    sVar1 = ";", // this is used to keep track of which players have interacted with her by appending their id to this string
                    Var1 = 0, // this keeps track of how many people she has played with in the current location
                    Var2 = PvPWorldStatProcedures.GetWorldTurnNumber() // this stores the turn number of Jewdewfae's last move
                };

                aiRepo.SaveAIDirective(directive);

            }
        }

        public static void MoveToNewLocation()
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");

            FairyChallengeViewModel fairyChallenges = new FairyChallengeViewModel();
            fairyChallenges.FairyChallengeBags = new List<FairyChallengeBag>();

            // load data from the xml
            string filename = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/FairyChallengeText/template.xml");
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeViewModel));
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            fairyChallenges = (FairyChallengeViewModel)reader.Deserialize(file);

            List<string> fairyLocations = fairyChallenges.FairyChallengeBags.Where(f => f.dbLocationName != fae.dbLocationName).Select(f => f.dbLocationName).ToList();
            double max = fairyLocations.Count();
            Random rand = new Random();
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            string newLocation = fairyLocations.ElementAt(index);

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "<b>Jewdewfae got bored and flew away from here.</b>");

            fae.dbLocationName = newLocation;
            playerRepo.SavePlayer(fae);

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "<b>Jewdewfae flew here.  She looks bored and wants to play with someone.</b>");

            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);
            directive.Var1 = 0;
            directive.Var2 = PvPWorldStatProcedures.GetWorldTurnNumber();
            directive.sVar1 = ";";
            aiRepo.SaveAIDirective(directive);


        }

        public static decimal AddInteraction(Player player) {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");

            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);


            if (directive == null)
            {
                directive = new AIDirective
                {
                    OwnerId = fae.Id,
                    Timestamp = DateTime.UtcNow,
                    sVar1 = ";",
                };
            }

            // Var1 will keep track how how many interactions Jewdewfae has had.  Award a bit less XP based on how many people she has played with down to 15 at lowest
            decimal xpGain = 50 - directive.Var1 * 2;

            if (xpGain < 15) {
                xpGain = 15;
            }

            directive.Var1 += 1;

            // add this player's ID to the list of people interacted with; one playtime per location!
            directive.sVar1 += player.Id.ToString() + ";";

            aiRepo.SaveAIDirective(directive);

            if (directive.Var1 >= 18) {
                MoveToNewLocation();
            }

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "<b>Jewdewfae played with " + player.FirstName + " " + player.LastName + " here.</b>");

            return xpGain;

        }

        public static FairyChallengeBag GetFairyChallengeInfoAtLocation(string location)
        {

            FairyChallengeViewModel fairyChallenges = new FairyChallengeViewModel();
            fairyChallenges.FairyChallengeBags = new List<FairyChallengeBag>();

            // load data from the xml
            string filename = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/FairyChallengeText/template.xml");
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeViewModel));
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            fairyChallenges = (FairyChallengeViewModel)reader.Deserialize(file);

            return fairyChallenges.FairyChallengeBags.FirstOrDefault(f => f.dbLocationName == location);
        }

        public static bool PlayerHasHadRecentInteraction(Player player, Player fae)
        {
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);

            PlayerProcedures.SetTimestampToNow(fae, false);

            if (directive.sVar1.Contains(";" + player.Id + ";"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

      

    }
}
﻿using System;
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
    public static class BossProcedures_Jewdewfae
    {

        public const string FaeFormDbName = "form_Perverted_Fairy_Judoo";
        public const string FirstName = "Jewdewfae";
        public const string LastName = "the Pervfae";

        public static void SpawnFae()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.JewdewfaeBotId);

            if (fae == null)
            {
                fae = new Player()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    ActionPoints = 120,
                    dbLocationName = "apartment_dog_park",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = FaeFormDbName,
                    Money = 1000,
                    Mobility = "full",
                    Level = 7,
                    MembershipId = "-6",
                    BotId = AIStatics.JewdewfaeBotId,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(fae);

                fae = PlayerProcedures.ReadjustMaxes(fae, ItemProcedures.GetPlayerBuffsSQL(fae));

                playerRepo.SavePlayer(fae);

                fae = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.JewdewfaeBotId);

                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective directive = new AIDirective
                {
                    OwnerId = fae.Id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = 0,
                    DoNotRecycleMe = true,
                    sVar1 = ";", // this is used to keep track of which players have interacted with her by appending their id to this string
                    sVar2 = "", // this is used to keep track of which locations Jewdewfae has been in during the past cycle
                    Var1 = 0, // this keeps track of how many people she has played with in the current location
                    Var2 = 0 // this stores the turn number of Jewdewfae's last move
                };

                aiRepo.SaveAIDirective(directive);

            }
        }

        public static void MoveToNewLocation()
        {

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.JewdewfaeBotId);

            IJewdewfaeEncounterRepository faeRepo = new EFJewdewfaeEncounterRepository();

             IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);

            // add fae's current location to the list of places visited
            directive.sVar2 += fae.dbLocationName + ";";

            List<string> fairyLocations = faeRepo.JewdewfaeEncounters.Where(f => f.IsLive == true).Select(f => f.dbLocationName).ToList();

            List<string> visitedFairyLocations = directive.sVar2.Split(';').Where(f => f.Length > 1).ToList();

            List<string> possibleLocations = fairyLocations.Except(visitedFairyLocations).ToList();

            // if there are no locations left to visit, reset
            if (possibleLocations.Count() == 0)
            {
                possibleLocations = fairyLocations;
                directive.sVar2 = "";
            }

            double max = possibleLocations.Count();
            Random rand = new Random();
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            string newLocation = possibleLocations.ElementAt(index);

          
            directive.Var1 = 0;
            directive.Var2 = PvPWorldStatProcedures.GetWorldTurnNumber();
            directive.sVar1 = ";";
           
            aiRepo.SaveAIDirective(directive);

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "<b>Jewdewfae got bored and flew away from here.</b>");

            fae.dbLocationName = newLocation;
            playerRepo.SavePlayer(fae);

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "<b>Jewdewfae flew here.  She looks bored and wants to play with someone.</b>");

            


        }

        public static decimal AddInteraction(Player player) {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.JewdewfaeBotId);

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
            decimal xpGain = 75 - directive.Var1 * 3;

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

        public static JewdewfaeEncounter GetFairyChallengeInfoAtLocation(string location)
        {
            IJewdewfaeEncounterRepository repo = new EFJewdewfaeEncounterRepository();
            return repo.JewdewfaeEncounters.FirstOrDefault(e => e.dbLocationName == location);
        }

        public static bool PlayerHasHadRecentInteraction(Player player, Player fae)
        {
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);

            PlayerProcedures.SetTimestampToNow(fae);

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
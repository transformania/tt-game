﻿using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Jewdewfae
    {

        public const string FaeFormDbName = "form_Perverted_Fairy_Judoo";
        public const string FirstName = "Jewdewfae";
        public const string LastName = "the Pervfae";

        private const int FaeFormId = 210;

        public static void SpawnFae()
        {
            var fae = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.JewdewfaeBotId });

            if (fae == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Location = "apartment_dog_park",
                    Gender = PvPStatics.GenderFemale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = FaeFormId,
                    Money = 1000,
                    Level = 7,
                    BotId = AIStatics.JewdewfaeBotId
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var faeEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                faeEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(faeEF));
                playerRepo.SavePlayer(faeEF);

                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                var directive = new AIDirective
                {
                    OwnerId = id,
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
            var fae = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.JewdewfaeBotId);

            IJewdewfaeEncounterRepository faeRepo = new EFJewdewfaeEncounterRepository();

             IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            var directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);

            // add fae's current location to the list of places visited
            directive.sVar2 += fae.dbLocationName + ";";

            var fairyLocations = faeRepo.JewdewfaeEncounters.Where(f => f.IsLive).Select(f => f.dbLocationName).ToList();

            var visitedFairyLocations = directive.sVar2.Split(';').Where(f => f.Length > 1).ToList();

            var possibleLocations = fairyLocations.Except(visitedFairyLocations).ToList();

            // if there are no locations left to visit, reset
            if (!possibleLocations.Any())
            {
                possibleLocations = fairyLocations;
                directive.sVar2 = "";
            }

            double max = possibleLocations.Count();
            var rand = new Random();
            var num = rand.NextDouble();

            var index = Convert.ToInt32(Math.Floor(num * max));
            var newLocation = possibleLocations.ElementAt(index);

          
            directive.Var1 = 0;
            directive.Var2 = PvPWorldStatProcedures.GetWorldTurnNumber();
            directive.sVar1 = ";";
           
            aiRepo.SaveAIDirective(directive);

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "Jewdewfae got bored and flew away from here.",LogStatics.LOG_TYPE_BOLD);

            fae.dbLocationName = newLocation;
            playerRepo.SavePlayer(fae);

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "Jewdewfae flew here.  She looks bored and wants to play with someone.",LogStatics.LOG_TYPE_BOLD);

            


        }

        public static decimal AddInteraction(Player player) {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var fae = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.JewdewfaeBotId);

            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            var directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);


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
            var xpGain = 75 - directive.Var1 * 3;

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

            LocationLogProcedures.AddLocationLog(fae.dbLocationName, "Jewdewfae played with " + player.FirstName + " " + player.LastName + " here.",LogStatics.LOG_TYPE_BOLD);

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
            var directive = aiRepo.AIDirectives.FirstOrDefault(i => i.OwnerId == fae.Id);

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
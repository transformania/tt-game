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

namespace tfgame.Procedures
{



    public static class CovenantProcedures
    {

        public static bool PlayerIsInCovenant(Player player)
        {
            if (player.Covenant != -1)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public static Covenant GetDbCovenant(int id)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            return covRepo.Covenants.FirstOrDefault(c => c.Id == id);
        }

        public static Covenant GetDbCovenant(string name)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            return covRepo.Covenants.FirstOrDefault(c => c.Name == name);
        }

        public static CovenantViewModel GetCovenantViewModel(Player player)
        {

            return GetCovenantViewModel(player.Covenant);
        }

        public static CovenantViewModel GetCovenantViewModel(int id)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            CovenantViewModel output = new CovenantViewModel();

            output.dbCovenant = covRepo.Covenants.FirstOrDefault(c => c.Id == id);

            // get all players in this covenant
            List<Player> dbplayersInCov = playerRepo.Players.Where(p => p.Covenant == id && p.MembershipId > 0).ToList();

            IEnumerable<PlayerFormViewModel> playerFormList = PlayerProcedures.GetPlayerFormViewModelsInCovenant(id);



            //foreach (Player person in dbplayersInCov)
            //{
            //    PlayerFormViewModel addme = new PlayerFormViewModel();
            //    addme.Player = person;
            //    addme.Form = FormStatics.GetForm(person.Form);
            //    playerFormList.Add(addme);
            //}

            output.Leader = playerRepo.Players.FirstOrDefault(p => p.Id == output.dbCovenant.LeaderId);

            output.Members = playerFormList;


            return output;
        }

        public static void LoadCovenantDictionary()
        {
            ICovenantRepository covRepo = new EFCovenantRepository();

            CovenantDictionary.IdNameFlagLookup.Clear();


            foreach (Covenant c in covRepo.Covenants.Where(c => c.LeaderId > -1))
            {
                CovenantNameFlag temp = new CovenantNameFlag
                {
                    FlagUrl = c.FlagUrl,
                    Name = c.Name,
                    HomeLocation = c.HomeLocation,
                    CovLevel = c.Level,
                };
                CovenantDictionary.IdNameFlagLookup.Add(c.Id, temp);
            }

        }

        public static IEnumerable<Covenant> GetAllCovenants()
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            return covRepo.Covenants.Where(c => c.Id > 0);
        }

        public static string AddPlayerToCovenant(Player player, int covId)
        {
            return AddPlayerToCovenant(player.Id, covId);
        }

        public static string AddPlayerToCovenant(int playerId, int covId)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.Covenant = covId;
            playerRepo.SavePlayer(dbPlayer);

            string covMessage = dbPlayer.GetFullName() + " is now a member of the covenant.";
            WriteCovenantLog(covMessage, covId, true);

            return "";
        }

        public static string StartNewCovenant(Covenant newcov)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant dbCov = new Covenant();
          //  dbCov.IsPvP = newcov.IsPvP;
            dbCov.IsPvP = false;
            dbCov.FounderMembershipId = newcov.FounderMembershipId;
            dbCov.FlagUrl = newcov.FlagUrl;
            dbCov.LastMemberAcceptance = DateTime.UtcNow;
            dbCov.Name = newcov.Name;
            dbCov.SelfDescription = newcov.SelfDescription;
            dbCov.LeaderId = newcov.LeaderId;
            dbCov.Captains = "";

            covRepo.SaveCovenant(dbCov);

            return "";

        }

        public static bool CovenantOfNameExists(string name)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant possible = covRepo.Covenants.FirstOrDefault(c => c.Name == name);

            if (possible != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemovePlayerFromCovenant(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.First(p => p.Id == player.Id);
            dbPlayer.Covenant = -1;
            playerRepo.SavePlayer(dbPlayer);

            // delete the covenant if it is now empty
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant possible = covRepo.Covenants.FirstOrDefault(c => c.Id == player.Covenant);

            int covMemberCount = GetPlayerCountInCovenant(possible, false);

            if (covMemberCount == 0)
            {
                covRepo.DeleteCovenant(possible.Id);

            }

            // the covenant is not empty, so we need to give it a new leader.  For now the new leader is essentially random.
            else if (possible.LeaderId == player.Id)
            {
                Player nextleader = playerRepo.Players.FirstOrDefault(p => p.Covenant == possible.Id);
                possible.LeaderId = nextleader.Id;
                covRepo.SaveCovenant(possible);
            }

            // remove the player from the covenant captain list if they are on it.  Catch null because covenant may have been deleted.
            try
            {
                ChangeCovenantCaptain(possible, dbPlayer, true);
            }
            catch
            {

            }

            LoadCovenantDictionary();

            string covMessage = player.GetFullName() + " is no longer a member of the covenant.";
            WriteCovenantLog(covMessage, possible.Id, true);
        }

        public static int GetPlayerCountInCovenant(Covenant covenant, bool animateOnly)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            if (animateOnly == true)
            {
                return playerRepo.Players.Where(p => p.Covenant == covenant.Id && p.Mobility == "full").Count();
            }
            else
            {
                return playerRepo.Players.Where(p => p.Covenant == covenant.Id).Count();
            }

            
        }

        public static int GetPlayerCountInCovenant_Animate_Lvl3(Covenant covenant)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => p.Covenant == covenant.Id).Where(p => p.Mobility == "full" && p.Level >= 3).Count();
        }

        public static IEnumerable<CovenantListItemViewModel> GetCovenantsList()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            ICovenantRepository covRepo = new EFCovenantRepository();

            List<Covenant> dbCovenants = covRepo.Covenants.Where(c => c.Id > 0).ToList();

            List<CovenantListItemViewModel> output = new List<CovenantListItemViewModel>();

            foreach (Covenant c in dbCovenants)
            {
                CovenantListItemViewModel addme = new CovenantListItemViewModel
                {
                    dbCovenant = c,
                    MemberCount = playerRepo.Players.Where(p => p.Covenant == c.Id && p.MembershipId > 0).Count(),
                    Leader = playerRepo.Players.FirstOrDefault(p => p.Id == c.LeaderId)
                };
                output.Add(addme);
            }
            return output;
        }

        public static void AddCovenantApplication(Player applicant, Covenant covenant)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            CovenantApplication saveMe = new CovenantApplication
            {
                CovenantId = covenant.Id,
                OwnerId = applicant.Id,
                Timestamp = DateTime.UtcNow,
                Message = "",
            };
            covAppRepo.SaveCovenantApplication(saveMe);
            string message = "<b><span style='color: #003300;'>" + applicant.FirstName + " " + applicant.LastName + " has applied to your covenant, " + covenant.Name + ".</span></b>";
            PlayerLogProcedures.AddPlayerLog(covenant.LeaderId, message , true);
        }

        public static string RevokeApplication(Player player)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            CovenantApplication app = covAppRepo.CovenantApplications.FirstOrDefault(c => c.OwnerId == player.Id);

            if (app != null)
            {
                covAppRepo.DeleteCovenantApplication(app.Id);
                return "Your application has been withdrawn.";
            }
            return "You don't have any pending applications.";
        }

        public static bool PlayerHasPendingApplication(Player player)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            CovenantApplication app = covAppRepo.CovenantApplications.FirstOrDefault(c => c.OwnerId == player.Id);

            if (app != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<CovenantApplicationViewModel> GetCovenantApplications(Covenant covenant)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            IEnumerable<CovenantApplication> apps = covAppRepo.CovenantApplications.Where(c => c.CovenantId == covenant.Id);
            List<CovenantApplicationViewModel> output = new List<CovenantApplicationViewModel>();

            foreach (CovenantApplication app in apps)
            {
                CovenantApplicationViewModel addme = new CovenantApplicationViewModel();
                addme.dbCovenantApplication = app;
                addme.dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == app.OwnerId);
                output.Add(addme);
            }


            return output;
        }

        public static CovenantApplication GetCovenantApplication(int id)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            return covAppRepo.CovenantApplications.FirstOrDefault(c => c.Id == id);
        }

        public static void UpdateCovenantDescription(int covId, string newDescription, string flagUrl)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant oldcov = covRepo.Covenants.FirstOrDefault(c => c.Id == covId);
            oldcov.SelfDescription = newDescription;
            oldcov.FlagUrl = flagUrl;
            covRepo.SaveCovenant(oldcov);
        }

        public static void SetLastMemberJoinTimestamp(Covenant covenant)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant dbCov = covRepo.Covenants.FirstOrDefault(c => c.Id == covenant.Id);
            dbCov.LastMemberAcceptance = DateTime.UtcNow;
            covRepo.SaveCovenant(dbCov);
        }

        public static void SendPlayerMoneyToCovenant(Player player, decimal amount)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Covenant dbCov = covRepo.Covenants.FirstOrDefault(c => c.Id == player.Covenant);
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            dbPlayer.Money -= amount;
           
            dbCov.Money += amount;
            playerRepo.SavePlayer(dbPlayer);
            covRepo.SaveCovenant(dbCov);

            string covMessage = player.GetFullName() + " donated " + (int)amount + " Arpeyjis to the covenant treasury.";
            WriteCovenantLog(covMessage, dbCov.Id, false);

        }

        public static void SendCovenantMoneyToPlayer(int covId, Player giftee, decimal amount)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Covenant dbCov = covRepo.Covenants.FirstOrDefault(c => c.Id == covId);
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == giftee.Id);

            dbCov.Money -= amount;

            // taxes...
            amount *= .95M;
            amount = Math.Floor(amount);

            dbPlayer.Money += amount;
            
            playerRepo.SavePlayer(dbPlayer);
            covRepo.SaveCovenant(dbCov);

            string covMessage = (int)amount + " Arpeyjis were gifted out to " + giftee.GetFullName() + " from the covenant treasury.";
            WriteCovenantLog(covMessage, covId, false);
        }

        public static bool ACovenantHasASafegroundHere(string location)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            int covenantsBasedHere = covRepo.Covenants.Where(c => c.HomeLocation == location).Count();

            if (covenantsBasedHere > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetCovenantSafeground(Covenant covenant, string location)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);
            dbCovenant.HomeLocation = location;
            dbCovenant.Money -= 2500;
            dbCovenant.Level = 1;
            covRepo.SaveCovenant(dbCovenant);
            LoadCovenantDictionary();

            string covMessage = "Covenant safeground was establish at " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == location).Name + ".";
            WriteCovenantLog(covMessage, covenant.Id, true);

            AttackProcedures.InstantTakeoverLocation(dbCovenant, location);

        }

        public static bool CovenantHasSafeground(int covenantId)
        {
            ICovenantRepository repo = new EFCovenantRepository();
            Covenant cov = repo.Covenants.FirstOrDefault(c => c.Id == covenantId);
            return CovenantHasSafeground(cov);
        }

        public static bool CovenantHasSafeground(Covenant covenant)
        {
            if (covenant.HomeLocation != null && covenant.HomeLocation != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void WriteCovenantLog(string message, int covenantId, bool isImportant)
        {
            ICovenantLogRepository covLogRepo = new EFCovenantLogRepository();
            CovenantLog log = new CovenantLog
            {
                CovenantId = covenantId,
                Message = message,
                Timestamp = DateTime.UtcNow,
                IsImportant = isImportant,
            };
            covLogRepo.SaveCovenantLog(log);
        }

        public static IEnumerable<CovenantLog> GetCovenantLogs(int covenantId)
        {
            ICovenantLogRepository covLogRepo = new EFCovenantLogRepository();
            return covLogRepo.CovenantLogs.Where(l => l.CovenantId == covenantId).OrderByDescending(l => l.Timestamp).Take(100);
        }

        public static void CleanOldCovenantLogs(int covenantId)
        {

        }

        public static decimal GetUpgradeCost(Covenant covenant)
        {
            return (covenant.Level) * 3000;
        }

        public static void UpgradeCovenant(Covenant covenant)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);
            dbCovenant.Money -= GetUpgradeCost(covenant);
            dbCovenant.Level++;
            
            covRepo.SaveCovenant(dbCovenant);
            ICovenantLogRepository covLogRepo = new EFCovenantLogRepository();
            CovenantLog newlog = new CovenantLog
            {
                CovenantId = covenant.Id,
                IsImportant = true,
                Message="The covenant leader has upgraded the covenant safeground to level " + (covenant.Level + 1) + ", allowing it to hold more furniture.",
                Timestamp = DateTime.UtcNow,
            };
            covLogRepo.SaveCovenantLog(newlog);
        }

        public static int GetCovenantFurnitureLimit(Covenant covenant)
        {
            return covenant.Level + 4;
        }

        public static int GetCurrentFurnitureOwnedByCovenant(Covenant covenant)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            return furnRepo.Furnitures.Where(f => f.CovenantId == covenant.Id).Count();
        }

        public static string ChangeCovenantCaptain(Covenant covenant, Player player)
        {
            return ChangeCovenantCaptain(covenant, player, false);
        }

        public static string ChangeCovenantCaptain(Covenant covenant, Player player, bool removeOnly)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);

            string playerIdString = player.Id + ";";

            // if the captains is null, change it to have an empty string to prevent null exceptions
            if (dbCovenant.Captains == null)
            {
                dbCovenant.Captains = "";
                covRepo.SaveCovenant(dbCovenant);
            }

            if (dbCovenant.Captains.Contains(playerIdString) == true || removeOnly == true)
            {
                dbCovenant.Captains = dbCovenant.Captains.Replace(playerIdString, "");
                covRepo.SaveCovenant(dbCovenant);
                WriteCovenantLog(player.GetFullName() + " is no longer a captain of the covenant.", covenant.Id, true);
                return player.GetFullName() + " is no longer a captain of the covenant.";
            } else if (removeOnly == false) {
                dbCovenant.Captains += playerIdString;
                covRepo.SaveCovenant(dbCovenant);
                WriteCovenantLog(player.GetFullName() + " is now a captain of the covenant.", covenant.Id, true);
                return player.GetFullName() + " is now a captain of the covenant.";
            }
            return "";
            
        }

        public static bool PlayerIsCaptain(Covenant covenant, Player player)
        {
            if (covenant == null)
            {
                return false;
            }
            string idString = player.Id + ";";
            return covenant.Captains.Contains(idString);

        }


        public static string AttackLocation(Player player)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            ICovenantRepository covRepo = new EFCovenantRepository();
            LocationInfo info = repo.LocationInfos.FirstOrDefault(l => l.dbName == player.dbLocationName);
            Location location = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
            string output = "";
            if (info == null)
            {
                info = new LocationInfo{
                    TakeoverAmount = 150,
                    CovenantId = -1,
                    dbName = player.dbLocationName,
                };
            }

            float takeoverAmount = player.Level / 2;

            // location is not controlled; give it to whichever covenant is attacking it
            if (info.TakeoverAmount <= 0)
            {

                if (info.CovenantId > 0)
                {
                    string covLogLoser= player.GetFullName() + " enchanted " + location.Name + ", stealing it out of the covenant's influence!";
                    CovenantProcedures.WriteCovenantLog(covLogLoser, info.CovenantId, true);
                }

                info.CovenantId = player.Covenant;
                info.TakeoverAmount = takeoverAmount;
                info.LastTakeoverTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
                output = "<b>Your enchantment settles in this location, converting its energies from the previous controlling covenant to your own!</b>";
                LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName).CovenantController = player.Covenant;
                Covenant myCov = covRepo.Covenants.First(c => c.Id == player.Covenant);

                string locationLogMessage = "<b class='playerAttackNotification'>" + player.GetFullName() + " enchanted this location and claimed it for " + myCov.Name + "!</b>";
                LocationLogProcedures.AddLocationLog(player.dbLocationName, locationLogMessage);


                string covLogWinner = player.GetFullName() + " enchanted " + location.Name + " and has claimed it for this covenant.";
                CovenantProcedures.WriteCovenantLog(covLogWinner, myCov.Id, true);

            }

            // otherwise the location is controlled by someone
            else
            {
                // add points toward the attacker's covenant or take them away if it belongs to another
                if (info.CovenantId == player.Covenant)
                {
                    info.TakeoverAmount += takeoverAmount;
                    Covenant cov = covRepo.Covenants.FirstOrDefault(c => c.Id == player.Covenant);
                    output = "Your enchantment reinforces this location by " + (takeoverAmount) + ".  New influence level is " + info.TakeoverAmount + " for your covenant, " + cov.Name + ".";
                }
                else
                {
                    info.TakeoverAmount -= takeoverAmount;
                    Covenant cov = covRepo.Covenants.FirstOrDefault(c => c.Id == info.CovenantId);

                    if (info.TakeoverAmount <= 0)
                    {
                        info.CovenantId = -1;
                        info.LastTakeoverTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
                    }

                    if (cov != null)
                    {
                        output = "You dispell the enchantment at this location by " + takeoverAmount + ".  New influence level is " + info.TakeoverAmount + " for the location's existing controlled, " + cov.Name + ".";
                    }
                    else
                    {
                        output = "You dispell the enchantment at this location by " + takeoverAmount + ".  New influence level is " + info.TakeoverAmount + ".";
                    }

                  

                }

                string locationLogMessage = "<span class='playerAttackNotification'>" + player.GetFullName() + " cast an enchantment on this location.</span>";
                LocationLogProcedures.AddLocationLog(player.dbLocationName, locationLogMessage);

            }



            // cap at 0 to 100 points
            if (info.TakeoverAmount >= 100 && info.CovenantId != -1)
            {
                info.TakeoverAmount = 100;
            }
            else if (info.TakeoverAmount <= 0)
            {
                info.CovenantId = -1;
                info.TakeoverAmount = 0;
            }

            

            repo.SaveLocationInfo(info);

            return output;
        }

        public static int GetLocationCovenantOwner(string location)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();

            LocationInfo info = repo.LocationInfos.FirstOrDefault(l => l.dbName == location);

            if (info != null)
            {
                return info.CovenantId;
            }
            else
            {
                return -1;
            }
        }

        public static int GetLocationControlCount(Covenant cov)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            return repo.LocationInfos.Where(c => c.CovenantId == cov.Id).Count();
        }

        public static IEnumerable<LocationInfo> GetLocationInfos()
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            return repo.LocationInfos;
        }

        public static bool FlagIsInUse(string flagURL)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant covWithFlag = covRepo.Covenants.FirstOrDefault(c => c.FlagUrl == flagURL);

            if (covWithFlag != null)
            {
                return true;
            }
            else
            {
                
                return false;
            }

        }

        public static List<string> FilterAvailableFlags(List<string> input)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            List<string> usedFlags = covRepo.Covenants.Select(c => c.FlagUrl).ToList();

            List<string> output = new List<string>();

            foreach (string s in input)
            {
                if (usedFlags.Contains(s) == false)
                {
                    output.Add(s);
                }
            }
            return output;
        }

    }
}
using System;
using System.Collections.Generic;
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

            if (GetPlayerCountInCovenant(possible) == 0)
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
            LoadCovenantDictionary();

            string covMessage = player.GetFullName() + " is no longer a member of the covenant.";
            WriteCovenantLog(covMessage, possible.Id, true);
        }

        public static int GetPlayerCountInCovenant(Covenant covenant)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return playerRepo.Players.Where(p => p.Covenant == covenant.Id).Count();
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

        public static void UpdateCovenantDescription(int covId, string newDescription)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            Covenant oldcov = covRepo.Covenants.FirstOrDefault(c => c.Id == covId);
            oldcov.SelfDescription = newDescription;
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
    

    }
}
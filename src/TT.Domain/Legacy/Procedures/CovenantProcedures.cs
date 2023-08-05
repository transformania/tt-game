using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{



    public static class CovenantProcedures
    {

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
            if (player.Covenant == null)
            {
                return null;
            }
            else
            {
                return GetCovenantViewModel((int)player.Covenant);
            }
        }

        public static CovenantViewModel GetCovenantViewModel(int id)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            var output = new CovenantViewModel();
            var cov = covRepo.Covenants.FirstOrDefault(c => c.Id == id);

            output.dbCovenant = cov;
            output.Leader = playerRepo.Players.FirstOrDefault(p => p.Id == output.dbCovenant.LeaderId);

            var playerFormList = PlayerProcedures.GetPlayerFormViewModelsInCovenant(id);
            output.Members = playerFormList.Where(p => p.Player.BotId != AIStatics.RerolledPlayerBotId);

            output.IsFull = GetPlayerCountInCovenant(cov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount;

            return output;
        }

        public static void LoadCovenantDictionary()
        {
            ICovenantRepository covRepo = new EFCovenantRepository();

            CovenantDictionary.IdNameFlagLookup.Clear();


            foreach (var c in covRepo.Covenants.Where(c => c.LeaderId > -1))
            {
                var temp = new CovenantNameFlag
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

            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);
            dbPlayer.Covenant = covId;
            playerRepo.SavePlayer(dbPlayer);

            var covMessage = dbPlayer.GetFullName() + " is now a member of the covenant.";
            WriteCovenantLog(covMessage, covId, true);

            return "";
        }

        public static string StartNewCovenant(Covenant newcov)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var dbCov = new Covenant();
          //  dbCov.IsPvP = newcov.IsPvP;
            dbCov.IsPvP = false;
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
            var possible = covRepo.Covenants.FirstOrDefault(c => c.Name == name);

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
            var dbPlayer = playerRepo.Players.First(p => p.Id == player.Id);
            dbPlayer.Covenant = null;
            playerRepo.SavePlayer(dbPlayer);

            // delete the covenant if it is now empty
            ICovenantRepository covRepo = new EFCovenantRepository();
            var possible = covRepo.Covenants.FirstOrDefault(c => c.Id == player.Covenant);

            var covMemberCount = GetPlayerCountInCovenant(possible, false);

            if (covMemberCount == 0)
            {
                covRepo.DeleteCovenant(possible.Id);
            }

            // the covenant is not empty, so we need to give it a new leader.  For now the new leader is essentially random.
            else if (possible.LeaderId == player.Id)
            {
                var nextleader = playerRepo.Players.FirstOrDefault(p => p.Covenant == possible.Id);
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

            var covMessage = player.GetFullName() + " is no longer a member of the covenant.";
            WriteCovenantLog(covMessage, possible.Id, true);
        }

        public static int GetPlayerCountInCovenant(Covenant covenant, bool animateOnly)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            return animateOnly
                ? playerRepo.Players.Count(p => p.Covenant == covenant.Id &&
                                                p.BotId == AIStatics.ActivePlayerBotId &&
                                                p.Mobility == PvPStatics.MobilityFull)
                : playerRepo.Players.Count(p => p.Covenant == covenant.Id &&
                                                p.BotId == AIStatics.ActivePlayerBotId);
        }

        public static int GetPlayerCountInCovenant_Animate_Lvl3(Covenant covenant)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            return
                playerRepo.Players.Count(p => p.Covenant == covenant.Id &&
                                              p.BotId == AIStatics.ActivePlayerBotId &&
                                              p.Mobility == PvPStatics.MobilityFull &&
                                              p.Level >= 3);
        }

        public static IEnumerable<CovenantListItemViewModel> GetCovenantsList()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            ICovenantRepository covRepo = new EFCovenantRepository();

            var dbCovenants = covRepo.Covenants.Where(c => c.Id > 0).ToList();

            return dbCovenants.Select(c => new CovenantListItemViewModel
            {
                dbCovenant = c,
                MemberCount =
                    playerRepo.Players.Count(p => p.Covenant == c.Id &&
                                                  p.BotId == AIStatics.ActivePlayerBotId),
                AnimateMemberCount =
                    playerRepo.Players.Count(p => p.Covenant == c.Id &&
                                                  p.BotId == AIStatics.ActivePlayerBotId &&
                                                  p.Mobility == PvPStatics.MobilityFull),
                Leader = playerRepo.Players.FirstOrDefault(p => p.Id == c.LeaderId)
            }).ToList();
        }

        public static void AddCovenantApplication(Player applicant, Covenant covenant)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            var saveMe = new CovenantApplication
            {
                CovenantId = covenant.Id,
                OwnerId = applicant.Id,
                Timestamp = DateTime.UtcNow,
                Message = "",
            };
            covAppRepo.SaveCovenantApplication(saveMe);
            var message = "<b><span style='color: #003300;'>" + applicant.FirstName + " " + applicant.LastName + " has applied to your covenant, " + covenant.Name + ".</span></b>";

            ICovenantRepository covRepo = new EFCovenantRepository();
            var CovCaptains = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id).Captains.TrimEnd(';');
            var captains = CovCaptains.Split(';');

            // Send message to coven leader
            PlayerLogProcedures.AddPlayerLog(covenant.LeaderId, message, true);

            // Send messcage to each captain
            if (captains.Length > 0)
            {
                for (var i = 0; i < captains.Length; i++)
                {
                    PlayerLogProcedures.AddPlayerLog(Int32.Parse(captains[i]), message, true);
                }
            }
        }

        public static string RevokeApplication(Player player)
        {
            ICovenantApplicationRepository covAppRepo = new EFCovenantApplicationRepository();
            var app = covAppRepo.CovenantApplications.FirstOrDefault(c => c.OwnerId == player.Id);

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
            var app = covAppRepo.CovenantApplications.FirstOrDefault(c => c.OwnerId == player.Id);

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
            var output = new List<CovenantApplicationViewModel>();

            foreach (var app in apps)
            {
                var addme = new CovenantApplicationViewModel();
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
            var oldcov = covRepo.Covenants.FirstOrDefault(c => c.Id == covId);
            oldcov.SelfDescription = newDescription;
            oldcov.FlagUrl = flagUrl;
            covRepo.SaveCovenant(oldcov);
        }

        public static void ChangeNoticeboardMessage(int covId, string newMessage)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var oldcov = covRepo.Covenants.FirstOrDefault(c => c.Id == covId);
            oldcov.NoticeboardMessage = newMessage;
            covRepo.SaveCovenant(oldcov);
        }

        public static void SetLastMemberJoinTimestamp(Covenant covenant)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var dbCov = covRepo.Covenants.FirstOrDefault(c => c.Id == covenant.Id);
            dbCov.LastMemberAcceptance = DateTime.UtcNow;
            covRepo.SaveCovenant(dbCov);
        }

        public static void SendPlayerMoneyToCovenant(Player player, decimal amount)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbCov = covRepo.Covenants.FirstOrDefault(c => c.Id == player.Covenant);
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            // reducing the money transfer amount to prevent the covenant money going over the limit
            if (dbCov.Money + amount > PvPStatics.MaxMoney)
            {
                amount = PvPStatics.MaxMoney - dbCov.Money;
            }

            dbPlayer.Money -= amount;
           
            dbCov.Money += amount;
            playerRepo.SavePlayer(dbPlayer);
            covRepo.SaveCovenant(dbCov);

            var covMessage = player.GetFullName() + " donated " + (int)amount + " Arpeyjis to the covenant treasury.";
            WriteCovenantLog(covMessage, dbCov.Id, false);

        }

        public static void SendCovenantMoneyToPlayer(int covId, Player giftee, decimal amount)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbCov = covRepo.Covenants.FirstOrDefault(c => c.Id == covId);
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == giftee.Id);

            dbCov.Money -= amount;

            // taxes...
            amount *= .95M;
            amount = Math.Floor(amount);

            if (dbPlayer.Money + amount > PvPStatics.MaxMoney)
            {
                dbPlayer.Money = PvPStatics.MaxMoney;
            }
            else 
            {
                dbPlayer.Money += amount;
            }

            playerRepo.SavePlayer(dbPlayer);
            covRepo.SaveCovenant(dbCov);

            var covMessage = (int)amount + " Arpeyjis were gifted out to " + giftee.GetFullName() + " from the covenant treasury.";
            WriteCovenantLog(covMessage, covId, false);
        }

        public static bool ACovenantHasASafegroundHere(string location)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var covenantsBasedHere = covRepo.Covenants.Count(c => c.HomeLocation == location);
            return covenantsBasedHere > 0;
        }

        public static void SetCovenantSafeground(Covenant covenant, string location)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);
            dbCovenant.HomeLocation = location;
            dbCovenant.Money -= 2500;
            dbCovenant.Level = 1;
            covRepo.SaveCovenant(dbCovenant);
            LoadCovenantDictionary();

            var covMessage =
                $"Covenant safeground was establish at {LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == location)?.Name ?? "unknown"}.";
            WriteCovenantLog(covMessage, covenant.Id, true);

            AttackProcedures.InstantTakeoverLocation(dbCovenant, location);

        }

        public static bool CovenantHasSafeground(int covenantId)
        {
            ICovenantRepository repo = new EFCovenantRepository();
            var cov = repo.Covenants.FirstOrDefault(c => c.Id == covenantId);
            return CovenantHasSafeground(cov);
        }

        public static bool CovenantHasSafeground(Covenant covenant)
        {
            if (!covenant.HomeLocation.IsNullOrEmpty())
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
            var log = new CovenantLog
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

        public static decimal GetUpgradeCost(Covenant covenant)
        {
            return (covenant.Level) * 3000;
        }

        public static void UpgradeCovenant(Covenant covenant)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);
            dbCovenant.Money -= GetUpgradeCost(covenant);
            dbCovenant.Level++;
            
            covRepo.SaveCovenant(dbCovenant);
            ICovenantLogRepository covLogRepo = new EFCovenantLogRepository();
            var newlog = new CovenantLog
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
            return covenant.Level + 2;
        }

        public static int GetCurrentFurnitureOwnedByCovenant(Covenant covenant)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            return furnRepo.Furnitures.Count(f => f.CovenantId == covenant.Id);
        }

        public static string ChangeCovenantCaptain(Covenant covenant, Player player, bool removeOnly = false)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);
            if (dbCovenant == null)
            {
                return "Covenant could not be found.";
            }

            var playerIdString = player.Id + ";";

            // if the captains is null, change it to have an empty string to prevent null exceptions
            if (dbCovenant.Captains == null)
            {
                dbCovenant.Captains = "";
                covRepo.SaveCovenant(dbCovenant);
            }

            var isACaptain = dbCovenant.Captains.Contains(playerIdString);
            if (isACaptain && removeOnly)
            {
                dbCovenant.Captains = dbCovenant.Captains.Replace(playerIdString, "");
                covRepo.SaveCovenant(dbCovenant);
                WriteCovenantLog(player.GetFullName() + " is no longer a captain of the covenant.", covenant.Id, true);
                return player.GetFullName() + " is no longer a captain of the covenant.";
            }

            if (isACaptain)
            {
                return $"{player.GetFullName()} is already a captain.";
            }

            if (removeOnly)
            {
                return $"{player.GetFullName()} can't be removed as a captain because they are not one currently.";
            }

            dbCovenant.Captains += playerIdString;
            covRepo.SaveCovenant(dbCovenant);
            WriteCovenantLog(player.GetFullName() + " is now a captain of the covenant.", covenant.Id, true);
            return player.GetFullName() + " is now a captain of the covenant.";
        }

        public static string ChangeCovenantLeader(Covenant covenant, Player player)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var dbCovenant = covRepo.Covenants.FirstOrDefault(i => i.Id == covenant.Id);
            if (dbCovenant == null)
            {
                return "Covenant could not be found.";
            }

            dbCovenant.LeaderId = player.Id;
            covRepo.SaveCovenant(dbCovenant);
            WriteCovenantLog(player.GetFullName() + " is now the leader of the covenant.", covenant.Id, true);
            return player.GetFullName() + " is now the leader of the covenant.";
        }

        public static bool PlayerIsCaptain(Covenant covenant, Player player)
        {
            if (covenant == null)
            {
                return false;
            }
            var idString = player.Id + ";";
            return covenant.Captains.Contains(idString);

        }

        public static string AttackLocation(Player player, BuffBox buffs)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            ICovenantRepository covRepo = new EFCovenantRepository();
            var location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
            var output = "";
            if (location == null)
            {
                output = "You cast an enchantment here, but you aren't actually anywhere!";
                return output;
            }

            var info = repo.LocationInfos.FirstOrDefault(l => l.dbName == player.dbLocationName) ?? new LocationInfo
            {
                TakeoverAmount = 75,
                CovenantId = -1,
                dbName = player.dbLocationName,
            };
            if (player.Covenant == null)
            {
                output = "You cast an enchantment here, but it did no effect as you aren't part of a covenant";
                return output;
            }

            if (info.TakeoverAmount >= 100 && info.CovenantId == player.Covenant)
            {
                output = "You cast an enchantment here, but it did no effect as this location's enchantment is already at its highest possible level, 100.";
                return output;
            }

            var takeoverAmount = (float)player.Level / 2.0F;

            takeoverAmount += buffs.EnchantmentBoost;

            decimal XPGain = 0;

            try
            {
                XPGain = 40 / Math.Round(Convert.ToDecimal(101 - Math.Abs(info.TakeoverAmount)), 1);
            }
            catch (Exception)
            {
                XPGain = 0;
            }

            if (XPGain > PvPStatics.XP__EnchantmentMaxXP)
            {
                XPGain = PvPStatics.XP__EnchantmentMaxXP;
            }

            var XPGainText = String.Format("{0:0.#}", XPGain);

            // location is not controlled; give it to whichever covenant is attacking it
            if (info.TakeoverAmount <= 0)
            {

                info.CovenantId = (int)player.Covenant;
                info.TakeoverAmount = takeoverAmount;


                if (info.TakeoverAmount > 100)
                {
                    info.TakeoverAmount = 100;
                }

                info.LastTakeoverTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
                output = "<b>Your enchantment settles in this location, converting its energies from the previous controlling covenant to your own!  (+" + XPGainText + " XP)</b>";
                location.CovenantController = (int)player.Covenant;
                location.TakeoverAmount = info.TakeoverAmount;
                var myCov = covRepo.Covenants.First(c => c.Id == player.Covenant);

                var locationLogMessage = "<b class='playerAttackNotification'>" + player.GetFullName() + " enchanted this location and claimed it for " + myCov.Name + "!</b>";
                LocationLogProcedures.AddLocationLog(player.dbLocationName, locationLogMessage);


                var covLogWinner = player.GetFullName() + " enchanted " + location.Name + " and has claimed it for this covenant.";
                CovenantProcedures.WriteCovenantLog(covLogWinner, myCov.Id, true);

                

            }

            // otherwise the location is controlled by someone
            else
            {
                // add points toward the attacker's covenant or take them away if it belongs to another
                if (info.CovenantId == player.Covenant)
                {
                    info.TakeoverAmount += takeoverAmount;
                    location.TakeoverAmount = info.TakeoverAmount;
                    var cov = covRepo.Covenants.FirstOrDefault(c => c.Id == player.Covenant);
                    output =
                        $"Your enchantment reinforces this location by {takeoverAmount}.  New influence level is {info.TakeoverAmount} for your covenant, {cov?.Name ?? "unknown"}.  (+{XPGainText} XP)</b>";
                   
                }
                else
                {
                    info.TakeoverAmount -= takeoverAmount;
                    location.TakeoverAmount = info.TakeoverAmount;
                    var cov = info.CovenantId == null
                        ? null
                        : covRepo.Covenants.FirstOrDefault(c => c.Id == info.CovenantId);

                    if (info.TakeoverAmount <= 0)
                    {

                        // notify old covenant who stole the location and their covenant
                        if (info.CovenantId != null && info.CovenantId > 0)
                        {
                            var attackingCov = CovenantProcedures.GetCovenantViewModel((int)player.Covenant);
                            var covLogLoser = player.GetFullName() + " of " + attackingCov.dbCovenant.Name + " enchanted " + location.Name + ", removing it from this covenant's influence!";
                            CovenantProcedures.WriteCovenantLog(covLogLoser, (int)info.CovenantId, true);
                        }

                        info.CovenantId = -1;
                        info.LastTakeoverTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
                    }

                    if (cov != null)
                    {
                        output = "You dispel the enchantment at this location by " + takeoverAmount + ".  New influence level is " + info.TakeoverAmount + " for the location's existing controller, " + cov.Name + ".  (+" + XPGainText + " XP)</b>";
                    }
                    else
                    {
                        output = "You dispel the enchantment at this location by " + takeoverAmount + ".  New influence level is " + info.TakeoverAmount + ".  (+" + XPGainText + " XP)</b>";
                    }

                  

                }

                var locationLogMessage = "<span class='playerAttackNotification'>" + player.GetFullName() + " cast an enchantment on this location.</span>";
                LocationLogProcedures.AddLocationLog(player.dbLocationName, locationLogMessage);

            }


            if (info.TakeoverAmount > 100)
            {
                info.TakeoverAmount = 100;
            }

            // cap at 0 to 100 points
           else if (info.TakeoverAmount <= 0)
            {
                info.CovenantId = -1;
                info.TakeoverAmount = 0;
            }

            

            repo.SaveLocationInfo(info);
            PlayerProcedures.GiveXP(player, XPGain);
            PlayerLogProcedures.AddPlayerLog(player.Id, output, false);

            return output;
        }

        public static int? GetLocationCovenantOwner(string location)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();

            var info = repo.LocationInfos.FirstOrDefault(l => l.dbName == location);

            if (info != null && info.CovenantId != null)
            {
                return (int)info.CovenantId;
            }
            else
            {
                return null;
            }
        }

        public static int GetLocationControlCount(Covenant cov)
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            return repo.LocationInfos.Count(c => c.CovenantId == cov.Id);
        }

        public static IEnumerable<LocationInfo> GetLocationInfos()
        {
            ILocationInfoRepository repo = new EFLocationInfoRepository();
            return repo.LocationInfos;
        }

        public static bool FlagIsInUse(string flagURL)
        {
            ICovenantRepository covRepo = new EFCovenantRepository();
            var covWithFlag = covRepo.Covenants.FirstOrDefault(c => c.FlagUrl == flagURL);

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
            var usedFlags = covRepo.Covenants.Select(c => c.FlagUrl).ToList();

            var output = new List<string>();

            foreach (var s in input)
            {
                if (!usedFlags.Contains(s))
                {
                    output.Add(s);
                }
            }
            return output;
        }

    }
}
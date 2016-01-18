using System.Linq;
using System.Web.Mvc;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Controllers
{
    public class TransformaniaTimeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FashionWars()
        {
            return View();
        }

        public ActionResult Bombie()
        {
            return View();
        }

        public ActionResult Play()
        {
           // using (var db = new StatsContext())
           // {
           //     GameshowStats newstat = new GameshowStats();
           //     newstat.FinishedTransformations = 5;

           //     db.GameshowStats.Add(newstat);
          //  }
            return View();
        }

        public void PostStat(string message)
        {
            EFGameshowStatsRepository statRepo = new EFGameshowStatsRepository();
            GameshowStats updatedStat = statRepo.GameshowStatss.FirstOrDefault();

            if (message == "AI full TF")
            {
                updatedStat.FinishedTransformations++;
                updatedStat.FinishedTransformationsAI++;
            }
            else if (message == "Human full TF")
            {
                updatedStat.FinishedTransformations++;
                updatedStat.FinishedTransformationsHuman++;
            }
            else if (message == "Win Dpong")
            {
                updatedStat.Dpong_Wins++;
            }
            else if (message == "Lose Dpong")
            {
                updatedStat.Dpong_Losses++;
            }


            else if (message == "Win Sequence")
            {
                updatedStat.Sequence_Wins++;
            }
            else if (message == "Lose Sequence")
            {
                updatedStat.Sequence_Losses++;
            }


            else if (message == "Win Roulette")
            {
                updatedStat.Roulette_Wins++;
            }
            else if (message == "Lose Roulette")
            {
                updatedStat.Roulette_Losses++;
            }


            else if (message == "Win Popup")
            {
                updatedStat.Popup_Wins++;
            }
            else if (message == "Lose Popup")
            {
                updatedStat.Popup_Losses++;
            }


            else if (message == "Win Discus")
            {
                updatedStat.Discus_Wins++;
            }
            else if (message == "Lose Discus")
            {
                updatedStat.Discus_Losses++;
            }


            else if (message == "Win Match2")
            {
                updatedStat.Match2_Wins++;
            }
            else if (message == "Lose Match2")
            {
                updatedStat.Match2_Losses++;
            }

            else if (message == "Win Target")
            {
                updatedStat.Target_Wins++;
            }
            else if (message == "Lose Target")
            {
                updatedStat.Target_Losses++;
            }
           


            statRepo.SaveGameshowStats(updatedStat);
        }
    }
}
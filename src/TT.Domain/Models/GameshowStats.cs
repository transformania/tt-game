namespace TT.Domain.Models
{
    public class GameshowStats
    {
        public int Id { get; set; }
        public int FinishedTransformations { get; set; }
        public int FinishedTransformationsAI { get; set; }
        public int FinishedTransformationsHuman { get; set; }

        public int Discus_Wins { get; set; }
        public int Discus_Losses { get; set; }

        public int Dpong_Wins { get; set; }
        public int Dpong_Losses { get; set; }

        public int Sequence_Wins { get; set; }
        public int Sequence_Losses { get; set; }

        public int Popup_Wins { get; set; }
        public int Popup_Losses { get; set; }

        public int Target_Wins { get; set; }
        public int Target_Losses { get; set; }

        public int Roulette_Wins { get; set; }
        public int Roulette_Losses { get; set; }

        public int Match2_Wins { get; set; }
        public int Match2_Losses { get; set; }

      

    }
}
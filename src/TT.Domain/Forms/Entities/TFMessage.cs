using TT.Domain.Entities;

namespace TT.Domain.Forms.Entities
{
    public class TFMessage : Entity<int>
    {

        public string TFMessage_20_Percent_1st { get; protected set; }
        public string TFMessage_40_Percent_1st { get; protected set; }
        public string TFMessage_60_Percent_1st { get; protected set; }
        public string TFMessage_80_Percent_1st { get; protected set; }
        public string TFMessage_100_Percent_1st { get; protected set; }
        public string TFMessage_Completed_1st { get; protected set; }

        public string TFMessage_20_Percent_1st_M { get; protected set; }
        public string TFMessage_40_Percent_1st_M { get; protected set; }
        public string TFMessage_60_Percent_1st_M { get; protected set; }
        public string TFMessage_80_Percent_1st_M { get; protected set; }
        public string TFMessage_100_Percent_1st_M { get; protected set; }
        public string TFMessage_Completed_1st_M { get; protected set; }

        public string TFMessage_20_Percent_1st_F { get; protected set; }
        public string TFMessage_40_Percent_1st_F { get; protected set; }
        public string TFMessage_60_Percent_1st_F { get; protected set; }
        public string TFMessage_80_Percent_1st_F { get; protected set; }
        public string TFMessage_100_Percent_1st_F { get; protected set; }
        public string TFMessage_Completed_1st_F { get; protected set; }

        public string TFMessage_20_Percent_3rd { get; protected set; }
        public string TFMessage_40_Percent_3rd { get; protected set; }
        public string TFMessage_60_Percent_3rd { get; protected set; }
        public string TFMessage_80_Percent_3rd { get; protected set; }
        public string TFMessage_100_Percent_3rd { get; protected set; }
        public string TFMessage_Completed_3rd { get; protected set; }

        public string TFMessage_20_Percent_3rd_M { get; protected set; }
        public string TFMessage_40_Percent_3rd_M { get; protected set; }
        public string TFMessage_60_Percent_3rd_M { get; protected set; }
        public string TFMessage_80_Percent_3rd_M { get; protected set; }
        public string TFMessage_100_Percent_3rd_M { get; protected set; }
        public string TFMessage_Completed_3rd_M { get; protected set; }

        public string TFMessage_20_Percent_3rd_F { get; protected set; }
        public string TFMessage_40_Percent_3rd_F { get; protected set; }
        public string TFMessage_60_Percent_3rd_F { get; protected set; }
        public string TFMessage_80_Percent_3rd_F { get; protected set; }
        public string TFMessage_100_Percent_3rd_F { get; protected set; }
        public string TFMessage_Completed_3rd_F { get; protected set; }

        public string CursedTF_Fail { get; protected set; }
        public string CursedTF_Fail_M { get; protected set; }
        public string CursedTF_Fail_F { get; protected set; }
        public string CursedTF_Succeed { get; protected set; }
        public string CursedTF_Succeed_M { get; protected set; }
        public string CursedTF_Succeed_F { get; protected set; }

        public virtual FormSource FormSource { get; protected set; }

        private TFMessage() { }

    }
}

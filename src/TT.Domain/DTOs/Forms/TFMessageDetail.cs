using TT.Domain.DTOs.Forms;

namespace TT.Domain.DTOs.Forms
{
    public class TFMessageDetail
    {
        public int Id { get; set; }
        public string FormDbName { get;  set; }

        public string TFMessage_20_Percent_1st { get;  set; }
        public string TFMessage_40_Percent_1st { get;  set; }
        public string TFMessage_60_Percent_1st { get;  set; }
        public string TFMessage_80_Percent_1st { get;  set; }
        public string TFMessage_100_Percent_1st { get;  set; }
        public string TFMessage_Completed_1st { get;  set; }

        public string TFMessage_20_Percent_1st_M { get;  set; }
        public string TFMessage_40_Percent_1st_M { get;  set; }
        public string TFMessage_60_Percent_1st_M { get;  set; }
        public string TFMessage_80_Percent_1st_M { get;  set; }
        public string TFMessage_100_Percent_1st_M { get;  set; }
        public string TFMessage_Completed_1st_M { get;  set; }

        public string TFMessage_20_Percent_1st_F { get;  set; }
        public string TFMessage_40_Percent_1st_F { get;  set; }
        public string TFMessage_60_Percent_1st_F { get;  set; }
        public string TFMessage_80_Percent_1st_F { get;  set; }
        public string TFMessage_100_Percent_1st_F { get;  set; }
        public string TFMessage_Completed_1st_F { get;  set; }

        public string TFMessage_20_Percent_3rd { get;  set; }
        public string TFMessage_40_Percent_3rd { get;  set; }
        public string TFMessage_60_Percent_3rd { get;  set; }
        public string TFMessage_80_Percent_3rd { get;  set; }
        public string TFMessage_100_Percent_3rd { get;  set; }
        public string TFMessage_Completed_3rd { get;  set; }

        public string TFMessage_20_Percent_3rd_M { get;  set; }
        public string TFMessage_40_Percent_3rd_M { get;  set; }
        public string TFMessage_60_Percent_3rd_M { get;  set; }
        public string TFMessage_80_Percent_3rd_M { get;  set; }
        public string TFMessage_100_Percent_3rd_M { get;  set; }
        public string TFMessage_Completed_3rd_M { get;  set; }

        public string TFMessage_20_Percent_3rd_F { get;  set; }
        public string TFMessage_40_Percent_3rd_F { get;  set; }
        public string TFMessage_60_Percent_3rd_F { get;  set; }
        public string TFMessage_80_Percent_3rd_F { get;  set; }
        public string TFMessage_100_Percent_3rd_F { get;  set; }
        public string TFMessage_Completed_3rd_F { get;  set; }

        public string CursedTF_Fail { get;  set; }
        public string CursedTF_Fail_M { get;  set; }
        public string CursedTF_Fail_F { get;  set; }
        public string CursedTF_Succeed { get;  set; }
        public string CursedTF_Succeed_M { get;  set; }
        public string CursedTF_Succeed_F { get;  set; }

        public FormSourceDetail FormSource { get; protected set; }
    }
}

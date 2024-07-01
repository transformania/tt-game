using TT.Domain.Entities;
using TT.Domain.Forms.DTOs;

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

        private TFMessage()
        {
        }

        public TFMessageDetail MapToDto(bool includeFormSource = true)
        {
            return new TFMessageDetail
            {
                Id = Id,
                TFMessage_20_Percent_1st = TFMessage_20_Percent_1st,
                TFMessage_40_Percent_1st = TFMessage_40_Percent_1st,
                TFMessage_60_Percent_1st = TFMessage_60_Percent_1st,
                TFMessage_80_Percent_1st = TFMessage_80_Percent_1st,
                TFMessage_100_Percent_1st = TFMessage_100_Percent_1st,
                TFMessage_Completed_1st = TFMessage_Completed_1st,
                TFMessage_20_Percent_1st_M = TFMessage_20_Percent_1st_M,
                TFMessage_40_Percent_1st_M = TFMessage_40_Percent_1st_M,
                TFMessage_60_Percent_1st_M = TFMessage_60_Percent_1st_M,
                TFMessage_80_Percent_1st_M = TFMessage_80_Percent_1st_M,
                TFMessage_100_Percent_1st_M = TFMessage_100_Percent_1st_M,
                TFMessage_Completed_1st_M = TFMessage_Completed_1st_M,
                TFMessage_20_Percent_1st_F = TFMessage_20_Percent_1st_F,
                TFMessage_40_Percent_1st_F = TFMessage_40_Percent_1st_F,
                TFMessage_60_Percent_1st_F = TFMessage_60_Percent_1st_F,
                TFMessage_80_Percent_1st_F = TFMessage_80_Percent_1st_F,
                TFMessage_100_Percent_1st_F = TFMessage_100_Percent_1st_F,
                TFMessage_Completed_1st_F = TFMessage_Completed_1st_F,
                TFMessage_20_Percent_3rd = TFMessage_20_Percent_3rd,
                TFMessage_40_Percent_3rd = TFMessage_40_Percent_3rd,
                TFMessage_60_Percent_3rd = TFMessage_60_Percent_3rd,
                TFMessage_80_Percent_3rd = TFMessage_80_Percent_3rd,
                TFMessage_100_Percent_3rd = TFMessage_100_Percent_3rd,
                TFMessage_Completed_3rd = TFMessage_Completed_3rd,
                TFMessage_20_Percent_3rd_M = TFMessage_20_Percent_3rd_M,
                TFMessage_40_Percent_3rd_M = TFMessage_40_Percent_3rd_M,
                TFMessage_60_Percent_3rd_M = TFMessage_60_Percent_3rd_M,
                TFMessage_80_Percent_3rd_M = TFMessage_80_Percent_3rd_M,
                TFMessage_100_Percent_3rd_M = TFMessage_100_Percent_3rd_M,
                TFMessage_Completed_3rd_M = TFMessage_Completed_3rd_M,
                TFMessage_20_Percent_3rd_F = TFMessage_20_Percent_3rd_F,
                TFMessage_40_Percent_3rd_F = TFMessage_40_Percent_3rd_F,
                TFMessage_60_Percent_3rd_F = TFMessage_60_Percent_3rd_F,
                TFMessage_80_Percent_3rd_F = TFMessage_80_Percent_3rd_F,
                TFMessage_100_Percent_3rd_F = TFMessage_100_Percent_3rd_F,
                TFMessage_Completed_3rd_F = TFMessage_Completed_3rd_F,
                CursedTF_Fail = CursedTF_Fail,
                CursedTF_Fail_M = CursedTF_Fail_M,
                CursedTF_Fail_F = CursedTF_Fail_F,
                CursedTF_Succeed = CursedTF_Succeed,
                CursedTF_Succeed_M = CursedTF_Succeed_M,
                CursedTF_Succeed_F = CursedTF_Succeed_F,
                FormSource = includeFormSource ? FormSource.MapToDto() : null,
            };
        }
    }
}
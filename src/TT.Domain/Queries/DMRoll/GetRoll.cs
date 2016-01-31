using TT.Domain.Procedures;

namespace TT.Domain.Queries.DMRoll
{
    public class GetRollText : QuerySingle<string>
    {
        public string ActionType { get; set; }
        public string Tag { get; set; }
        
        internal override string FindSingle()
        {
            return DMRollProcedures.GetRoll(ActionType, Tag);
        }
    }
}
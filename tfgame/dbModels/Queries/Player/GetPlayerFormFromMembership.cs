using tfgame.Procedures;
using tfgame.ViewModels;

namespace tfgame.dbModels.Queries.Player
{
    public class GetPlayerFormFromMembership : QuerySingle<PlayerFormViewModel>
    {
        public int MembershipId { get; set; }
        
        internal override PlayerFormViewModel FindSingle()
        {           
            return PlayerProcedures.GetPlayerFormViewModel_FromMembership(MembershipId);
        }
    }
}
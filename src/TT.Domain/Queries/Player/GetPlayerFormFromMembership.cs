using TT.Domain.Procedures;
using TT.Domain.ViewModels;

namespace TT.Domain.Queries.Player
{
    public class GetPlayerFormFromMembership : QuerySingle<PlayerFormViewModel>
    {
        public string MembershipId { get; set; }
        
        internal override PlayerFormViewModel FindSingle()
        {           
            return PlayerProcedures.GetPlayerFormViewModel_FromMembership(MembershipId);
        }
    }
}
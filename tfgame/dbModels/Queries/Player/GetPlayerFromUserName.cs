using System.Linq;
using Microsoft.Ajax.Utilities;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Models;
using tfgame.Procedures;

namespace tfgame.dbModels.Queries.Player
{
    public class GetPlayerFromUserName : QuerySingle<Player_VM>
    {
        public string UserName { get; set; }
        
        internal override Player_VM FindSingle()
        {
            int userId;
            
            using (var userContext = new UsersContext())
            {
                userId = userContext.UserProfiles.Single(up => up.UserName == UserName).UserId;
            }

            return PlayerProcedures.GetPlayerFormViewModel_FromMembership(userId).Player;
        }
    }
}
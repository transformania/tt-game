using tfgame.dbModels.Models;
using tfgame.Procedures;

namespace tfgame.dbModels.Commands.Player
{
    public class UpdateOnlineActivityTimestamp : Command
    {
        public Player_VM Player { get; set; }
        
        internal override void InternalExecute()
        {
            PlayerProcedures.MarkOnlineActivityTimestamp(Player);
        }
    }
}
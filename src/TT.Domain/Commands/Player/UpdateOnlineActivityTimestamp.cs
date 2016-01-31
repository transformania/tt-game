using TT.Domain.Models;
using TT.Domain.Procedures;

namespace TT.Domain.Commands.Player
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
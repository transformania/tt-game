using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class ItemTransferLogViewModel
    {
        public string OwnerIP { get; set; }
        public string OwnerName { get; set; }
        public ItemTransferLog_VM ItemLog {get; set; }
    }
}
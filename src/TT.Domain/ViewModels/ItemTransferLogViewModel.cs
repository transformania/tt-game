using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class ItemTransferLogViewModel
    {
        public string OwnerIP { get; set; }
        public string OwnerName { get; set; }
        public ItemTransferLog_VM ItemLog {get; set; }
    }
}
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class MindControlViewModel
    {
        public MindControl MindControl { get; set; }
        public PlayerFormViewModel Victim { get; set; }
        public PlayerFormViewModel Master { get; set; }
        public string TypeFriendlyName { get; set;}
    }
}
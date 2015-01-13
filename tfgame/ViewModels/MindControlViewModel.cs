using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class MindControlViewModel
    {
        public MindControl MindControl { get; set; }
        public PlayerFormViewModel Victim { get; set; }
        public string TypeFriendlyName { get; set;}
    }
}
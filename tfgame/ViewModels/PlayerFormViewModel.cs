using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class PlayerFormViewModel
    {
        public Player_VM Player { get; set; }
        public Form Form { get; set; }
    }

    //public class PlayerFormViewModel2
    //{
    //    public Player_VM Player { get; set; }
    //    public Form Form { get; set; }
    //}
}
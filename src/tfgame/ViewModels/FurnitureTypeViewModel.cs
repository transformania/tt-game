using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class FurnitureViewModel
    {
        public Furniture_VM dbFurniture { get; set; }
        public StaticFurniture FurnitureType { get; set; }
    }
}
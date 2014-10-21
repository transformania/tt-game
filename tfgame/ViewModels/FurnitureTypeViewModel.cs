using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class FurnitureViewModel
    {
        public Furniture dbFurniture { get; set; }
        public DbStaticFurniture FurnitureType { get; set; }
    }
}
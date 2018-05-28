using System.Collections.Generic;

namespace TT.Domain.ViewModels
{
    public class UseFurnitureViewModel
    {
        public List<FurnitureViewModel> Furniture { get; set; }
        public string CovenantSafeground { get; set; }
        public int FurnitureLimit { get; set; }
        public bool AtCovenantSafeground { get; set; }
    }
}

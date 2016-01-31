using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class FurnitureViewModel
    {
        public Furniture_VM dbFurniture { get; set; }
        public StaticFurniture FurnitureType { get; set; }
    }
}
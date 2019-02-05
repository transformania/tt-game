namespace TT.Domain.ViewModels
{
    public class StaticFurniture
    {
        public int Id { get; set; }
        public string dbType { get; set; }
        public string FriendlyName { get; set; }
        public int? GivesEffectSourceId { get; set; }
        public decimal APReserveRefillAmount { get; set; }
        public decimal BaseCost { get; set; }
        public int BaseContractTurnLength { get; set; }
        public int? GivesItemSourceId { get; set; }
        public decimal MinutesUntilReuse { get; set; }
        public string Description { get; set; }
        public string PortraitUrl { get; set; }
    }
}
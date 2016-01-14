namespace tfgame.dbModels.Models
{
    public class LocationInfo
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public int CovenantId { get; set; }
        public float TakeoverAmount { get; set; }
        public int LastTakeoverTurn { get; set; }
    }
}
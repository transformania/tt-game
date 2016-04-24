namespace TT.Domain.ViewModels
{
    public class PublicBroadcastViewModel
    {
        public string Message { get; set; }
    }

    public class PlayerNameViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewForm { get; set; }
        public int Level { get; set; }
        public decimal Money { get; set; }
    }
}
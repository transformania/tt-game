namespace TT.Domain.Identity.DTOs
{
    public class DonatorDetail
    {
        public int Id { get; set; }
        public string PatreonName { get; set; }
        public int Tier { get; set; }
        public decimal ActualDonationAmount { get; set; }
        public string SpecialNotes { get; set; }
        public virtual UserDetail Owner { get; protected set; }
    }
}

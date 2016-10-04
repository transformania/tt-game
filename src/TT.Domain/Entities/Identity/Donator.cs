using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Domain.Entities.Identity
{
    public class Donator : Entity<int>
    {
        public string PatreonName { get; protected set; }
        public int Tier { get; protected set; }
        public decimal ActualDonationAmount { get; protected set; }
        public string SpecialNotes { get; protected set; }
        
        public virtual User Owner { get; protected set; }

        private Donator() { }

        public void Update(string patreonName, int tier, decimal actualDonationAmount, string specialNotes)
        {
            this.PatreonName = patreonName;
            this.Tier = tier;
            this.ActualDonationAmount = actualDonationAmount;
            this.SpecialNotes = specialNotes;
        }

        public static Donator Create(string patreonName, int tier, decimal actualDonationAmount, string specialNotes)
        {
            return new Donator
            {
                PatreonName = patreonName,
                Tier = tier,
                ActualDonationAmount = actualDonationAmount,
                SpecialNotes = specialNotes
            };
        }
    }
}

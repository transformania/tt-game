using System;

namespace TT.Domain.ViewModels
{
    public class BalancePageViewModel
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public decimal Balance { get; set; }
        public decimal AbsolutePoints { get; set; }
        public Boolean IsUnique { get; set; }
    }
}
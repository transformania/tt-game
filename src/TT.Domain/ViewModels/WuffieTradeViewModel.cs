using System.Collections.Generic;
using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels
{

    /// <summary>
    /// Stores the data used to render the page when interacting with Wuffie the pet merchant
    /// </summary>
    public class WuffieTradeViewModel
    {
        /// <summary>
        /// All of the pets Wuffie currently owns
        /// </summary>
        public IEnumerable<ItemDetail> Pets { get; set; }

        /// <summary>
        /// Stores pagination data
        /// </summary>
        public Paginator Paginator { get; set; }

        /// <summary>
        /// Returns whether or not the player interacting with Wuffie already has a pet and is free to buy/sell another
        /// </summary>
        public bool PlayerHasPet { get; set; }

        /// <summary>
        /// The amount of money the player interacting with Wuffie possesses
        /// </summary>
        public int Money { get; set; }

    }
}

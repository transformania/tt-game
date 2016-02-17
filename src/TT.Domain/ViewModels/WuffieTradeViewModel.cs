using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IEnumerable<ItemViewModel> Pets { get; set; }

        /// <summary>
        /// The count of pets Wuffie owns
        /// </summary>
        public int PetCount { get; set; }

        /// <summary>
        /// The highest page number to be rendered when paginating
        /// </summary>
        public int MaxPageSize { get; set; }

        /// <summary>
        /// The current page number that the viewer is looking at
        /// </summary>
        public int CurrentPage { get; set; }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.DTOs.Players;

namespace TT.Domain.ViewModels
{
    public class AddStrikeViewModel
    {
        public string UserId { get; set; }
        public string Reason { get; set; }

        public PlayerUserStrikesDetail PlayerUserStrikesDetail { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.ViewModels
{
    public class MoveLogBox
    {
        public string SourceLocationLog { get; set; }
        public string DestinationLocationLog { get; set; }
        public string PlayerLog { get; set; }
        public int ConcealmentLevel { get; set; }
    }
}

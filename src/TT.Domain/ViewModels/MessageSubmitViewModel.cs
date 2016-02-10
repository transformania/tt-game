using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.ViewModels
{
    public class MessageSubmitViewModel
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SendingToName { get; set; }

        public string MessageText { get; set; }

        public int responseToId { get; set; }
        public string RespondingToMsg { get; set; }
    }
}

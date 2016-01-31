using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels.Quest
{
    public class QuestsAtLocationViewModel
    {
        public IEnumerable<QuestStart> AllQuests { get; set; }
        public IEnumerable<QuestStart> AvailableQuests { get; set; }
    }
}
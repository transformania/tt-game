using System.Collections.Generic;
using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.ViewModels.Quest
{
    public class QuestPlayPageViewModel
    {
        public QuestStart QuestStart { get; set; }
        public QuestState QuestState { get; set; }
        public IEnumerable<QuestConnection> QuestConnections { get; set; }
        public PlayerFormViewModel Player { get; set; }
        public BuffBox BuffBox { get; set; }
        public IEnumerable<QuestPlayerVariable> QuestPlayerVariables { get; set; }
        public int NewMessages { get; set; }


        public bool ShowEnd()
        {
            if (this.QuestState.QuestEnds != null && this.QuestState.QuestEnds.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
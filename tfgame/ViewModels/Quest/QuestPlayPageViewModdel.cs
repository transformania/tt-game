using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels.Quest
{
    public class QuestPlayPageViewModel
    {
        public QuestStart QuestStart { get; set; }
        public QuestState QuestState { get; set; }
        public IEnumerable<QuestState> ChildQuestStates { get; set; }
        public PlayerFormViewModel Player { get; set; }
        public BuffBox BuffBox { get; set; }
    }
}
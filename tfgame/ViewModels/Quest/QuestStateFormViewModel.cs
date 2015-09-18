using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels.Quest
{
    public class QuestStateFormViewModel
    {
        public QuestState QuestState { get; set; }
        public QuestState ParentQuestState { get; set; }
        public IEnumerable<QuestState> ChildQuestStates { get; set; }
    }
}
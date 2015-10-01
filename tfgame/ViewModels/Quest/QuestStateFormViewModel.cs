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
        public QuestState JumpToQuestState { get; set; }
        public IEnumerable<QuestState> ChildQuestStates { get; set; }
        public IEnumerable<QuestEnd> QuestEnds { get; set; }
    }

    public class QuestStateRequirementFormViewModel
    {
        public QuestStateRequirement QuestStateRequirement { get; set; }
        public QuestState ParentQuestState { get; set; }
    }

    public class QuestStatePreactionFormViewModel
    {
        public QuestStatePreaction QuestStatePreaction { get; set; }
        public QuestState ParentQuestState { get; set; }
    }

    public class QuestEndFormViewModel
    {
        public QuestEnd QuestEnd { get; set; }
        public QuestState ParentQuestState { get; set; }
    }
}
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
        public IEnumerable<QuestEnd> QuestEnds { get; set; }
        public IEnumerable<QuestConnection> QuestConnectionsFrom { get; set; }
        public IEnumerable<QuestConnection> QuestConnectionsTo { get; set; }
        public IEnumerable<QuestConnection> QuestConnectionsFailTo { get; set; }
    }

    public class QuestConnectionFormViewModel
    {
        public QuestConnection QuestConnection { get; set; }
        public QuestState FromQuestState { get; set; }
        public QuestState ToQuestState { get; set; }
        public QuestState FailToQuestState { get; set; }
    }

    public class QuestConnectionRequirementFormViewModel
    {
        public QuestConnectionRequirement QuestConnectionRequirement { get; set; }
        public QuestConnection QuestConnection { get; set; }
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

    public class QuestStateJSONObject
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public int EndCount { get; set; }
        public bool IsStart { get; set; }
        public bool Pin { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
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
        public IEnumerable<QuestPlayerVariable> QuestPlayerVariables { get; set; }
        public int NewMessages { get; set; }

        public bool ShowChildrenStates()
        {
            if (this.ChildQuestStates != null && 
                this.ChildQuestStates.Count() > 0 && 
                this.QuestState.JumpToQuestStateId <= 0 && 
                this.QuestState.QuestEnds.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShowJump()
        {
            if (this.QuestState.JumpToQuestStateId > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


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
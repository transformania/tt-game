using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels.Quest
{
    public class QuestPlayPageViewModdel
    {
        public QuestStart QuestStart { get; set; }
        public QuestState QuestState { get; set; }
        public PlayerFormViewModel Player { get; set; }
    }
}
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
        public string ConnectionText { get; set; }

        /// <summary>
        /// Returns true if the player's current quest state has more than QuestEnd, in which case they are obligated to conclude the quest immediately
        /// </summary>
        /// <returns></returns>
        public bool ShowEnd()
        {
            if (this.QuestState.QuestEnds != null && this.QuestState.QuestEnds.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Appends an <hr> tag to the connection text if it is not empty
        /// </summary>
        /// <param name="input"></param>
        public void SetConnectionText(string input)
        {
            if (input != null && input.Length > 0)
            {
                this.ConnectionText = input + "<hr>";
            }
        }
    }
}
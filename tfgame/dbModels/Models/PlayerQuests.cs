using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class PlayerQuests
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int QuestId { get; set; }
        public int Outcome { get; set; }
    }
}

//[PlayerQuests]
//This table is used to keep track of which players have done which quests and their outcomes.
//[Id] -- int
//[PlayerId] - int.  Database name of the quest
//[QuestId] -- int.  The quest that this refers to
//[Outcome] - string.  Either “completed” or “failed.”
//[StartedTurn] - int.  The game turn that the player began this quest.
//[CompletedOrFailedTurn] - int.  The game turn that the player either last failed or completed this quest.
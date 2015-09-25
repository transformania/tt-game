using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class ServerLog
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public int Errors { get; set; }
        public string FullLog { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime FinishTimestamp { get; set; }
        public int Population { get; set; }

        public void AddLog(string input)
        {
            FullLog += "<p>" + input + "</p>";
        }

    }

   
}
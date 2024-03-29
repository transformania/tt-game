﻿using System;

namespace TT.Domain.Models
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

        public string GetErrorColor()
        {
            if (this.Errors == 1)
            {
                return "pink";
            }
            else if (this.Errors > 1)
            {
                return "red";
            }
            return "";
        }

    }

   
}
using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class NewsPost
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
        public int ViewState { get; set; }

        private string nl = Environment.NewLine;
        private string t = "   ";

        public enum ViewStateOptions
        {
            Hidden = 0,
            Live = 1,
            Archived = 2,
        };

        public NewsPost()
        {
            this.Timestamp = DateTime.UtcNow;
            this.Text = "<ul>" + nl + t + "<li>" + nl + t +  "</li>" + nl + t + "<li>" + nl + "</li>" + nl + t + "<li>" + nl + "</li>" + nl + "</ul>";
            this.ViewState = (int)ViewStateOptions.Hidden;
        }

        public string PrintVisiblity()
        {
            if (this.ViewState == (int)ViewStateOptions.Hidden)
            {
                return "Hidden";
            }
            else if (this.ViewState == (int)ViewStateOptions.Live)
            {
                return "Live";
            }
            else if (this.ViewState == (int)ViewStateOptions.Archived)
            {
                return "Archived";
            }
            return "";
        }
    }
}
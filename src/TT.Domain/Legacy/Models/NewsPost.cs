using System;

namespace TT.Domain.Models
{

    /// <summary>
    /// A piece of news associated with some change to the game such as new functionality, spells, fanart, or anything else worth making known to users
    /// </summary>
    public class NewsPost
    {

        /// <summary>
        /// Primary identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The timestamp associated with this piece of news.  Defaults to the day of creation but can be changed arbitrarily.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The HTML markup for the news post.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The visibility status for players.  States are Hidden (not shown to players yet, perhaps in the middle of being written), Live (visible to all), and Archived (visible to those only looking at the archived news pages.)
        /// </summary>
        public int ViewState { get; set; }

        private string nl = Environment.NewLine;
        private string t = "   ";

        public enum ViewStateOptions
        {
            Hidden = 0,
            Live = 1,
            Archived = 2,
        };

        /// <summary>
        /// Default constructor.  The timestamp is set to the current time in UTC.  Templated HTML markup is provided for use.  The news post will be marked as Hidden.
        /// </summary>
        public NewsPost()
        {
            this.Timestamp = DateTime.UtcNow;
            this.Text = "<ul>" + nl + t + "<li></li>" + nl + t + "<li></li>" + nl + t + "<li></li>" + nl + "</ul>";
            this.ViewState = (int)ViewStateOptions.Hidden;
        }

        /// <summary>
        /// Returns a string representing the visibility status of this news post
        /// </summary>
        /// <returns></returns>
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
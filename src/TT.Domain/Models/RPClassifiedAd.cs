using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{

    /// <summary>
    /// Holds some information about an RP Classified Ad including description, desired themes, preferred timezone, etc
    /// </summary>
    public class RPClassifiedAd
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime RefreshTimestamp { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Sets fields that have null strings to empty strings instead for some easier server side validation
        /// </summary>
        public void SetNullsToEmptyStrings()
        {
            if (this.Title == null)
            {
                this.Title = "";
            }

            if (this.Text == null)
            {
                this.Text = "";
            }

            if (this.YesThemes == null)
            {
                this.YesThemes = "";
            }

            if (this.NoThemes == null)
            {
                this.NoThemes = "";
            }

            if (this.PreferredTimezones == null)
            {
                this.PreferredTimezones = "";
            }

        }
    }
}
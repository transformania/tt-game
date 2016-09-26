using System;
using System.ComponentModel.DataAnnotations;
using TT.Domain.Procedures;

namespace TT.Domain.Models
{
    public class PlayerBio
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public string Text { get; set; }
        public string WebsiteURL { get; set; }
        public int PublicVisibility { get; set; }
        public string OtherNames { get; set; }
        public string Tags { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsDonator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerBio"/> class. Used by EF to create this entity.
        /// </summary>
        public PlayerBio()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerBio"/> class. All of it's fields are set to the default value.
        /// </summary>
        /// <param name="ownerMembershipId">The id of the owner that should be used when setting <see cref="IsDonator"/>.</param>
        public PlayerBio(string ownerMembershipId)
        {
            Player owner = PlayerProcedures.GetPlayerFromMembership(ownerMembershipId);

            OwnerMembershipId = ownerMembershipId;
            Timestamp = DateTime.UtcNow;
            WebsiteURL = "";
            Text = "";
            IsDonator = DonatorProcedures.DonatorGetsMessagesRewards(owner);
        }
    }
}
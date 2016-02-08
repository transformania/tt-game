using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class ContributorCustomForm
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public virtual DbStaticForm CustomForm { get; set; }
    }
}
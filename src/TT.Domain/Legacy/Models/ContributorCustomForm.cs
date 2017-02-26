using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Domain.Models
{
    public class ContributorCustomForm
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public int CustomForm_Id { get; set; }
        [ForeignKey("CustomForm_Id")]
        public virtual DbStaticForm CustomForm { get; set; }
    }
}
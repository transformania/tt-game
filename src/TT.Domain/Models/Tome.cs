using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TT.Domain.Models
{
    public class Tome
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int BaseItemId { get; set; }
        [ForeignKey("BaseItemId")]
        public virtual DbStaticItem BaseItem { get; set; }

    }
}

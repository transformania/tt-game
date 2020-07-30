using System.Collections.Generic;
using TT.Domain.Items.DTOs;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class ItemDetailsModel
    {
        public ItemDetail Item { get; set; }
        public IEnumerable<DbStaticSkill> Skills { get; set; }
    }
}

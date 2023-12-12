using System.Collections.Generic;
using TT.Domain.Models;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.ViewModels
{
    public class SelfCastViewModel
    {
        public int ItemId { get; set; }
        public IEnumerable<SkillSourceFormSourceDetail> Skills { get; set; }
        public IEnumerable<DbStaticForm> Forms { get; set; }
        public ItemViewModel Item { get; set; }
        public Player ItemPlayer { get; set; }
        public decimal OwnerMoney { get; set; }
    }
}

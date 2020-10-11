using System.Collections.Generic;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.ViewModels
{
    public class SelfCastViewModel
    {
        public int ItemId { get; set; }
        public IEnumerable<SkillSourceFormSourceDetail> Skills { get; set; }
    }
}

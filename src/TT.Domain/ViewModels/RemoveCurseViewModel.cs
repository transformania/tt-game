using System.Collections.Generic;

namespace TT.Domain.ViewModels
{
    public class RemoveCurseViewModel
    {
        public IEnumerable<EffectViewModel2> Effects { get; set; }
        public ItemViewModel Item { get; set; }
    }
}

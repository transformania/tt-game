using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class EffectViewModel
    {
        public Effect dbEffect {  get; set;}
        public StaticEffect Effect { get; set; }
    }

    public class EffectViewModel2
    {
        public Effect_VM dbEffect { get; set; }
        public StaticEffect Effect { get; set; }
    }
}
using TT.Domain.Players.DTOs;

namespace TT.Domain.Effects.DTOs
{
    public class EffectDetail
    {
        public PlayerDetail Owner { get;  set; }
        public int Duration { get;  set; }
        public bool IsPermanent { get;  set; }
        public int Level { get;  set; }
        public int Cooldown { get;  set; }
        public EffectSourceDetail EffectSource { get;  set; }
    }
}

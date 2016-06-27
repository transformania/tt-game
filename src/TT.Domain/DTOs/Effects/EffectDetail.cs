using TT.Domain.DTOs.Players;

namespace TT.Domain.DTOs.Effects
{
    public class EffectDetail
    {
        public PlayerDetail Owner { get;  set; }
        public string dbName { get;  set; }
        public int Duration { get;  set; }
        public bool IsPermanent { get;  set; }
        public int Level { get;  set; }
        public int Cooldown { get;  set; }
        public EffectSourceDetail EffectSource { get;  set; }
    }
}

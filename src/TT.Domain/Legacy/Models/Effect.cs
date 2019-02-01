namespace TT.Domain.Models
{
    public class Effect
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int EffectSourceId { get; set; }
        public int Duration { get; set; }
        public bool IsPermanent { get; set; }
        public int Level { get; set; }
        public int Cooldown { get; set; }
    }

    public class Effect_VM
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int EffectSourceId { get; set; }
        public int Duration { get; set; }
        public bool IsPermanent { get; set; }
        public int Level { get; set; }
        public int Cooldown { get; set; }
    }
}
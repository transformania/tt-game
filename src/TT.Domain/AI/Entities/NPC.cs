using TT.Domain.AI.Commands;
using TT.Domain.Entities;

namespace TT.Domain.AI.Entities
{
    public class NPC : Entity<int>
    {

        public string SpawnText { get; protected set; }

        private NPC() { }

        public static NPC Create(string spawnText)
        {
            return new NPC
            {
                SpawnText = spawnText
            };
        }

        public NPC Update(UpdateNPC cmd)
        {
            Id = cmd.NPCId;
            SpawnText = cmd.SpawnText;
            return this;
        }
    }
}
using TT.Domain.Commands.AI;

namespace TT.Domain.Entities.NPCs
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
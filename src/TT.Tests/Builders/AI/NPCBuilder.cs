using TT.Domain.AI.Entities;

namespace TT.Tests.Builders.AI
{
    public class NPCBuilder : Builder<NPC, int>
    {
        public NPCBuilder()
        {
            Instance = Create();
            With(x => x.SpawnText, "this is the spawn text");
        }
    }
}
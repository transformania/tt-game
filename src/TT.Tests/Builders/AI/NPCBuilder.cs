using System;
using TT.Domain.AI.Entities;
using TT.Tests.Builders.Identity;

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
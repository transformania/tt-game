
using TT.Domain.Entities.Skills;

namespace TT.Tests.Builders.Skills
{
    public class SkillBuilder : Builder<Skill, int>
    {
        public SkillBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}

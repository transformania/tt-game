using TT.Domain.Entities.Skills;

namespace TT.Tests.Builders.Skills
{
    public class SkillSourceBuilder : Builder<SkillSource, int>
    {
        public SkillSourceBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}

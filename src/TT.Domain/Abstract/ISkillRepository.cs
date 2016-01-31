using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ISkillRepository
    {

        IQueryable<Skill> Skills { get; }

        IQueryable<DbStaticSkill> DbStaticSkills { get; }

        IQueryable<DbStaticForm> DbStaticForms { get; }

        void SaveSkill(Skill Skill);

        void DeleteSkill(int SkillId);

    }
}
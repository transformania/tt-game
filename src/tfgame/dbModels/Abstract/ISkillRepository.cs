using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
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
using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDbStaticSkillRepository
    {

        IQueryable<DbStaticSkill> DbStaticSkills { get; }

        void SaveDbStaticSkill(DbStaticSkill DbStaticSkill);

        void DeleteDbStaticSkill(int DbStaticSkillId);

    }
}
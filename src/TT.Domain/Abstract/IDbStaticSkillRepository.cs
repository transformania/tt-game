using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDbStaticSkillRepository
    {

        IQueryable<DbStaticSkill> DbStaticSkills { get; }

        void SaveDbStaticSkill(DbStaticSkill DbStaticSkill);

        void DeleteDbStaticSkill(int DbStaticSkillId);

    }
}
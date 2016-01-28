using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFDbStaticSkillRepository : IDbStaticSkillRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<DbStaticSkill> DbStaticSkills
        {
            get { return context.DbStaticSkills; }
        }

        public void SaveDbStaticSkill(DbStaticSkill DbStaticSkill)
        {
            if (DbStaticSkill.Id == 0)
            {
                context.DbStaticSkills.Add(DbStaticSkill);
            }
            else
            {
                DbStaticSkill editMe = context.DbStaticSkills.Find(DbStaticSkill.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DbStaticSkill.Name;
                    // dbEntry.Message = DbStaticSkill.Message;
                    // dbEntry.TimeStamp = DbStaticSkill.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticSkill(int id)
        {

            DbStaticSkill dbEntry = context.DbStaticSkills.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticSkills.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
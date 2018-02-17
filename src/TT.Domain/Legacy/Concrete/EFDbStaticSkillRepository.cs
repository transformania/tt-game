using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
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
                var editMe = context.DbStaticSkills.Find(DbStaticSkill.Id);
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

            var dbEntry = context.DbStaticSkills.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticSkills.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
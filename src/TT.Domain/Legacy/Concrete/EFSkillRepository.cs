using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFSkillRepository : ISkillRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Skill> Skills
        {
            get { return context.Skills; }
        }

        public IQueryable<DbStaticSkill> DbStaticSkills
        {
            get { return context.DbStaticSkills; }
        }

        public IQueryable<DbStaticForm> DbStaticForms
        {
            get { return context.DbStaticForms;  }
        }

        public void SaveSkill(Skill Skill)
        {
            if (Skill.Id == 0)
            {
                context.Skills.Add(Skill);
            }
            else
            {
                var editMe = context.Skills.Find(Skill.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = StaticSkill.Name;
                    // dbEntry.Message = StaticSkill.Message;
                    // dbEntry.TimeStamp = StaticSkill.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteSkill(int id)
        {

            var dbEntry = context.Skills.Find(id);
            if (dbEntry != null)
            {
                context.Skills.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
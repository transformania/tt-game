using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
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
                Skill editMe = context.Skills.Find(Skill.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Skill.Name;
                    // dbEntry.Message = Skill.Message;
                    // dbEntry.TimeStamp = Skill.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteSkill(int id)
        {

            Skill dbEntry = context.Skills.Find(id);
            if (dbEntry != null)
            {
                context.Skills.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
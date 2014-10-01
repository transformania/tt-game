using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFDbStaticFormRepository : IDbStaticFormRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<DbStaticForm> DbStaticForms
        {
            get { return context.DbStaticForms; }
        }

        public void SaveDbStaticForm(DbStaticForm DbStaticForm)
        {
            if (DbStaticForm.Id == 0)
            {
                context.DbStaticForms.Add(DbStaticForm);
            }
            else
            {
                DbStaticForm editMe = context.DbStaticForms.Find(DbStaticForm.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DbStaticForm.Name;
                    // dbEntry.Message = DbStaticForm.Message;
                    // dbEntry.TimeStamp = DbStaticForm.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticForm(int id)
        {

            DbStaticForm dbEntry = context.DbStaticForms.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticForms.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
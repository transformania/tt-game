using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFMindControlRepository : IMindControlRepository
    {
        private StatsContext context = new StatsContext();


        public IQueryable<MindControl> MindControls
        {
            get { return context.MindControls; }
        }


        public void SaveMindControl(MindControl MindControl)
        {
            if (MindControl.Id == 0)
            {
                context.MindControls.Add(MindControl);
            }
            else
            {
                MindControl editMe = context.MindControls.Find(MindControl.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Effect.Name;
                    // dbEntry.Message = Effect.Message;
                    // dbEntry.TimeStamp = Effect.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteMindControl(int id)
        {

            MindControl dbEntry = context.MindControls.Find(id);
            if (dbEntry != null)
            {
                context.MindControls.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
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
                var editMe = context.MindControls.Find(MindControl.Id);
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

            var dbEntry = context.MindControls.Find(id);
            if (dbEntry != null)
            {
                context.MindControls.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
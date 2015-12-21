using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFEffectRepository : IEffectRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Effect> Effects
        {
            get { return context.Effects; }
        }

        public IQueryable<DbStaticEffect> DbStaticEffects
        {
            get { return context.DbStaticEffects; }
        }

        public void SaveEffect(Effect Effect)
        {
            if (Effect.Id == 0)
            {
                context.Effects.Add(Effect);
            }
            else
            {
                Effect editMe = context.Effects.Find(Effect.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Effect.Name;
                    // dbEntry.Message = Effect.Message;
                    // dbEntry.TimeStamp = Effect.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteEffect(int id)
        {

            Effect dbEntry = context.Effects.Find(id);
            if (dbEntry != null)
            {
                context.Effects.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void SaveDbStaticEffect(DbStaticEffect Effect)
        {
            if (Effect.Id == 0)
            {
                context.DbStaticEffects.Add(Effect);
            }
            else
            {
                DbStaticEffect editMe = context.DbStaticEffects.Find(Effect.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Effect.Name;
                    // dbEntry.Message = Effect.Message;
                    // dbEntry.TimeStamp = Effect.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticEffect(int id)
        {

            DbStaticEffect dbEntry = context.DbStaticEffects.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticEffects.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
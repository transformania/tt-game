﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFDbStaticEffectRepository : IDbStaticEffectRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<DbStaticEffect> DbStaticEffects
        {
            get { return context.DbStaticEffects; }
        }

        public void SaveDbStaticEffect(DbStaticEffect DbStaticEffect)
        {
            if (DbStaticEffect.Id == 0)
            {
                context.DbStaticEffects.Add(DbStaticEffect);
            }
            else
            {
                var editMe = context.DbStaticEffects.Find(DbStaticEffect.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DbStaticEffect.Name;
                    // dbEntry.Message = DbStaticEffect.Message;
                    // dbEntry.TimeStamp = DbStaticEffect.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticEffect(int id)
        {

            var dbEntry = context.DbStaticEffects.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticEffects.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
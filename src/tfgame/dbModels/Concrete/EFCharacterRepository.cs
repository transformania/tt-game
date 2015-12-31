using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;


namespace tfgame.dbModels.Concrete
{
    public class EFCharacterRepository : ICharacterRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Character> Characters
        {
            get { return context.Characters; }
        }

        public void SaveCharacter(Character Character)
        {
            if (Character.Id == 0)
            {
                context.Characters.Add(Character);
            }
            else
            {
                Character editMe = context.Characters.Find(Character.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Characters.Name;
                    // dbEntry.Message = Characters.Message;
                    // dbEntry.TimeStamp = Characters.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteCharacter(int id)
        {

            Character dbEntry = context.Characters.Find(id);
            if (dbEntry != null)
            {
                context.Characters.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
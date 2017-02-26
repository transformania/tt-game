using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFJewdewfaeEncounterRepository : IJewdewfaeEncounterRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<JewdewfaeEncounter> JewdewfaeEncounters
        {
            get { return context.JewdewfaeEncounters; }
        }

        public void SaveJewdewfaeEncounter(JewdewfaeEncounter JewdewfaeEncounter)
        {
            if (JewdewfaeEncounter.Id == 0)
            {
                context.JewdewfaeEncounters.Add(JewdewfaeEncounter);
            }
            else
            {
                JewdewfaeEncounter editMe = context.JewdewfaeEncounters.Find(JewdewfaeEncounter.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = JewdewfaeEncounter.Name;
                    // dbEntry.Message = JewdewfaeEncounter.Message;
                    // dbEntry.TimeStamp = JewdewfaeEncounter.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteJewdewfaeEncounter(int id)
        {

            JewdewfaeEncounter dbEntry = context.JewdewfaeEncounters.Find(id);
            if (dbEntry != null)
            {
                context.JewdewfaeEncounters.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
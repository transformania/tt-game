using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFAIDirectiveRepository : IAIDirectiveRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<AIDirective> AIDirectives
        {
            get { return context.AIDirectives; }
        }

        public void SaveAIDirective(AIDirective AIDirective)
        {
            if (AIDirective.Id == 0)
            {
                context.AIDirectives.Add(AIDirective);
            }
            else
            {
                var editMe = context.AIDirectives.Find(AIDirective.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = AIDirective.Name;
                    // dbEntry.Message = AIDirective.Message;
                    // dbEntry.TimeStamp = AIDirective.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteAIDirective(int id)
        {

            var dbEntry = context.AIDirectives.Find(id);
            if (dbEntry != null)
            {
                context.AIDirectives.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
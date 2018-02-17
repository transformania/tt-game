using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFTFMessageRepository : ITFMessageRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<TFMessage> TFMessages
        {
            get { return context.TFMessages; }
        }

        public void SaveTFMessage(TFMessage TFMessage)
        {
            if (TFMessage.Id == 0)
            {
                context.TFMessages.Add(TFMessage);
            }
            else
            {
                var editMe = context.TFMessages.Find(TFMessage.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = TFMessage.Name;
                    // dbEntry.Message = TFMessage.Message;
                    // dbEntry.TimeStamp = TFMessage.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteTFMessage(int id)
        {

            var dbEntry = context.TFMessages.Find(id);
            if (dbEntry != null)
            {
                context.TFMessages.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
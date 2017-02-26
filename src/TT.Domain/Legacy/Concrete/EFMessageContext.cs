using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFMessageRepository : IMessageRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Message> Messages
        {
            get { return context.Messages; }
        }

        public void SaveMessage(Message Message)
        {
            if (Message.Id == 0)
            {
                context.Messages.Add(Message);
            }
            else
            {
                Message editMe = context.Messages.Find(Message.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Message.Name;
                    // dbEntry.Message = Message.Message;
                    // dbEntry.TimeStamp = Message.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteMessage(int id)
        {

            Message dbEntry = context.Messages.Find(id);
            if (dbEntry != null)
            {
                context.Messages.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
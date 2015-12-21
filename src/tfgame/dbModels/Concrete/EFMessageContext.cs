using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
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
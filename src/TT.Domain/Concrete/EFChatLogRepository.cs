using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFChatLogRepository : IChatLogRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<ChatLog> ChatLogs
        {
            get { return context.ChatLogs; }
        }

        public void SaveChatLog(ChatLog ChatLog)
        {
            if (ChatLog.Id == 0)
            {
                context.ChatLogs.Add(ChatLog);
            }
            else
            {
                ChatLog editMe = context.ChatLogs.Find(ChatLog.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ChatLog.Name;
                    // dbEntry.Message = ChatLog.Message;
                    // dbEntry.TimeStamp = ChatLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteChatLog(int id)
        {

            ChatLog dbEntry = context.ChatLogs.Find(id);
            if (dbEntry != null)
            {
                context.ChatLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}
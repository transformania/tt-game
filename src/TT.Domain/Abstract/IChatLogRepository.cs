using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IChatLogRepository
    {

        IQueryable<ChatLog> ChatLogs { get; }

        void SaveChatLog(ChatLog ChatLog);

        void DeleteChatLog(int ChatLogId);

    }
}
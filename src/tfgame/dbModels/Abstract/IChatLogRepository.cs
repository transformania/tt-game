using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IChatLogRepository
    {

        IQueryable<ChatLog> ChatLogs { get; }

        void SaveChatLog(ChatLog ChatLog);

        void DeleteChatLog(int ChatLogId);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IMessageRepository
    {

        IQueryable<Message> Messages { get; }

        void SaveMessage(Message Message);

        void DeleteMessage(int MessageId);

    }
}
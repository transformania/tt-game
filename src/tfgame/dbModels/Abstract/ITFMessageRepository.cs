using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ITFMessageRepository
    {

        IQueryable<TFMessage> TFMessages { get; }

        void SaveTFMessage(TFMessage TFMessage);

        void DeleteTFMessage(int TFMessageId);

    }
}
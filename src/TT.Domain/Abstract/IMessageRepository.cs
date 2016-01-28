using System.Linq;
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
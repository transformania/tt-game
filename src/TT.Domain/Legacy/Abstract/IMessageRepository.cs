using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IMessageRepository
    {

        IQueryable<Message> Messages { get; }

        void SaveMessage(Message Message);

        void DeleteMessage(int MessageId);

    }
}
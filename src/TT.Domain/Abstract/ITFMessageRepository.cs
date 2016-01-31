using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ITFMessageRepository
    {

        IQueryable<TFMessage> TFMessages { get; }

        void SaveTFMessage(TFMessage TFMessage);

        void DeleteTFMessage(int TFMessageId);

    }
}
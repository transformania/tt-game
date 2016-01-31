using System.Collections.Generic;

namespace TT.Domain.Abstract
{
    public interface IRoot
    {
        IList<T> Find<T>(Query<T> query);
        T Find<T>(QuerySingle<T> query);
        void Execute(Command command);
        T Execute<T>(Command<T> command);
    }
}
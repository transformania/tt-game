using System.Collections.Generic;
using tfgame.dbModels.Abstract;

namespace tfgame.dbModels
{
    /// <summary>
    /// The 'mother' of all the objects in the domain.  Acts as a point from which
    /// domain-logic can be mocked. 
    /// http://broloco.blogspot.com/2009/05/who-is-responsible-party-when-nobody.html
    /// </summary>
    public class Root : IRoot
    {
        public IList<T> Find<T>(Query<T> query)
        {
            return query.Find();
        }

        public T Find<T>(QuerySingle<T> query)
        {
            return query.FindSingle();
        }

        public void Execute(Command command)
        {
            command.InternalExecute();
        }

        public T Execute<T>(Command<T> command)
        {
            return command.InternalExecute();
        }
    }
}